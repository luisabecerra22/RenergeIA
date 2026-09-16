using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class Proyecto : EntidadBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public string? Municipio { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? AccuWeatherLocationKey { get; set; }
    public decimal CapacidadKWp { get; set; }
    public decimal PresupuestoContractual { get; set; }
    public decimal TasaCambioCOPUSD { get; set; } = 4000m;
    public DateTime FechaInicioPlaneada { get; set; }
    public DateTime FechaFinPlaneada { get; set; }
    public DateTime? FechaInicioReal { get; set; }
    public DateTime? FechaFinReal { get; set; }
    public EstadoProyecto Estado { get; set; } = EstadoProyecto.Planificacion;
    public string? Descripcion { get; set; }
    public int MesInicialHistograma  { get; set; } = 1;
    public int AnioInicialHistograma { get; set; } = 2025;
    public DateTime? FechaInicioPagos { get; set; }
    public DateTime? FechaFinPagos { get; set; }
    public DateTime? FechaCorteTesoreria { get; set; }
    public string? ArchivoTesoreria { get; set; }
    public DateTime? FechaActualizacionPlanDocumental { get; set; }
    public string? ArchivoPlanDocumental { get; set; }

    public string? BomAlcance { get; set; }
    public decimal? BomTrm { get; set; }
    public decimal? BomCostoCOP { get; set; }
    public decimal? BomPrecioCOP { get; set; }
    public decimal? BomMargenCOP { get; set; }
    public decimal? BomMargenPct { get; set; }
    public decimal? BomCostoUSD { get; set; }
    public decimal? BomPrecioUSD { get; set; }
    public decimal? BomMargenUSD { get; set; }
    public decimal? BomObrasCivilesPrecioCOP { get; set; }
    public decimal? BomCostoPlenoPrecioCOP { get; set; }
    public decimal? BomTotalIvaPrecioCOP { get; set; }
    public decimal? BomObrasCivilesCostoCOP { get; set; }
    public decimal? BomCostoPlenoCostoCOP { get; set; }
    public decimal? BomTotalIvaCostoCOP { get; set; }

    public bool Eliminado { get; set; }
    public DateTime? FechaEliminacion { get; set; }

    public ICollection<ActividadWBS> Actividades { get; set; } = [];
    public ICollection<InformeDiario> InformesDiarios { get; set; } = [];
    public ICollection<Documento> Documentos { get; set; } = [];
    public ICollection<Partida> Partidas { get; set; } = [];
    public ICollection<NoConformidad> NoConformidades { get; set; } = [];
    public ICollection<Restriccion> Restricciones { get; set; } = [];
    public ICollection<Alerta> Alertas { get; set; } = [];
    public ICollection<RegistroClima> RegistrosClima { get; set; } = [];
    public ICollection<RegistroAvanceDiario> RegistrosAvance { get; set; } = [];
    public ICollection<Proveedor> Proveedores { get; set; } = [];
    public ICollection<PersonaExterna> PersonasExternas { get; set; } = [];
    public ICollection<RecursoEquipo> RecursosEquipo { get; set; } = [];
    public ICollection<DocumentoControl> DocumentosControl { get; set; } = [];
    public ICollection<InformeConsolidado> InformesConsolidados { get; set; } = [];
    public ICollection<PagoCorteSemanal> PagosCorteSemanal { get; set; } = [];
}
