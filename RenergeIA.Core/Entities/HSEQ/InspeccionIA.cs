using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class InspeccionIA : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    // Datos del formulario de campo
    public string Area { get; set; } = string.Empty;
    public string Actividad { get; set; } = string.Empty;
    public string Tarea { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public string Inspector { get; set; } = string.Empty;
    public DateTime FechaInspeccion { get; set; } = DateTime.Today;
    public string? ObservacionManual { get; set; }
    public string? EvidenciaUrl { get; set; }

    // Control de validación SST
    public EstadoValidacionIA EstadoValidacion { get; set; } = EstadoValidacionIA.PendienteValidacion;
    public string? ValidadoPor { get; set; }
    public DateTime? FechaValidacion { get; set; }
    public string? ObservacionValidacion { get; set; }

    // Resultado de la IA (JSON serializado con la estructura completa)
    public string? ResultadoIA { get; set; }

    // Campos clave desnormalizados para consultas rápidas
    public string? PeligrosIdentificados { get; set; }
    public string? NivelRiesgoSugerido { get; set; }
    public string? HallazgoRedactado { get; set; }
    public string? AccionCorrectivaSugerida { get; set; }

    // Riesgos generados al aprobar esta inspección
    public ICollection<RiesgoIPERV> RiesgosGenerados { get; set; } = [];
}
