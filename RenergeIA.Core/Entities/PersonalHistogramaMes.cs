namespace RenergeIA.Core.Entities;

// Histograma de personal por cargo y mes calendario (sin tope de 12 meses).
// Tipo "Planificado": importado del BOM (hoja H PER) y/o digitado.
// Tipo "Real": calculado desde la nómina de tesorería (Compromisos → Salarios); la usuaria
// puede ajustar una celda (EsManual) y el ajuste se conserva en las siguientes importaciones.
public class PersonalHistogramaMes : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Tipo { get; set; } = "Planificado";
    public string Cargo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal Cantidad { get; set; }
    // Solo Real: personas distintas según la nómina (lo que se usa si no hay ajuste manual)
    public decimal CantidadNomina { get; set; }
    public bool EsManual { get; set; }
}
