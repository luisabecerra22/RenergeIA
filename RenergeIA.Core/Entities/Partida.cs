namespace RenergeIA.Core.Entities;

public class Partida : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int? ActividadWBSId { get; set; }
    public ActividadWBS? ActividadWBS { get; set; }

    public string? Numero { get; set; }
    public int Nivel { get; set; } = 1;
    public bool EsPrincipal { get; set; }
    public int? PadreId { get; set; }
    public Partida? Padre { get; set; }

    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Unidad { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public decimal CantidadPresupuestada { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal MontoPresupuestado => CantidadPresupuestada * PrecioUnitario;
    public decimal ValorEjecutado { get; set; }

    public ICollection<CostoReal> CostosReales { get; set; } = [];
    public ICollection<Partida> SubPartidas { get; set; } = [];
}
