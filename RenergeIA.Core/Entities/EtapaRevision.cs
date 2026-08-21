using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class EtapaRevision : EntidadBase
{
    public int RecursoEquipoId { get; set; }
    public RecursoEquipo RecursoEquipo { get; set; } = null!;

    public EtapaProceso Etapa { get; set; }
    public EstadoEtapa Estado { get; set; } = EstadoEtapa.Pendiente;

    public string? ResponsableNombre { get; set; }
    public string? ResponsableEmail { get; set; }

    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaRecepcion { get; set; }
    public DateTime? FechaComentarios { get; set; }
    public string? DetalleComentarios { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? Observaciones { get; set; }
}
