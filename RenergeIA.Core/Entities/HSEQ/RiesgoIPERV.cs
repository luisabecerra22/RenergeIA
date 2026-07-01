using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class RiesgoIPERV : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    // Trazabilidad del origen
    public FuenteRiesgo FuenteOrigen { get; set; } = FuenteRiesgo.Manual;
    public int? InspeccionIAId { get; set; }
    public InspeccionIA? InspeccionIA { get; set; }

    // Identificación del peligro
    public string Area { get; set; } = string.Empty;
    public string Actividad { get; set; } = string.Empty;
    public string Tarea { get; set; } = string.Empty;
    public bool EsRutinaria { get; set; } = true;
    public string DescripcionPeligro { get; set; } = string.Empty;
    public string ClasificacionPeligro { get; set; } = string.Empty;
    public string? EfectosPosibles { get; set; }

    // Controles existentes (jerarquía F-M-I)
    public string? ControlFuente { get; set; }
    public string? ControlMedio { get; set; }
    public string? ControlIndividuo { get; set; }

    // Valoración GTC 45
    public int ND { get; set; }   // Nivel Deficiencia:  10=MA, 6=A, 2=M, 0=B
    public int NE { get; set; }   // Nivel Exposición:   4=EC, 3=EF, 2=EO, 1=EEsp
    public int NP { get; set; }   // Nivel Probabilidad: ND × NE
    public int NC { get; set; }   // Nivel Consecuencia: 100=M, 60=MG, 25=G, 10=L
    public int NR { get; set; }   // Nivel Riesgo:       NP × NC
    public string Aceptabilidad { get; set; } = string.Empty;  // I,II,III,IV,V

    // Medidas de intervención (jerarquía de controles)
    public string? Eliminacion { get; set; }
    public string? Sustitucion { get; set; }
    public string? ControlIngenieria { get; set; }
    public string? ControlAdministrativo { get; set; }
    public string? Senalizacion { get; set; }
    public string? EPP { get; set; }

    // Gestión y cierre
    public string Responsable { get; set; } = string.Empty;
    public DateTime? Plazo { get; set; }
    public string? EvidenciaUrl { get; set; }
    public string? Hallazgo { get; set; }
    public string? AccionCorrectiva { get; set; }

    // Estado
    public EstadoRiesgo Estado { get; set; } = EstadoRiesgo.Activo;
    public EstadoValidacionIA EstadoValidacion { get; set; } = EstadoValidacionIA.PendienteValidacion;
}
