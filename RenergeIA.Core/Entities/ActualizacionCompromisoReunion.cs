namespace RenergeIA.Core.Entities;

// Gestión sobre un compromiso (avance, cambio de fecha acordado, cierre propuesto/confirmado,
// respuesta por WhatsApp, etc.). Es un registro de solo agregar: nunca reescribe el acta emitida.
public class ActualizacionCompromisoReunion : EntidadBase
{
    public int CompromisoReunionId { get; set; }
    public CompromisoReunion CompromisoReunion { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public string Autor { get; set; } = string.Empty;
    public string? TipoActualizacion { get; set; }   // avance | cierre_propuesto | cierre_confirmado | reapertura | cambio_fecha | nota
    public string? EstadoAnterior { get; set; }
    public string? EstadoNuevo { get; set; }
    public string? Observacion { get; set; }
    public string? ActaReferencia { get; set; }      // acta en la que se habló
    public string? CambiosJson { get; set; }
    public string Origen { get; set; } = "Web";      // Web | WhatsApp | Migracion
}
