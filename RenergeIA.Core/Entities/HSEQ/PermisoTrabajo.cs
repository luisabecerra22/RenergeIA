using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class PermisoTrabajo : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Numero { get; set; } = string.Empty;
    public TipoPermisoTrabajo Tipo { get; set; }
    public string Actividad { get; set; } = string.Empty;
    public string FrenteTrabajo { get; set; } = string.Empty;
    public string ResponsableTrabajo { get; set; } = string.Empty;
    public string EmitidoPor { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public EstadoPermisoTrabajo Estado { get; set; } = EstadoPermisoTrabajo.Activo;
    public string? Observaciones { get; set; }
    public string? MedidasControl { get; set; }
}
