namespace RenergeIA.Core.Entities;

public class RegistroHorometro : EntidadBase
{
    public int EquipoId { get; set; }
    public Equipo Equipo { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public decimal LecturaHorometro { get; set; }
    public decimal HorasTrabajadas { get; set; }
    public string? Operador { get; set; }
    public string? Observaciones { get; set; }
}
