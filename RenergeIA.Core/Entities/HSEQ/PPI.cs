using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class PPI : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ActividadWBS { get; set; }
    public string? FrenteTrabajo { get; set; }
    public string Disciplina { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public DateTime FechaPlaneada { get; set; }
    public DateTime? FechaRealizacion { get; set; }
    public EstadoPPI Estado { get; set; } = EstadoPPI.Pendiente;
    public string? Observaciones { get; set; }
    public string? Evidencias { get; set; }
}
