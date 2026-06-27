using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class PlanTrabajoHSE : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Actividad { get; set; } = string.Empty;
    public string TipoIntervencion { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Recursos { get; set; }
    public string? EjecutadoPor { get; set; }
    public DateTime FechaPlanificada { get; set; }
    public DateTime? FechaEjecutada { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    public string? Area { get; set; }
    public EstadoPlanTrabajo Estado { get; set; } = EstadoPlanTrabajo.Planificada;
    public string? Observaciones { get; set; }
}
