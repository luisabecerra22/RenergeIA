namespace RenergeIA.Core.Entities;

public class FlujoCajaSemanal : EntidadBase
{
    public int InformeConsolidadoId { get; set; }
    public InformeConsolidado InformeConsolidado { get; set; } = null!;

    public DateTime Semana { get; set; }
    public int OrdenSemana { get; set; }
    public string TipoFlujo { get; set; } = "USDCOP";
    public decimal Ingresos { get; set; }
    public decimal Pagos { get; set; }
    public string? Justificacion { get; set; }
}
