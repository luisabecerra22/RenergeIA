namespace RenergeIA.Core.Entities;

public class CronogramaVersion : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;
    public int NumeroVersion { get; set; }
    public bool EsVigente { get; set; }
    public string? MotivoReprogramacion { get; set; }
    public string? Observaciones { get; set; }
    public string CreadoPor { get; set; } = string.Empty;

    public ICollection<ActividadWBS> Actividades { get; set; } = [];
}
