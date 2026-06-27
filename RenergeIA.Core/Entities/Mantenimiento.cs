using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class Mantenimiento : EntidadBase
{
    public int EquipoId { get; set; }
    public Equipo Equipo { get; set; } = null!;

    public TipoMantenimiento TipoMantenimiento { get; set; }
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal? Costo { get; set; }
    public string? RealizadoPor { get; set; }
    public DateTime? ProximoMantenimiento { get; set; }
}
