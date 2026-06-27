namespace RenergeIA.Core.Entities;

public class RegistroAvancePersonal
{
    public int RegistroAvanceDiarioId { get; set; }
    public RegistroAvanceDiario RegistroAvanceDiario { get; set; } = null!;

    public int PersonalProyectoId { get; set; }
    public PersonalProyecto PersonalProyecto { get; set; } = null!;

    public decimal HorasTrabajadas { get; set; }
}
