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

    public string? EtapaPHVA { get; set; }
    public string? FrecuenciaVerificacion { get; set; }
    public int Orden { get; set; }

    public bool EneProg { get; set; }
    public bool EneEjec { get; set; }
    public bool FebProg { get; set; }
    public bool FebEjec { get; set; }
    public bool MarProg { get; set; }
    public bool MarEjec { get; set; }
    public bool AbrProg { get; set; }
    public bool AbrEjec { get; set; }
    public bool MayProg { get; set; }
    public bool MayEjec { get; set; }
    public bool JunProg { get; set; }
    public bool JunEjec { get; set; }
    public bool JulProg { get; set; }
    public bool JulEjec { get; set; }
    public bool AgoProg { get; set; }
    public bool AgoEjec { get; set; }
    public bool SepProg { get; set; }
    public bool SepEjec { get; set; }
    public bool OctProg { get; set; }
    public bool OctEjec { get; set; }
    public bool NovProg { get; set; }
    public bool NovEjec { get; set; }
    public bool DicProg { get; set; }
    public bool DicEjec { get; set; }
}
