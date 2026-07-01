using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class ChecklistAuditoria : EntidadBase
{
    public int? ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }
    public DivisionHSEQ? Division { get; set; }
    public string NormaISO { get; set; } = string.Empty;
    public TipoNormaHSEQ? TipoNorma { get; set; }
    public TipoAuditoriaHSEQ TipoAuditoria { get; set; } = TipoAuditoriaHSEQ.Interna;
    public string? ProcesoArea { get; set; }
    public string Auditor { get; set; } = string.Empty;
    public DateTime FechaAuditoria { get; set; }
    public decimal PorcentajeCumplimiento { get; set; }
    public string? Observaciones { get; set; }
    public EstadoAuditoria EstadoAuditoria { get; set; } = EstadoAuditoria.Borrador;
    public string? UsuarioId { get; set; }

    public ICollection<ItemChecklist> Items { get; set; } = [];
}

public class ItemChecklist : EntidadBase
{
    public int ChecklistAuditoriaId { get; set; }
    public ChecklistAuditoria ChecklistAuditoria { get; set; } = null!;

    // Identificación del requisito
    public string Clausula { get; set; } = string.Empty;
    public string TituloClausula { get; set; } = string.Empty;
    public string NumeroRequisito { get; set; } = string.Empty;
    public string DescripcionRequisito { get; set; } = string.Empty;

    // Evaluación
    public EstadoCumplimiento Estado { get; set; } = EstadoCumplimiento.SinEvaluar;
    public decimal Puntaje { get; set; }

    // Hallazgos y evidencia
    public string? EvidenciaUrl { get; set; }
    public string? Hallazgo { get; set; }
    public string? OportunidadMejora { get; set; }
    public string? Observaciones { get; set; }

    // Gestión
    public string? Responsable { get; set; }
    public DateTime? Plazo { get; set; }
    public EstadoSeguimiento Seguimiento { get; set; } = EstadoSeguimiento.Pendiente;
}
