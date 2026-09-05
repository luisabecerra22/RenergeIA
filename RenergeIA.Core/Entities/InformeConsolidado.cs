using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class InformeConsolidado : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int NumeroInforme { get; set; }
    public int Version { get; set; } = 1;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    public decimal TRM { get; set; }
    public decimal TRMBomInicial { get; set; }

    public EstadoInforme Estado { get; set; } = EstadoInforme.Borrador;
    public string CreadoPor { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
    public string? JustificacionVenta { get; set; }
    public string? JustificacionCosto { get; set; }
    public string? AnalisisIA { get; set; }
    public bool MostrarAnalisisEnPrint { get; set; }

    public decimal VentaContractualCOP { get; set; }
    public decimal VentaContractualUSD { get; set; }

    public decimal PresupuestoCOP { get; set; }
    public decimal EjecutadoCOP { get; set; }
    public decimal ComprometidoCOP { get; set; }

    public decimal PresupuestoUSD { get; set; }
    public decimal EjecutadoUSD { get; set; }
    public decimal ComprometidoUSD { get; set; }

    public decimal ImprevistosCOP { get; set; }
    public decimal ImprevistosUSD { get; set; }
    public decimal TotalPOsCOP { get; set; }
    public decimal TotalPOsUSD { get; set; }

    public DateTime? FechaInicioFlujo { get; set; }
    public DateTime? FechaFinFlujo { get; set; }

    public int? ConsolidadoAnteriorId { get; set; }
    public InformeConsolidado? ConsolidadoAnterior { get; set; }

    public bool Eliminado { get; set; }

    public ICollection<LineaConsolidado> Lineas { get; set; } = [];
    public ICollection<FlujoCajaSemanal> FlujosCaja { get; set; } = [];
}
