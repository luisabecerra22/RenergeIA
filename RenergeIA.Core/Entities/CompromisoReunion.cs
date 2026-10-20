using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

// Compromiso originado en un acta. Conserva su identidad desde la reunión de origen
// hasta su cierre: las continuidades entre reuniones NO crean otro compromiso
// (ContinuidadDe) y toda gestión queda como ActualizacionCompromisoReunion.
public class CompromisoReunion : EntidadBase
{
    public int ActaReunionId { get; set; }
    public ActaReunion ActaReunion { get; set; } = null!;
    public string IdOrigen { get; set; } = string.Empty;        // id del sistema anterior: <codigoActa>-NN
    public int Orden { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;     // original; compartidos separados con " / "
    public string? ResponsablePrincipal { get; set; }           // opcional, sin borrar la asignación original
    public string? Empresa { get; set; }
    public string? FechaTexto { get; set; }                     // fecha descriptiva original ("Por definir", etc.)
    public DateTime? FechaLimite { get; set; }                  // solo si el texto es una fecha real
    public EstadoCompromisoReunion Estado { get; set; } = EstadoCompromisoReunion.Pendiente;
    public TipoCompromisoReunion Tipo { get; set; } = TipoCompromisoReunion.Propio;
    public string? Categoria { get; set; }                      // columna del tablero (área temática)
    public string? NotaCierre { get; set; }
    public string? CerradoPor { get; set; }
    public DateTime? CerradoEl { get; set; }
    public bool RequiereConfirmacion { get; set; }              // "Por aclarar"
    public string? ObservacionSeguimiento { get; set; }
    public int? ContinuidadDeId { get; set; }                   // compromiso del que es continuidad
    public CompromisoReunion? ContinuidadDe { get; set; }
    public string? ContinuidadDeOrigen { get; set; }            // id antiguo del compromiso continuado
    public string? ProyectoTexto { get; set; }
    public string? Area { get; set; }
    public DateTime? FechaActa { get; set; }
    public string? OrigenOriginalJson { get; set; }             // snapshot original (inmutable) del sistema anterior
    public string? HistorialJson { get; set; }                  // historial importado tal cual
    public DateTime? CreadoOrigen { get; set; }
    public string? EditadoPor { get; set; }
    public DateTime? EditadoEl { get; set; }
    public int? LoteImportacionId { get; set; }
    public bool Eliminado { get; set; }
    public DateTime? FechaEliminacion { get; set; }

    public ICollection<ActualizacionCompromisoReunion> Actualizaciones { get; set; } = [];
}
