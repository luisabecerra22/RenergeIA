using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class AnalisisTrabajoSeguro : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Actividad { get; set; } = string.Empty;
    public string FrenteTrabajo { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int NumeroTrabajadores { get; set; }
    public string? RiesgosIdentificados { get; set; }
    public string? MedidasControl { get; set; }
    public string? EPPRequerido { get; set; }
    public string? Observaciones { get; set; }
    public string? Firmas { get; set; }
}
