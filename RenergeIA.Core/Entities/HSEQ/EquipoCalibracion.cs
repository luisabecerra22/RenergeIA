using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class EquipoCalibracion : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Serial { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string? NumeroCertificado { get; set; }
    public DateTime? FechaCalibracion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string Responsable { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public string? Observaciones { get; set; }

    public EstadoCalibracion Estado =>
        FechaVencimiento is null ? EstadoCalibracion.SinCertificado :
        FechaVencimiento < DateTime.Today ? EstadoCalibracion.Vencido :
        FechaVencimiento < DateTime.Today.AddDays(30) ? EstadoCalibracion.ProximoAVencer :
        EstadoCalibracion.Vigente;
}
