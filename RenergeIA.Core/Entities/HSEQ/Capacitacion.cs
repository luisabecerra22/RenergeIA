using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class Capacitacion : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Area { get; set; }
    public DateTime Fecha { get; set; }
    public decimal DuracionHoras { get; set; }
    public int NumeroAsistentes { get; set; }
    public string? Participantes { get; set; }
    public string? Lugar { get; set; }
    public EstadoPlanTrabajo Estado { get; set; } = EstadoPlanTrabajo.Ejecutada;
    public string? Observaciones { get; set; }
    public string? Evidencias { get; set; }
}
