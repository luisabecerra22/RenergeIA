using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class CompromisoCosto : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int? PartidaId { get; set; }
    public Partida? Partida { get; set; }

    public string Codigo { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public EstadoCompromiso Estado { get; set; } = EstadoCompromiso.Pendiente;
    public string Prioridad { get; set; } = "Normal";
    public string? Observaciones { get; set; }
}
