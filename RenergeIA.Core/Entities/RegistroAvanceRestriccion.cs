namespace RenergeIA.Core.Entities;

public class RegistroAvanceRestriccion
{
    public int RegistroAvanceDiarioId { get; set; }
    public RegistroAvanceDiario RegistroAvanceDiario { get; set; } = null!;

    public int RestriccionId { get; set; }
    public Restriccion Restriccion { get; set; } = null!;
}
