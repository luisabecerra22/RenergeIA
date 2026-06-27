using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class PlantillaHistograma : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public TipoHistograma Tipo { get; set; }

    public ICollection<ItemHistograma> Items { get; set; } = [];
}
