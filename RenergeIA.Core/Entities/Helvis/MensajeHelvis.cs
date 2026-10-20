namespace RenergeIA.Core.Entities.Helvis;

// Historial persistente de la conversación con Helvis: todo mensaje entrante y saliente
// queda guardado. El WaMessageId único también sirve de deduplicación persistente
// (Meta reintenta entregas; antes el dedupe vivía solo en memoria del proceso).
public class MensajeHelvis : EntidadBase
{
    public int? ContactoHelvisId { get; set; }
    public ContactoHelvis? Contacto { get; set; }

    public string Telefono { get; set; } = string.Empty;    // solo dígitos, aunque no esté registrado
    public bool Entrante { get; set; }                      // true = lo escribió la persona; false = lo envió Helvis
    public string? WaMessageId { get; set; }                // id de Meta (único si existe)

    public string? Texto { get; set; }
    public string Tipo { get; set; } = "text";              // text | button | image | document | ...
    public string? OpcionId { get; set; }                   // id del botón pulsado, si aplica

    // Para salientes: resultado del envío (enviado / entregado / leido / fallo) y detalle del error.
    public string? Estado { get; set; }
    public string? DetalleError { get; set; }
}
