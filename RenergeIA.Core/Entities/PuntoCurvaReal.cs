namespace RenergeIA.Core.Entities;

/// <summary>
/// Punto del histórico de avance REAL acumulado de un proyecto (Curva S), cargado desde el
/// seguimiento externo (informe interno en Excel / Project) para las fechas anteriores a los
/// informes diarios de la app. Un punto por fecha y alcance (todo el proyecto / solo construcción).
/// </summary>
public class PuntoCurvaReal : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public decimal PorcentajeReal { get; set; }

    /// <summary>true = curva de "Avance de construcción" (Civil/Mecánica/Eléctrica); false = todo el proyecto.</summary>
    public bool SoloConstruccion { get; set; }

    public string? Origen { get; set; }
}
