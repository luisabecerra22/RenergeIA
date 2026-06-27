using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class InspeccionAmbiental : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string FrenteTrabajo { get; set; } = string.Empty;
    public string Inspector { get; set; } = string.Empty;
    public DateTime FechaInspeccion { get; set; }
    public EstadoInspeccionHSEQ Estado { get; set; } = EstadoInspeccionHSEQ.Programada;
    public int HallazgosEncontrados { get; set; }
    public int HallazgosCerrados { get; set; }
    public string? Observaciones { get; set; }
    public string? Evidencias { get; set; }
}
