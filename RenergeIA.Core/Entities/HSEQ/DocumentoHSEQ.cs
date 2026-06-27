using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class DocumentoHSEQ : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public TipoDocumentoHSEQ Tipo { get; set; }
    public string Version { get; set; } = "1.0";
    public EstadoDocumentoHSEQ Estado { get; set; } = EstadoDocumentoHSEQ.Vigente;
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string Responsable { get; set; } = string.Empty;
    public string? AprobadoPor { get; set; }
    public string? UbicacionSharePoint { get; set; }
    public string? Disciplina { get; set; }
    public string? Observaciones { get; set; }
}
