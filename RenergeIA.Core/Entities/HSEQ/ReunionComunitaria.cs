namespace RenergeIA.Core.Entities.HSEQ;

public class ReunionComunitaria : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public int ComunidadId { get; set; }
    public Comunidad Comunidad { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string Objetivo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string Facilitador { get; set; } = string.Empty;
    public int NumeroAsistentes { get; set; }
    public string? TemasTratatados { get; set; }
    public string? Acuerdos { get; set; }
    public string? Observaciones { get; set; }
    public string? Evidencias { get; set; }
    public string? ActaUrl { get; set; }
}
