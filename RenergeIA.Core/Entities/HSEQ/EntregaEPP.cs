namespace RenergeIA.Core.Entities.HSEQ;

public class EntregaEPP : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Trabajador { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string TipoEntrega { get; set; } = "EPP"; // EPP | Dotación
    public string ItemEPP { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Talla { get; set; }
    public int Cantidad { get; set; }
    public DateTime FechaEntrega { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string EntregadoPor { get; set; } = string.Empty;
    public bool FirmaConformidad { get; set; }
    public string? Observaciones { get; set; }
}
