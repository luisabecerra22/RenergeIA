namespace RenergeIA.Core.Entities.HSEQ;

public class AspectoImpacto : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Actividad { get; set; } = string.Empty;
    public string Aspecto { get; set; } = string.Empty;
    public string Impacto { get; set; } = string.Empty;
    public string MedioAfectado { get; set; } = string.Empty;
    public int Magnitud { get; set; }
    public int Frecuencia { get; set; }
    public int Duracion { get; set; }
    public int NivelSignificancia { get; set; }
    public bool EsSignificativo { get; set; }
    public string? MedidasControl { get; set; }
    public string Responsable { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}
