using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class CapacitacionPlanificada : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string? PublicoObjetivo { get; set; }
    public DateTime FechaPlanificada { get; set; }
    public decimal DuracionEstimadaHoras { get; set; }
    public string? Responsable { get; set; }
    public EstadoPlanTrabajo Estado { get; set; } = EstadoPlanTrabajo.Planificada;
    public int? CapacitacionEjecutadaId { get; set; }
    public string? Observaciones { get; set; }
}
