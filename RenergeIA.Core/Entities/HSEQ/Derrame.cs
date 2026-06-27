using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class Derrame : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Numero { get; set; } = string.Empty;
    public string Sustancia { get; set; } = string.Empty;
    public decimal VolumenLitros { get; set; }
    public string FrenteTrabajo { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public string ReportadoPor { get; set; } = string.Empty;
    public string? CausaRaiz { get; set; }
    public string? AccionesInmediatas { get; set; }
    public EstadoNoConformidad Estado { get; set; } = EstadoNoConformidad.Abierta;
    public DateTime? FechaCierre { get; set; }
    public string? Evidencias { get; set; }
}
