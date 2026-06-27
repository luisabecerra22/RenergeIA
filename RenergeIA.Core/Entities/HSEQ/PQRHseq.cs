using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class PQRHseq : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public int? ComunidadId { get; set; }
    public Comunidad? Comunidad { get; set; }
    public string Numero { get; set; } = string.Empty;
    public TipoPQR Tipo { get; set; }
    public string Solicitante { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaRecepcion { get; set; }
    public DateTime FechaLimiteRespuesta { get; set; }
    public DateTime? FechaCierre { get; set; }
    public EstadoPQR Estado { get; set; } = EstadoPQR.Abierta;
    public string? Responsable { get; set; }
    public string? Respuesta { get; set; }
    public string? Evidencias { get; set; }
}
