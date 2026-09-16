using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

/// <summary>
/// Responsable interno de la documentación de un área en un proyecto
/// (ej. Mecánico → Coordinador mecánico – Eduard Rodriguez). Recibe las alertas
/// de los documentos de su área que no tengan un responsable propio.
/// </summary>
public class ResponsableAreaDocumento : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public AreaDocumento Area { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
}
