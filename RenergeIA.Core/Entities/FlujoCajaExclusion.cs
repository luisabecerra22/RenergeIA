namespace RenergeIA.Core.Entities;

public class FlujoCajaExclusion : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int PartidaId { get; set; }
    public Partida Partida { get; set; } = null!;

    public string Moneda { get; set; } = "COP";
}
