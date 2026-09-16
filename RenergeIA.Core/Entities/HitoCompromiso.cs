namespace RenergeIA.Core.Entities;

// Hito o pago de una orden de compra de tesorería (factura F#, cuenta de cobro CC#, póliza POL#),
// o una línea de las secciones Salarios / Impuestos / Proyectado
public class HitoCompromiso : EntidadBase
{
    public int CompromisoCostoId { get; set; }
    public CompromisoCosto CompromisoCosto { get; set; } = null!;

    public int Orden { get; set; }
    public string? Periodo { get; set; }
    public string? Codigo { get; set; }
    public bool CodigoManual { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public string? Documento { get; set; }
    public DateTime? Fecha { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Importe { get; set; }
    public decimal RetFuente { get; set; }
    public decimal RetIca { get; set; }
    public decimal TotalPagar { get; set; }
    public bool Pagado { get; set; }
}
