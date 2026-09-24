using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Endpoint que recibe los mensajes de WhatsApp.
/// GET  /api/whatsapp -> verificación del webhook que hace Meta una sola vez.
/// POST /api/whatsapp -> mensajes entrantes y estados de entrega.
/// </summary>
public static class WhatsAppWebhook
{
    // Meta reintenta si no respondemos rápido; sin esto se contestaría dos veces el mismo mensaje.
    private static readonly ConcurrentDictionary<string, DateTime> _procesados = new();

    public static IEndpointRouteBuilder MapWhatsAppWebhook(this IEndpointRouteBuilder rutas)
    {
        var grupo = rutas.MapGroup("/api/whatsapp").AllowAnonymous();

        grupo.MapGet("", Verificar);
        grupo.MapPost("", Recibir).DisableAntiforgery();

        return rutas;
    }

    private static IResult Verificar(HttpContext ctx, IOptions<WhatsAppOptions> opciones, ILoggerFactory logs)
    {
        var log = logs.CreateLogger("WhatsApp.Webhook");
        var q = ctx.Request.Query;
        var modo = q["hub.mode"].ToString();
        var token = q["hub.verify_token"].ToString();
        var reto = q["hub.challenge"].ToString();

        if (!opciones.Value.PuedeVerificar)
        {
            log.LogError("WhatsApp: llegó una verificación pero no hay VerifyToken configurado.");
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        if (modo == "subscribe" && CadenasIguales(token, opciones.Value.VerifyToken))
        {
            log.LogInformation("WhatsApp: webhook verificado por Meta.");
            return Results.Text(reto, "text/plain");
        }

        log.LogWarning("WhatsApp: verificación rechazada (modo '{Modo}', token incorrecto).", modo);
        return Results.StatusCode(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> Recibir(
        HttpContext ctx,
        IOptions<WhatsAppOptions> opcionesAcc,
        WhatsAppConversacion conversacion,
        ILoggerFactory logs,
        CancellationToken ct)
    {
        var log = logs.CreateLogger("WhatsApp.Webhook");
        var opciones = opcionesAcc.Value;

        string cuerpo;
        using (var lector = new StreamReader(ctx.Request.Body, Encoding.UTF8))
            cuerpo = await lector.ReadToEndAsync(ct);

        if (opciones.ValidarFirma && !FirmaValida(cuerpo, ctx.Request.Headers["X-Hub-Signature-256"].ToString(), opciones.AppSecret))
        {
            log.LogWarning("WhatsApp: mensaje descartado por firma inválida.");
            return Results.StatusCode(StatusCodes.Status401Unauthorized);
        }

        // A Meta siempre se le responde 200: si devolvemos error, reintenta y duplica el mensaje.
        try
        {
            foreach (var mensaje in Extraer(cuerpo, log))
            {
                if (!_procesados.TryAdd(mensaje.Id, DateTime.UtcNow)) continue;
                await conversacion.AtenderAsync(mensaje, ct);
            }
            LimpiarProcesados();
        }
        catch (Exception ex)
        {
            log.LogError(ex, "WhatsApp: error procesando el mensaje entrante.");
        }

        return Results.Ok();
    }

    /// <summary>Saca los mensajes del JSON de Meta. Ignora los avisos de estado (enviado, entregado, leído).</summary>
    private static List<MensajeWhatsApp> Extraer(string json, ILogger log)
    {
        var mensajes = new List<MensajeWhatsApp>();
        if (string.IsNullOrWhiteSpace(json)) return mensajes;

        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("entry", out var entradas)) return mensajes;

        foreach (var entrada in entradas.EnumerateArray())
        {
            if (!entrada.TryGetProperty("changes", out var cambios)) continue;

            foreach (var cambio in cambios.EnumerateArray())
            {
                if (!cambio.TryGetProperty("value", out var valor)) continue;
                if (!valor.TryGetProperty("messages", out var lista)) continue;

                var nombres = new Dictionary<string, string>();
                if (valor.TryGetProperty("contacts", out var contactos))
                {
                    foreach (var c in contactos.EnumerateArray())
                    {
                        var wa = c.TryGetProperty("wa_id", out var w) ? w.GetString() : null;
                        var nombre = c.TryGetProperty("profile", out var p) && p.TryGetProperty("name", out var n)
                            ? n.GetString() : null;
                        if (wa is not null && nombre is not null) nombres[wa] = nombre;
                    }
                }

                foreach (var m in lista.EnumerateArray())
                {
                    var id = m.TryGetProperty("id", out var i) ? i.GetString() : null;
                    var de = m.TryGetProperty("from", out var f) ? f.GetString() : null;
                    var tipo = m.TryGetProperty("type", out var t) ? t.GetString() ?? "" : "";
                    if (id is null || de is null) continue;

                    var (texto, opcionId) = LeerContenido(m, tipo);

                    mensajes.Add(new MensajeWhatsApp(
                        Id: id,
                        De: de,
                        Nombre: nombres.TryGetValue(de, out var nom) ? nom : "",
                        Tipo: tipo,
                        Texto: texto,
                        OpcionId: opcionId));
                }
            }
        }

        if (mensajes.Count == 0) log.LogDebug("WhatsApp: notificación sin mensajes (probablemente un estado de entrega).");
        return mensajes;
    }

    private static (string Texto, string? OpcionId) LeerContenido(JsonElement m, string tipo)
    {
        switch (tipo)
        {
            case "text":
                return (m.TryGetProperty("text", out var t) && t.TryGetProperty("body", out var b)
                    ? b.GetString() ?? "" : "", null);

            case "interactive":
                if (!m.TryGetProperty("interactive", out var inter)) return ("", null);
                if (inter.TryGetProperty("button_reply", out var btn))
                    return (btn.TryGetProperty("title", out var tb) ? tb.GetString() ?? "" : "",
                            btn.TryGetProperty("id", out var ib) ? ib.GetString() : null);
                if (inter.TryGetProperty("list_reply", out var lst))
                    return (lst.TryGetProperty("title", out var tl) ? tl.GetString() ?? "" : "",
                            lst.TryGetProperty("id", out var il) ? il.GetString() : null);
                return ("", null);

            case "button": // respuesta a una plantilla con botones
                return (m.TryGetProperty("button", out var bo) && bo.TryGetProperty("text", out var tx)
                    ? tx.GetString() ?? "" : "", null);

            default:
                return ("", null);
        }
    }

    private static bool FirmaValida(string cuerpo, string cabecera, string appSecret)
    {
        if (string.IsNullOrWhiteSpace(appSecret) || string.IsNullOrWhiteSpace(cabecera)) return false;
        if (!cabecera.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase)) return false;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var calculada = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(cuerpo))).ToLowerInvariant();
        return CadenasIguales(calculada, cabecera["sha256=".Length..].ToLowerInvariant());
    }

    /// <summary>Comparación en tiempo constante para no filtrar el token ni la firma.</summary>
    private static bool CadenasIguales(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));

    private static void LimpiarProcesados()
    {
        if (_procesados.Count < 500) return;
        var limite = DateTime.UtcNow.AddMinutes(-30);
        foreach (var par in _procesados)
            if (par.Value < limite) _procesados.TryRemove(par.Key, out _);
    }
}

/// <summary>Un mensaje entrante ya interpretado.</summary>
public record MensajeWhatsApp(string Id, string De, string Nombre, string Tipo, string Texto, string? OpcionId);
