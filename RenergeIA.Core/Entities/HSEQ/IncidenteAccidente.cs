using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class IncidenteAccidente : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Numero { get; set; } = string.Empty;
    public TipoIncidente Tipo { get; set; }
    public GravedadIncidente Gravedad { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string FrenteTrabajo { get; set; } = string.Empty;
    public string PersonaInvolucrada { get; set; } = string.Empty;
    public string InvestigadoPor { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public DateTime? FechaInvestigacion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? CausaRaiz { get; set; }
    public string? AccionesInmediatas { get; set; }
    public EstadoNoConformidad Estado { get; set; } = EstadoNoConformidad.Abierta;
    public string? Evidencias { get; set; }
}
