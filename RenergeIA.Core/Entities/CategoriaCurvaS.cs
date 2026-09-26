namespace RenergeIA.Core.Entities;

/// <summary>
/// Categoría de la Curva S de un proyecto (ej. Suministro principal 25 %, Ingeniería 10 %,
/// Construcción 50 %, EPC Línea y pruebas 15 %). La curva total es la suma ponderada de la
/// curva de cada categoría; dentro de la categoría las actividades hoja pesan por duración.
/// </summary>
public class CategoriaCurvaS : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int Orden { get; set; }
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Peso en porcentaje (0-100) dentro de la curva total.</summary>
    public decimal Peso { get; set; }

    /// <summary>Códigos WBS (o prefijos) incluidos, separados por coma: "2.6, 2.7". Una actividad
    /// pertenece si su código es igual o empieza por "código." de alguno.</summary>
    public string Incluye { get; set; } = string.Empty;

    /// <summary>Códigos WBS excluidos de la categoría, misma regla: "2.8.7, 2.8.9".</summary>
    public string? Excluye { get; set; }

    /// <summary>true = forma parte del alcance "Solo construcción" de la Curva S.</summary>
    public bool EsConstruccion { get; set; }
}
