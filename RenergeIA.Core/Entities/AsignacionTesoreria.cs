namespace RenergeIA.Core.Entities;

// Memoria de asignaciones manuales hechas por la usuaria sobre datos importados de tesorería,
// para reaplicarlas en cada nueva importación.
// Tipo "NumeroOC": Clave = clave de la OC sin consecutivo (XXX), Valor = número de OC asignado.
// Tipo "CodigoHito": Clave = clave del hito, Valor = código de rubro asignado.
public class AsignacionTesoreria : EntidadBase
{
    public int ProyectoId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}
