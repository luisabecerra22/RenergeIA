namespace RenergeIA.Core.Entities.HSEQ;

public class RegistroSTC : EntidadBase
{
    public string Inspector { get; set; } = string.Empty;
    public string CorreoInspector { get; set; } = string.Empty;
    public string Inspeccionado { get; set; } = string.Empty;
    public string CorreoInspeccionado { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public int Mes { get; set; }
    public int Anio { get; set; }
    public int CantidadInspecciones { get; set; }
    public string? Observaciones { get; set; }
}
