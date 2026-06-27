namespace RenergeIA.Core.Entities;

public class CostoReal : EntidadBase
{
    public int PartidaId { get; set; }
    public Partida Partida { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string TipoCosto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? NumeroFactura { get; set; }
    public string? Proveedor { get; set; }
    public string RegistradoPor { get; set; } = string.Empty;

    public decimal Monto => Cantidad * PrecioUnitario;
}
