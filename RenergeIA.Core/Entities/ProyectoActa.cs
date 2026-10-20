namespace RenergeIA.Core.Entities;

// Catálogo de proyectos de las actas (incluye alias y códigos anteriores para reconocer
// actas históricas). Se relaciona con el Proyecto de RenergeIA sin duplicarlo; la relación
// es opcional porque hay actas corporativas y proyectos que no existen en RenergeIA.
public class ProyectoActa : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }                 // ej. COS6ML
    public string? AreasJson { get; set; }
    public string? AliasJson { get; set; }
    public string? CodigosAnterioresJson { get; set; }
    public int? ProyectoId { get; set; }                // proyecto de RenergeIA (opcional, se asigna manualmente)
    public Proyecto? Proyecto { get; set; }
    public string? IdOrigen { get; set; }
    public int? LoteImportacionId { get; set; }
}
