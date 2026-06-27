namespace RenergeIA.Core.Entities;

public class Fotografia : EntidadBase
{
    public int InformeDiarioId { get; set; }
    public InformeDiario InformeDiario { get; set; } = null!;

    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaToma { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public string? Etiquetas { get; set; }
}
