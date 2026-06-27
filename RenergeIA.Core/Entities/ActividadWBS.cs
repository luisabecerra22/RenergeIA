using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class ActividadWBS : EntidadBase
{
    public Disciplina? Disciplina { get; set; }
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int? ActividadPadreId { get; set; }
    public ActividadWBS? ActividadPadre { get; set; }

    public string CodigoWBS { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int NivelWBS { get; set; }
    public string? Responsable { get; set; }
    public DateTime FechaInicioPlaneada { get; set; }
    public DateTime FechaFinPlaneada { get; set; }
    public DateTime? FechaInicioReal { get; set; }
    public DateTime? FechaFinReal { get; set; }
    public decimal AvancePlanificado { get; set; }
    public decimal AvanceReal { get; set; }
    public EstadoActividad Estado { get; set; } = EstadoActividad.Pendiente;

    public bool Activo { get; set; } = true;

    public int? CronogramaVersionId { get; set; }
    public CronogramaVersion? CronogramaVersion { get; set; }
    public decimal CantidadTotal { get; set; }
    public string? Unidad { get; set; }
    public decimal CantidadEjecutadaAcumulada { get; set; }
    public bool EsCritica { get; set; }
    public string? FrenteTrabajo { get; set; }

    public ICollection<ActividadWBS> SubActividades { get; set; } = [];
    public ICollection<RegistroAvanceDiario> RegistrosAvance { get; set; } = [];
}
