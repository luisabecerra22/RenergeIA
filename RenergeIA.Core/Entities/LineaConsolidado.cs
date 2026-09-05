namespace RenergeIA.Core.Entities;

public class LineaConsolidado : EntidadBase
{
    public int InformeConsolidadoId { get; set; }
    public InformeConsolidado InformeConsolidado { get; set; } = null!;

    public string Categoria { get; set; } = string.Empty;
    public string CodigoCategoria { get; set; } = string.Empty;
    public int Orden { get; set; }

    public decimal PresupuestoCOP { get; set; }
    public decimal EjecutadoCOP { get; set; }
    public decimal ComprometidoCOP { get; set; }

    public decimal PresupuestoUSD { get; set; }
    public decimal EjecutadoUSD { get; set; }
    public decimal ComprometidoUSD { get; set; }
}
