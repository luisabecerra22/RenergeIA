using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class GestionResiduos : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public TipoResiduos Tipo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal CantidadKg { get; set; }
    public DateTime Fecha { get; set; }
    public string? GestorAutorizado { get; set; }
    public string? ManifiestoNumero { get; set; }
    public string FrenteTrabajo { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string? Evidencias { get; set; }
}
