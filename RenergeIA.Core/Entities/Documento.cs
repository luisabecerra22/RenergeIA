using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class Documento : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public CategoriaDocumento Categoria { get; set; }
    public string? CodigoCliente { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
    public EstadoDocumento Estado { get; set; } = EstadoDocumento.PendienteEmitir;
    public DisciplinaDocumento Disciplina { get; set; } = DisciplinaDocumento.Otro;
    public AreaDocumento Area { get; set; } = AreaDocumento.General;
    public string Version { get; set; } = "0";
    public int Fase { get; set; } = 1;

    public DateTime? FechaEmision { get; set; }

    public DateTime? FechaEntrega1 { get; set; }
    public DateTime? FechaDevolucion1 { get; set; }
    public DateTime? FechaEntrega2 { get; set; }
    public DateTime? FechaDevolucion2 { get; set; }
    public DateTime? FechaEntrega3 { get; set; }
    public DateTime? FechaDevolucion3 { get; set; }
    public DateTime? FechaEntrega4 { get; set; }
    public DateTime? FechaDevolucion4 { get; set; }

    public DateTime? FechaValidacion { get; set; }

    public string? Transmittal { get; set; }
    public string? Observaciones { get; set; }
    public string? Responsable { get; set; }

    /// <summary>Columna "Observación tiempo de retraso" de la planificación documental.</summary>
    public string? ObservacionRetraso { get; set; }

    // Trazabilidad entre la planificación (Excel) y lo editado en la app
    public DateTime? FechaUltimaImportacion { get; set; }
    public DateTime? FechaEdicionApp { get; set; }
    public string? EditadoPor { get; set; }
    public string? ValidadoPor { get; set; }

    // Seguimiento de Redline / As-Built (solo Ingeniería, columnas Z–AI de la planificación)
    public string? ObservacionRedline { get; set; }
    public bool? RegistraCambios { get; set; }
    public string? ResponsableRedline { get; set; }
    public bool? RequiereRedline { get; set; }
    public decimal? AvanceRedline { get; set; }
    public bool? RedlineAprobadoInterventoria { get; set; }
    public decimal? AvanceAsBuilt { get; set; }
    public string? ResponsableAsBuilt { get; set; }
    public bool? AsBuiltAprobadoInterventoria { get; set; }
    public string? ObservacionAsBuilt { get; set; }

    public int? TiempoRetraso1 => CalcularRetraso(FechaEntrega1, FechaDevolucion1);
    public int? TiempoRetraso2 => CalcularRetraso(FechaEntrega2, FechaDevolucion2);
    public int? TiempoRetraso3 => CalcularRetraso(FechaEntrega3, FechaDevolucion3);
    public int? TiempoRetraso4 => CalcularRetraso(FechaEntrega4, FechaDevolucion4);

    private static int? CalcularRetraso(DateTime? entrega, DateTime? devolucion)
    {
        if (!entrega.HasValue) return null;
        var fin = devolucion ?? DateTime.Today;
        return (int)(fin - entrega.Value).TotalDays;
    }

    public ICollection<VersionDocumento> Versiones { get; set; } = [];
}
