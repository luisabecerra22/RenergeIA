using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class Restriccion : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Numero { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public EstadoRestriccion Estado { get; set; } = EstadoRestriccion.Abierta;
    public DateTime FechaIdentificacion { get; set; }
    public DateTime? FechaCompromiso { get; set; }
    public DateTime? FechaLevantamiento { get; set; }
    public string? Responsable { get; set; }
    public string? Impacto { get; set; }
    public string? Plan { get; set; }
}
