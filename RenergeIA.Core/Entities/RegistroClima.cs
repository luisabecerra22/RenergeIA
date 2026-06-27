using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class RegistroClima : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public CondicionClimatica Condicion { get; set; }
    public decimal? TemperaturaMaxima { get; set; }
    public decimal? TemperaturaMinima { get; set; }
    public decimal? HumedadRelativa { get; set; }
    public decimal? VelocidadViento { get; set; }
    public decimal? PrecipitacionMm { get; set; }
    public decimal? HorasDisponiblesTrabajar { get; set; }
    public bool AfectoActividades { get; set; }
    public string? Observaciones { get; set; }

    public int? InformeDiarioId { get; set; }
    public InformeDiario? InformeDiario { get; set; }
}
