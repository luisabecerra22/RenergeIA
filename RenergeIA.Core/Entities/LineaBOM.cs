namespace RenergeIA.Core.Entities;

public class LineaBOM : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Codigo { get; set; } = string.Empty;
    public string Fuente { get; set; } = string.Empty;
    public string? Concepto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Unidad { get; set; }

    public decimal CantidadBOM { get; set; }
    public decimal CostoUnitarioBOM { get; set; }
    public string MonedaCosto { get; set; } = "COP";
    public decimal CostoTotalBOM { get; set; }

    public decimal CantidadReal { get; set; }
    public decimal ValorReal { get; set; }
}
