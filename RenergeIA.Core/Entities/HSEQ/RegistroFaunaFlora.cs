namespace RenergeIA.Core.Entities.HSEQ;

public class RegistroFaunaFlora : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string NombreCientifico { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public string RegistradoPor { get; set; } = string.Empty;
    public string? AccionesManejo { get; set; }
    public string? Observaciones { get; set; }
    public string? Evidencias { get; set; }
}
