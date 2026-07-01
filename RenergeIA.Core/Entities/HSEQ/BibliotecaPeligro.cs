namespace RenergeIA.Core.Entities.HSEQ;

public class BibliotecaPeligro : EntidadBase
{
    // Identificación
    public string Area { get; set; } = string.Empty;
    public string Actividad { get; set; } = string.Empty;
    public string Tarea { get; set; } = string.Empty;
    public string Peligro { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public string? EfectosPosibles { get; set; }

    // Controles recomendados (jerarquía F-M-I)
    public string? ControlFuente { get; set; }
    public string? ControlMedio { get; set; }
    public string? ControlIndividuo { get; set; }
    public string? MedidasIntervencion { get; set; }

    // EPP y documentación
    public string? EPPRecomendado { get; set; }
    public string? DocumentosAsociados { get; set; }
    public string? PermisosRequeridos { get; set; }

    // Valoración sugerida
    public string NivelRiesgoSugerido { get; set; } = "Medio";

    public bool Activo { get; set; } = true;
}
