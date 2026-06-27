using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class ChecklistAuditoria : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public DivisionHSEQ Division { get; set; }
    public string NormaISO { get; set; } = string.Empty;
    public string Auditor { get; set; } = string.Empty;
    public DateTime FechaAuditoria { get; set; }
    public decimal PorcentajeCumplimiento { get; set; }
    public string? Observaciones { get; set; }

    public ICollection<ItemChecklist> Items { get; set; } = [];
}

public class ItemChecklist : EntidadBase
{
    public int ChecklistAuditoriaId { get; set; }
    public ChecklistAuditoria ChecklistAuditoria { get; set; } = null!;
    public string NumeroRequisito { get; set; } = string.Empty;
    public string DescripcionRequisito { get; set; } = string.Empty;
    public EstadoCumplimiento Estado { get; set; }
    public string? Observaciones { get; set; }
    public string? Evidencia { get; set; }
    public string? Responsable { get; set; }
}
