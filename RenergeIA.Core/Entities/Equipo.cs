using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class Equipo : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public TipoEquipo TipoEquipo { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? NumeroSerie { get; set; }
    public string? Propietario { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public DateTime? FechaSalida { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<RegistroHorometro> RegistrosHorometro { get; set; } = [];
    public ICollection<Mantenimiento> Mantenimientos { get; set; } = [];
}
