namespace RenergeIA.Core.Entities;

public class PagoCorteSemanal : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int PartidaId { get; set; }
    public Partida Partida { get; set; } = null!;

    public DateTime FechaCorte { get; set; }
    public decimal Monto { get; set; }
    public string? Descripcion { get; set; }
    public string? NumeroFactura { get; set; }
    public string? Proveedor { get; set; }
}
