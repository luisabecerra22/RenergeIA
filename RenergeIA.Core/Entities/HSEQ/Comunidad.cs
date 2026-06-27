namespace RenergeIA.Core.Entities.HSEQ;

public class Comunidad : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string? Resguardo { get; set; }
    public string? Corregimiento { get; set; }
    public string? LiderComunal { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public int? NumeroPobladores { get; set; }
    public string? Observaciones { get; set; }

    public ICollection<ReunionComunitaria> Reuniones { get; set; } = [];
    public ICollection<PQRHseq> PQRs { get; set; } = [];
}
