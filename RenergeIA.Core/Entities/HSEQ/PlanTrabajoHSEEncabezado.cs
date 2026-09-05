namespace RenergeIA.Core.Entities.HSEQ;

public class PlanTrabajoHSEEncabezado : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public int Anio { get; set; }
    public string? ResponsableNombre { get; set; }
    public string? Cargo { get; set; }
    public string? Ubicacion { get; set; }
    public string? ObjetivoGeneral { get; set; }
    public string? IndicadorCumplimiento { get; set; }
    public string? IndicadorEficacia { get; set; }
    public string? IndicadorCobertura { get; set; }
    public DateTime? FechaElaboracion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
