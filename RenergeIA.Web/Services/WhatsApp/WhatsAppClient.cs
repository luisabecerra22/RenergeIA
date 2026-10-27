using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Envío de mensajes por la Cloud API de Meta.
/// Fuera de la ventana de 24 horas solo se pueden enviar plantillas aprobadas (EnviarPlantillaAsync).
/// </summary>
public class WhatsAppClient
{
    private readonly HttpClient _http;
    private readonly WhatsAppOptions _opciones;
    private readonly ILogger<WhatsAppClient> _log;

    public WhatsAppClient(HttpClient http, IOptions<WhatsAppOptions> opciones, ILogger<WhatsAppClient> log)
    {
        _http = http;
        _opciones = opciones.Value;
        _log = log;
    }

    public Task<bool> EnviarTextoAsync(string numero, string texto, CancellationToken ct = default) =>
        EnviarAsync(new
        {
            messaging_product = "whatsapp",
            to = numero,
            type = "text",
            text = new { preview_url = false, body = texto }
        }, ct);

    /// <summary>Mensaje con hasta 3 botones de respuesta rápida.</summary>
    public Task<bool> EnviarBotonesAsync(string numero, string cuerpo,
        IEnumerable<(string Id, string Titulo)> botones, CancellationToken ct = default) =>
        EnviarAsync(new
        {
            messaging_product = "whatsapp",
            to = numero,
            type = "interactive",
            interactive = new
            {
                type = "button",
                body = new { text = cuerpo },
                action = new
                {
                    // Meta acepta máximo 3 botones y 20 caracteres por título
                    buttons = botones.Take(3).Select(b => new
                    {
                        type = "reply",
                        reply = new { id = b.Id, title = Recortar(b.Titulo, 20) }
                    }).ToArray()
                }
            }
        }, ct);

    /// <summary>Plantilla aprobada; es la única forma de escribir fuera de la ventana de 24 horas.</summary>
    public Task<bool> EnviarPlantillaAsync(string numero, string plantilla, string idioma = "es",
        IEnumerable<string>? parametros = null, CancellationToken ct = default)
    {
        object componentes = parametros is null
            ? Array.Empty<object>()
            : new object[]
            {
                new
                {
                    type = "body",
                    parameters = parametros.Select(p => new { type = "text", text = p }).ToArray()
                }
            };

        return EnviarAsync(new
        {
            messaging_product = "whatsapp",
            to = numero,
            type = "template",
            template = new
            {
                name = plantilla,
                language = new { code = idioma },
                components = componentes
            }
        }, ct);
    }

    /// <summary>Marca el mensaje como leído (el doble check azul) para que la persona sepa que llegó.</summary>
    public Task<bool> MarcarLeidoAsync(string messageId, CancellationToken ct = default) =>
        EnviarAsync(new
        {
            messaging_product = "whatsapp",
            status = "read",
            message_id = messageId
        }, ct);

    private async Task<bool> EnviarAsync(object cuerpo, CancellationToken ct)
    {
        if (!_opciones.PuedeEnviar)
        {
            _log.LogWarning("WhatsApp: no hay AccessToken o PhoneNumberId configurados; no se envió nada.");
            return false;
        }

        var url = $"{_opciones.ApiVersion}/{_opciones.PhoneNumberId}/messages";

        // Meta a veces rechaza envíos de forma INTERMITENTE (visto el 2026-10-27 con el
        // error 131037 del nombre visible): se reintenta un par de veces antes de rendirse.
        const int intentos = 3;
        for (var intento = 1; intento <= intentos; intento++)
        {
            using var peticion = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(cuerpo)
            };
            peticion.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opciones.AccessToken);

            try
            {
                using var respuesta = await _http.SendAsync(peticion, ct);
                if (respuesta.IsSuccessStatusCode)
                {
                    if (intento > 1)
                        _log.LogInformation("WhatsApp: envío recuperado en el intento {Intento}.", intento);
                    return true;
                }

                var detalle = await respuesta.Content.ReadAsStringAsync(ct);
                _log.LogError("WhatsApp: error {Codigo} al enviar (intento {Intento}/{Total}): {Detalle}",
                    (int)respuesta.StatusCode, intento, intentos, detalle);

                // 401 = token inválido: reintentar no ayuda.
                if ((int)respuesta.StatusCode == 401) return false;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "WhatsApp: falló el envío (intento {Intento}/{Total}).", intento, intentos);
            }

            if (intento < intentos) await Task.Delay(TimeSpan.FromSeconds(2 * intento), ct);
        }

        return false;
    }

    private static string Recortar(string texto, int largo) =>
        texto.Length <= largo ? texto : texto[..largo];
}
