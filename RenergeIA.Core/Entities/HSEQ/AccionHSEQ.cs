using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class AccionHSEQ : EntidadBase
{
    public int HallazgoHSEQId { get; set; }
    public HallazgoHSEQ HallazgoHSEQ { get; set; } = null!;
    public string Descripcion { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public DateTime FechaCompromiso { get; set; }
    public DateTime? FechaCierre { get; set; }
    public int AvancePorcentaje { get; set; }
    public EstadoNoConformidad Estado { get; set; } = EstadoNoConformidad.Abierta;
    public string? Evidencia { get; set; }
    public string? Observaciones { get; set; }
}
