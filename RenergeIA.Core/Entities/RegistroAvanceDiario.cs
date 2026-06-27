using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class RegistroAvanceDiario : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int ActividadWBSId { get; set; }
    public ActividadWBS ActividadWBS { get; set; } = null!;

    public int InformeDiarioId { get; set; }
    public InformeDiario InformeDiario { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public decimal PorcentajeAvance { get; set; }
    public decimal? HorasTrabajadas { get; set; }
    public int? PersonalEnSitio { get; set; }
    public string? Observaciones { get; set; }
    public string ReportadoPor { get; set; } = string.Empty;

    public decimal CantidadEjecutadaDia { get; set; }
    public decimal AvanceEsperado { get; set; }
    public decimal AvanceAcumulado { get; set; }
    public decimal Desviacion { get; set; }
    public int DiasAtraso { get; set; }
    public EstadoAvance Estado { get; set; } = EstadoAvance.EnTiempo;
    public decimal? HorasAfectadasClima { get; set; }
    public string? Novedades { get; set; }

    public ICollection<RegistroAvancePersonal> PersonalAsignado { get; set; } = [];
    public ICollection<RegistroAvanceEquipo> EquiposUtilizados { get; set; } = [];
    public ICollection<RegistroAvanceRestriccion> RestriccionesRelacionadas { get; set; } = [];
}
