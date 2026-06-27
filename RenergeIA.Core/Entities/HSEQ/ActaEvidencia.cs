namespace RenergeIA.Core.Entities.HSEQ;

public class ActaEvidencia : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string? ComunidadOParte { get; set; }
    public string? ResponsableElaboracion { get; set; }
    public string? Firmantes { get; set; }
    public string? ArchivoUrl { get; set; }
    public string? Observaciones { get; set; }
}
