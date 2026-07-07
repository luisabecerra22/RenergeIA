namespace RenergeIA.Core.Entities.HSEQ;

public class PausaActiva : EntidadBase
{
    public int? ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }
    public string Trabajador { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public int Mes { get; set; }
    public int Anio { get; set; }
    public int CantidadPausas { get; set; }
    public string ResponsableRegistro { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}
