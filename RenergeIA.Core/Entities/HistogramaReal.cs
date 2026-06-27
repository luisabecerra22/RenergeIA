using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class HistogramaReal : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public TipoHistograma Tipo { get; set; }

    public ICollection<ItemHistogramaReal> Items { get; set; } = [];
}
