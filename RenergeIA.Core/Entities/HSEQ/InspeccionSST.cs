using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class InspeccionSST : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Seguridad"; // Seguridad | OTS | STC
    public string Area { get; set; } = string.Empty;
    public string Frente { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public string Inspector { get; set; } = string.Empty;
    public DateTime FechaInspeccion { get; set; }
    public EstadoInspeccionHSEQ Estado { get; set; } = EstadoInspeccionHSEQ.Programada;
    public int HallazgosEncontrados { get; set; }
    public int HallazgosCerrados { get; set; }
    public int ActosInseguros { get; set; }
    public int CondicionesInseguras { get; set; }
    public string? AccionesGeneradas { get; set; }
    public string? ResponsableCierre { get; set; }
    public DateTime? FechaCompromiso { get; set; }
    public string? Observaciones { get; set; }
    public string? Evidencias { get; set; }
}
