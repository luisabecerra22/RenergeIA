using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

// Acta de reunión (migrada desde Actas IA o creada en la app). El documento histórico
// NO se reescribe: los Word/PDF emitidos viven en ArchivoActaReunion y el acta.json
// original completo se conserva en DatosOrigenJson.
public class ActaReunion : EntidadBase
{
    public string Codigo { get; set; } = string.Empty;          // ej. COS6VL-ACT-SEG-2026-002 (formato FO-SG-GC-021)
    public DateTime? Fecha { get; set; }
    public string? FechaTexto { get; set; }
    public string? ProyectoTexto { get; set; }                  // texto original del acta
    public int? ProyectoActaId { get; set; }
    public ProyectoActa? ProyectoActa { get; set; }
    public string? Area { get; set; }                           // ADM, CON, ING, OYM, PRO, COM, RH, SIG, CAL, AMB, SST, SEG
    public string? Asunto { get; set; }
    public string? Modalidad { get; set; }
    public string? Hora { get; set; }
    public EstadoActaReunion Estado { get; set; } = EstadoActaReunion.Borrador;
    public string? Resumen { get; set; }
    public string? AsistentesJson { get; set; }                 // [{nombre,cargo,empresa,correo}]
    public string? PuntosJson { get; set; }                     // desarrollo / puntos de la reunión
    public string? ActaAnteriorCodigo { get; set; }
    public string? TrazabilidadJson { get; set; }               // registros de "Seguimiento de compromisos anteriores"
    public bool SeguimientoPendienteConfirmacion { get; set; }
    public string? FuenteUrl { get; set; }                      // ej. enlace de Read AI
    public string? DatosOrigenJson { get; set; }                // acta.json completo, tal cual se migró
    public string Origen { get; set; } = "ActasIA";             // ActasIA | RenergeIA
    public int? LoteImportacionId { get; set; }
    public bool Eliminado { get; set; }
    public DateTime? FechaEliminacion { get; set; }

    public ICollection<CompromisoReunion> Compromisos { get; set; } = [];
    public ICollection<ArchivoActaReunion> Archivos { get; set; } = [];
    public ICollection<ComentarioActaReunion> Comentarios { get; set; } = [];
}
