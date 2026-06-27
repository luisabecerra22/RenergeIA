using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class CompromisoSocial : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public int? ComunidadId { get; set; }
    public Comunidad? Comunidad { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Beneficiario { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public DateTime FechaCompromiso { get; set; }
    public DateTime? FechaCumplimiento { get; set; }
    public EstadoCompromisoSocial Estado { get; set; } = EstadoCompromisoSocial.Pendiente;
    public string? FuenteCompromiso { get; set; }
    public string? Evidencias { get; set; }
    public string? Observaciones { get; set; }
}
