using RenergeIA.Core.Entities;

namespace RenergeIA.Core.Helpers;

/// <summary>
/// Cálculo puro de un cierre del C3 (avance de facturación), igual a la hoja de Control de
/// Facturación: cada ítem factura valor × % por avance de obra × (avance acumulado − lo ya
/// facturado); los hitos finales facturan su % sobre el valor de venta; la obra civil (AIU) suma
/// administración, imprevistos y utilidad, con IVA solo sobre la utilidad; el resto lleva IVA pleno
/// salvo los ítems exentos; el anticipo se amortiza en el mismo % sobre lo facturado.
/// </summary>
public static class CalculoFacturacion
{
    public record LineaEntrada(int ItemId, decimal Valor, RegimenImpuestoFacturacion Regimen, decimal PctAnterior, decimal PctReal);

    public record LineaResultado(int ItemId, decimal PctAnterior, decimal PctReal, decimal Delta, decimal Monto);

    public record Resultado(
        List<LineaResultado> Lineas,
        decimal BaseAvance, decimal BaseHitos,
        decimal BasePleno, decimal BaseAiu, decimal BaseExento,
        decimal Administracion, decimal Imprevistos, decimal Utilidad,
        decimal Iva, decimal Amortizacion, decimal Total)
    {
        public decimal Aiu => Administracion + Imprevistos + Utilidad;
        public decimal Subtotal => BaseAvance + BaseHitos + Aiu;
    }

    public static Resultado Calcular(ContratoFacturacion c, IEnumerable<LineaEntrada> lineas,
        bool culminacionMecanica, bool culminacionSustancial, bool aceptacionProvisional)
    {
        var pctAvance = c.PctAvanceObra / 100m;
        decimal pleno = 0, aiu = 0, exento = 0;
        var res = new List<LineaResultado>();
        foreach (var l in lineas)
        {
            var real = Math.Clamp(l.PctReal, 0, 100);
            var delta = Math.Max(0, real - l.PctAnterior);
            var monto = Math.Round(l.Valor * pctAvance * delta / 100m, 2);
            switch (l.Regimen)
            {
                case RegimenImpuestoFacturacion.Aiu: aiu += monto; break;
                case RegimenImpuestoFacturacion.Exento: exento += monto; break;
                default: pleno += monto; break;
            }
            res.Add(new LineaResultado(l.ItemId, l.PctAnterior, real, delta, monto));
        }

        var hitos = 0m;
        if (culminacionMecanica) hitos += c.ValorVenta * c.PctCulminacionMecanica / 100m;
        if (culminacionSustancial) hitos += c.ValorVenta * c.PctCulminacionSustancial / 100m;
        if (aceptacionProvisional) hitos += c.ValorVenta * c.PctAceptacionProvisional / 100m;
        hitos = Math.Round(hitos, 2);

        var baseAvance = pleno + aiu + exento;
        var adm = Math.Round(aiu * c.PctAdministracion / 100m, 2);
        var imp = Math.Round(aiu * c.PctImprevistos / 100m, 2);
        var uti = Math.Round(aiu * c.PctUtilidad / 100m, 2);
        var iva = Math.Round((pleno + hitos) * c.PctIva / 100m + uti * c.PctIva / 100m, 2);
        var amort = Math.Round((baseAvance + hitos) * c.PctAnticipo / 100m, 2);
        var total = baseAvance + hitos + adm + imp + uti + iva - amort;

        return new Resultado(res, baseAvance, hitos, pleno + hitos, aiu, exento, adm, imp, uti, iva, amort, total);
    }

    /// <summary>Valor del anticipo (sobre el valor de venta, sin IVA).</summary>
    public static decimal ValorAnticipo(ContratoFacturacion c) => Math.Round(c.ValorVenta * c.PctAnticipo / 100m, 2);

    /// <summary>Último día del mes de una fecha.</summary>
    public static DateTime FinDeMes(DateTime f) => new DateTime(f.Year, f.Month, DateTime.DaysInMonth(f.Year, f.Month));

    public static List<string> CodigosWbs(string? texto) =>
        (texto ?? "").Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct().ToList();

    public static string? UnirCodigos(IEnumerable<string> codigos)
    {
        var l = codigos.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct().ToList();
        return l.Count == 0 ? null : string.Join("|", l);
    }
}
