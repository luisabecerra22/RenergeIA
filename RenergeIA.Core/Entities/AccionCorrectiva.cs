namespace RenergeIA.Core.Entities;

public class AccionCorrectiva : EntidadBase
{
    public int NoConformidadId { get; set; }
    public NoConformidad NoConformidad { get; set; } = null!;

    public string Descripcion { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public DateTime FechaCompromiso { get; set; }
    public DateTime? FechaImplementacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}
