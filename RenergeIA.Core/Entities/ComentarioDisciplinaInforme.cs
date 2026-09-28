namespace RenergeIA.Core.Entities;

/// <summary>
/// Comentario por disciplina / frente (Ingeniería, Suministros, Construcción, Puesta en marcha…)
/// al final de un Informe Diario. Se imprime en el PDF del informe.
/// </summary>
public class ComentarioDisciplinaInforme : EntidadBase
{
    public int InformeDiarioId { get; set; }
    public InformeDiario InformeDiario { get; set; } = null!;

    /// <summary>Clave estable de la sección (ej. "ingenieria", "suministros", "construccion").</summary>
    public string Clave { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int Orden { get; set; }
    public string? Comentario { get; set; }
}
