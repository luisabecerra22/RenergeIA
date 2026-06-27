namespace RenergeIA.Core.Entities;

public class ItemHistograma : EntidadBase
{
    public int PlantillaHistogramaId { get; set; }
    public PlantillaHistograma PlantillaHistograma { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;

    public decimal Mes1  { get; set; }
    public decimal Mes2  { get; set; }
    public decimal Mes3  { get; set; }
    public decimal Mes4  { get; set; }
    public decimal Mes5  { get; set; }
    public decimal Mes6  { get; set; }
    public decimal Mes7  { get; set; }
    public decimal Mes8  { get; set; }
    public decimal Mes9  { get; set; }
    public decimal Mes10 { get; set; }
    public decimal Mes11 { get; set; }
    public decimal Mes12 { get; set; }
}
