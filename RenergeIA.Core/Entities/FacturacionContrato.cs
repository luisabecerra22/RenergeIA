namespace RenergeIA.Core.Entities;

// C3 · Avance de facturación. Un proyecto puede tener varios contratos (ej. Suministro y Planta),
// cada uno con su valor de venta, sus porcentajes de anticipo / avance / hitos finales y sus
// ítems de facturación ligados a actividades del cronograma. Cada mes se hace un cierre (corte)
// que genera el acta de avance; al acta se le asocia el número de factura, y esa factura queda
// como ingreso estimado en el flujo de caja del Consolidado.

public enum RegimenImpuestoFacturacion
{
    IvaPleno = 0,
    Aiu = 1,
    Exento = 2
}

public enum EstadoCorteFacturacion
{
    Borrador = 0,
    ActaGenerada = 1,
    Facturada = 2
}

public enum TipoCorteFacturacion
{
    Mensual = 0,
    Anticipo = 1
}

public class ContratoFacturacion : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Nombre { get; set; } = "Contrato";
    public string? NumeroContrato { get; set; }
    public DateTime? FechaContrato { get; set; }
    public string Moneda { get; set; } = "COP";
    public string? Cliente { get; set; }

    /// <summary>Valor de venta del contrato, sin IVA (editable).</summary>
    public decimal ValorVenta { get; set; }

    public decimal PctAnticipo { get; set; } = 5;
    public decimal PctAvanceObra { get; set; } = 80;
    public decimal PctCulminacionMecanica { get; set; } = 5;
    public decimal PctCulminacionSustancial { get; set; } = 5;
    public decimal PctAceptacionProvisional { get; set; } = 10;

    public decimal PctIva { get; set; } = 19;
    public decimal PctAdministracion { get; set; } = 14;
    public decimal PctImprevistos { get; set; } = 1;
    public decimal PctUtilidad { get; set; } = 5;

    public int PlazoPagoDias { get; set; } = 30;
    public int Orden { get; set; }
    public bool Eliminado { get; set; }

    public ICollection<ItemFacturacion> Items { get; set; } = [];
    public ICollection<CorteFacturacion> Cortes { get; set; } = [];
}

public class ItemFacturacion : EntidadBase
{
    public int ContratoFacturacionId { get; set; }
    public ContratoFacturacion Contrato { get; set; } = null!;

    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public RegimenImpuestoFacturacion Regimen { get; set; } = RegimenImpuestoFacturacion.IvaPleno;
    public int Orden { get; set; }

    /// <summary>
    /// Códigos WBS de las actividades del cronograma asociadas, separados por "|". Se guardan por
    /// código (no por Id) para que sobrevivan a las reprogramaciones del cronograma. Un código de
    /// una actividad con hijas incluye todas sus hojas.
    /// </summary>
    public string? ActividadesWbs { get; set; }
}

public class CorteFacturacion : EntidadBase
{
    public int ContratoFacturacionId { get; set; }
    public ContratoFacturacion Contrato { get; set; } = null!;

    public TipoCorteFacturacion Tipo { get; set; } = TipoCorteFacturacion.Mensual;
    /// <summary>Último día del mes del corte (para el anticipo, la fecha del anticipo).</summary>
    public DateTime FechaCorte { get; set; }
    public EstadoCorteFacturacion Estado { get; set; } = EstadoCorteFacturacion.Borrador;

    public int? NumeroActa { get; set; }
    public DateTime? FechaActa { get; set; }
    public string? ActaGeneradaPor { get; set; }

    public bool CulminacionMecanica { get; set; }
    public bool CulminacionSustancial { get; set; }
    public bool AceptacionProvisional { get; set; }

    // Totales congelados al generar el acta (en la moneda del contrato).
    public decimal BaseAvance { get; set; }
    public decimal BaseHitos { get; set; }
    public decimal ValorAiu { get; set; }
    public decimal ValorIva { get; set; }
    public decimal Amortizacion { get; set; }
    public decimal Total { get; set; }

    public string? NumeroFactura { get; set; }
    public DateTime? FechaFactura { get; set; }
    public int? PlazoPagoDias { get; set; }
    public DateTime? FechaPagoEstimada { get; set; }
    public string? FacturaRegistradaPor { get; set; }

    public string? Observaciones { get; set; }

    public ICollection<DetalleCorteFacturacion> Detalles { get; set; } = [];
}

public class DetalleCorteFacturacion : EntidadBase
{
    public int CorteFacturacionId { get; set; }
    public CorteFacturacion Corte { get; set; } = null!;
    public int ItemFacturacionId { get; set; }
    public ItemFacturacion Item { get; set; } = null!;

    public decimal PctPlaneado { get; set; }
    public decimal? PctCronograma { get; set; }
    /// <summary>% real acumulado que se factura en este corte.</summary>
    public decimal PctReal { get; set; }
    public bool AjustadoManual { get; set; }
    public string? AjustadoPor { get; set; }
    public decimal PctAnterior { get; set; }
    public decimal Monto { get; set; }
}
