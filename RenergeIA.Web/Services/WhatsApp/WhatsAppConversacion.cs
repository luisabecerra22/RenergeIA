namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Lógica del bot: menú de opciones. Es deliberadamente determinista —el código decide, no un modelo—
/// para que las respuestas sean predecibles y auditables.
/// Por ahora solo responde el menú; cada opción se irá conectando a la plataforma por fases.
/// </summary>
public class WhatsAppConversacion
{
    private readonly WhatsAppClient _wa;
    private readonly ILogger<WhatsAppConversacion> _log;

    public WhatsAppConversacion(WhatsAppClient wa, ILogger<WhatsAppConversacion> log)
    {
        _wa = wa;
        _log = log;
    }

    public async Task AtenderAsync(MensajeWhatsApp mensaje, CancellationToken ct = default)
    {
        _log.LogInformation("WhatsApp: mensaje de {Numero} ({Nombre}), tipo {Tipo}, opción {Opcion}: {Texto}",
            mensaje.De, mensaje.Nombre, mensaje.Tipo, mensaje.OpcionId ?? "-", mensaje.Texto);

        await _wa.MarcarLeidoAsync(mensaje.Id, ct);

        var opcion = mensaje.OpcionId ?? Interpretar(mensaje.Texto);

        switch (opcion)
        {
            case "alertas":
                await _wa.EnviarTextoAsync(mensaje.De,
                    "📋 *Alertas y vencimientos*\n\n" +
                    "Aquí verás los documentos por vencer y tus pendientes de planificación documental.\n\n" +
                    "Esta opción se conecta en la fase 1. Escribe *menú* para volver.", ct);
                break;

            case "consultas":
                await _wa.EnviarTextoAsync(mensaje.De,
                    "📊 *Consultas del proyecto*\n\n" +
                    "Aquí podrás preguntar por avance, documentos y costos.\n\n" +
                    "Esta opción se conecta en la fase 2. Escribe *menú* para volver.", ct);
                break;

            case "informe":
                await _wa.EnviarTextoAsync(mensaje.De,
                    "📝 *Informe diario*\n\n" +
                    "Aquí registrarás avance, clima y fotografías desde campo.\n\n" +
                    "Esta opción se conecta en la fase 4. Escribe *menú* para volver.", ct);
                break;

            default:
                await EnviarMenuAsync(mensaje, ct);
                break;
        }
    }

    private Task EnviarMenuAsync(MensajeWhatsApp mensaje, CancellationToken ct)
    {
        var saludo = string.IsNullOrWhiteSpace(mensaje.Nombre)
            ? "Hola 👋"
            : $"Hola {PrimerNombre(mensaje.Nombre)} 👋";

        return _wa.EnviarBotonesAsync(mensaje.De,
            $"{saludo}\n\nSoy el asistente de *RenergeIA*. ¿Con qué te ayudo?",
            new[]
            {
                ("alertas", "📋 Alertas"),
                ("consultas", "📊 Consultas"),
                ("informe", "📝 Informe diario")
            }, ct);
    }

    /// <summary>Permite responder con número o con palabras, no solo con los botones.</summary>
    private static string? Interpretar(string texto)
    {
        var t = Normalizar(texto);
        if (t.Length == 0) return null;

        if (t is "1" || t.Contains("alerta") || t.Contains("vencim") || t.Contains("pendiente")) return "alertas";
        if (t is "2" || t.Contains("consulta") || t.Contains("avance") || t.Contains("costo") || t.Contains("documento")) return "consultas";
        if (t is "3" || t.Contains("informe") || t.Contains("reporte") || t.Contains("diario")) return "informe";

        return null; // cualquier otra cosa devuelve el menú
    }

    private static string Normalizar(string texto)
    {
        var sinTildes = new string(texto.Trim().ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD)
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                        != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());
        return sinTildes.Normalize(System.Text.NormalizationForm.FormC);
    }

    private static string PrimerNombre(string nombre) =>
        nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? nombre;
}
