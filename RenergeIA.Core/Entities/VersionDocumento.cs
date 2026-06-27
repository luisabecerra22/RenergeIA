namespace RenergeIA.Core.Entities;

public class VersionDocumento : EntidadBase
{
    public int DocumentoId { get; set; }
    public Documento Documento { get; set; } = null!;

    public string NumeroVersion { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; }
    public string SubidoPor { get; set; } = string.Empty;
    public long TamanioBytes { get; set; }
    public string? Comentarios { get; set; }
    public bool EsVersionActual { get; set; }
}
