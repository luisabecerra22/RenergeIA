namespace RenergeIA.Core.Entities;

// Copia del Flujo de Caja semanal tal como quedó con cada archivo de tesorería importado.
// Permite comparar en el Consolidado el corte actual contra el anterior (variación temporal).
// Los montos son los mismos totales por semana que muestra la pantalla de Flujo de Caja.
public class CorteFlujoTesoreria : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public DateTime FechaCorte { get; set; }
    public string? Archivo { get; set; }
    public DateTime Semana { get; set; }
    public decimal PagosCOP { get; set; }
    public decimal PagosUSD { get; set; }
}
