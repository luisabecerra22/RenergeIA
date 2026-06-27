namespace RenergeIA.Core.Entities;

public class RegistroAvanceEquipo
{
    public int RegistroAvanceDiarioId { get; set; }
    public RegistroAvanceDiario RegistroAvanceDiario { get; set; } = null!;

    public int EquipoId { get; set; }
    public Equipo Equipo { get; set; } = null!;

    public decimal HorasUtilizadas { get; set; }
}
