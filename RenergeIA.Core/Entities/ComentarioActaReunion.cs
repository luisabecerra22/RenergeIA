namespace RenergeIA.Core.Entities;

public class ComentarioActaReunion : EntidadBase
{
    public int ActaReunionId { get; set; }
    public ActaReunion ActaReunion { get; set; } = null!;
    public string Texto { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime Creado { get; set; }
    public bool Atendido { get; set; }
    public string? AtendidoPor { get; set; }
    public DateTime? AtendidoEl { get; set; }
    public string? IdOrigen { get; set; }
}
