namespace RenergeIA.Core.Entities;

public class DocumentoPersona : EntidadBase
{
    public int PersonalProyectoId { get; set; }
    public PersonalProyecto PersonalProyecto { get; set; } = null!;

    public string TipoDocumento { get; set; } = string.Empty;
    public string NombreDocumento { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime? FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    public bool EsVigente => FechaVencimiento == null || FechaVencimiento > DateTime.UtcNow;
}
