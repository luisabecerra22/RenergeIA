using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace RenergeIA.Web.Services;

// Lee del Forecast Control de tesorería las órdenes de compra con sus hitos y las secciones
// visuales (Salarios, Proyectado). Los pagos de impuestos DIAN se omiten (definido por la usuaria). Columnas: A código, B proveedor, C descripción,
// D número OC / documento, E aprobado (fila OC) o fecha (hito), F subtotal, G IVA, H importe
// (facturado en la fila OC), I retefuente, J ret ICA, K total a pagar (verde = pagado).
public static partial class TesoreriaCompromisosParser
{
    public sealed record Hito(
        string? Periodo, string? Codigo, string Descripcion, string? Detalle, string? Documento, DateTime? Fecha,
        decimal Subtotal, decimal Iva, decimal Importe, decimal RetFuente, decimal RetIca, decimal TotalPagar, bool Pagado);

    public sealed class Bloque
    {
        public string Grupo { get; init; } = "OC";
        public string Moneda { get; init; } = "COP";
        public string Clave { get; init; } = string.Empty;
        public string NumeroOC { get; init; } = string.Empty;
        public string Proveedor { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
        public decimal Aprobado { get; init; }
        public decimal Facturado { get; init; }
        public List<Hito> Hitos { get; } = [];
        public bool EnProceso => NumeroOC.Contains("XXX", StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex(@"^CO_\d{4}_(\d+|X+)$", RegexOptions.IgnoreCase)]
    private static partial Regex RegexOC();

    [GeneratedRegex(@"^[A-Za-zÁÉÍÓÚáéíóú]{3}-\d{4}$")]
    private static partial Regex RegexMes();

    public static List<Bloque> Parse(XLWorkbook wb)
    {
        var ws = wb.Worksheets.First();
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        var bloques = new List<Bloque>();

        string? moneda = null;
        string modo = "";
        string? periodo = null;
        string? ultimoMes = null;
        string? cargoDefecto = null;
        Bloque? oc = null;
        var conteoXXX = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        Bloque Grupo(string grupo, string nombre)
        {
            var clave = $"GRP|{grupo}|{moneda}";
            var b = bloques.FirstOrDefault(x => x.Clave == clave);
            if (b is null)
            {
                b = new Bloque { Grupo = grupo, Moneda = moneda!, Clave = clave, NumeroOC = grupo.ToUpperInvariant(), Proveedor = nombre, Descripcion = nombre };
                bloques.Add(b);
            }
            return b;
        }

        for (int r = 3; r <= lastRow; r++)
        {
            var a = Texto(ws.Cell(r, 1));
            var b = Texto(ws.Cell(r, 2));
            var c = Texto(ws.Cell(r, 3));
            var d = Texto(ws.Cell(r, 4));

            if (b == "EGRESOS - COP") { moneda = "COP"; modo = ""; oc = null; continue; }
            if (b == "EGRESOS - USD") { moneda = "USD"; modo = ""; oc = null; continue; }
            if (b.StartsWith("Total Acumulado", StringComparison.OrdinalIgnoreCase)) break;
            if (moneda is null) continue;

            if (b.StartsWith("SALARIOS / MANO DE OBRA", StringComparison.OrdinalIgnoreCase)) { modo = "Salarios"; oc = null; periodo = null; ultimoMes = null; continue; }
            if (b.StartsWith("OTROS COSTOS", StringComparison.OrdinalIgnoreCase)) { modo = "OC"; oc = null; continue; }
            if (b.StartsWith("DIAN", StringComparison.OrdinalIgnoreCase) && a.Length == 0 && c.Length == 0) { modo = "Impuestos"; oc = null; continue; } // sección DIAN: se ignora
            if (b.StartsWith("PROYECTADO - ", StringComparison.OrdinalIgnoreCase)) { modo = "Proyectado"; oc = null; periodo = null; continue; }

            switch (modo)
            {
                case "OC":
                {
                    if (RegexOC().IsMatch(d))
                    {
                        var aprobado = Num(ws.Cell(r, 5));
                        var facturado = Num(ws.Cell(r, 8));
                        if (b.StartsWith("XXXX", StringComparison.OrdinalIgnoreCase) && aprobado == 0 && facturado == 0)
                        {
                            oc = null; // fila plantilla vacía
                            continue;
                        }
                        var numero = d.ToUpperInvariant();
                        string clave;
                        if (numero.Contains("XXX"))
                        {
                            var baseClave = $"{moneda}|{b}|{c}".ToUpperInvariant();
                            var n = conteoXXX.GetValueOrDefault(baseClave) + 1;
                            conteoXXX[baseClave] = n;
                            clave = $"OCXXX|{baseClave}|{n}";
                        }
                        else clave = $"OC|{numero}";

                        oc = new Bloque { Grupo = "OC", Moneda = moneda, Clave = clave, NumeroOC = numero, Proveedor = b, Descripcion = c, Aprobado = aprobado, Facturado = facturado };
                        bloques.Add(oc);
                        continue;
                    }
                    if (a.Length == 0 && b.Length == 0 && c.Length == 0 && d.Length == 0) { oc = null; continue; }
                    if (oc is not null && c.Length > 0)
                        oc.Hitos.Add(LeerHito(ws, r, null, a, c, null, d));
                    break;
                }
                case "Salarios":
                {
                    if (d.Equals("Cargo", StringComparison.OrdinalIgnoreCase))
                    {
                        if (b.Length > 0) ultimoMes = b;
                        periodo = string.IsNullOrEmpty(ultimoMes) ? c : $"{ultimoMes} · {c}";
                        cargoDefecto = null;
                        continue;
                    }
                    // Bloques sin columna de cargo: planillas de seguridad social y liquidaciones
                    if (a.Length == 0 && b.Equals("Seguridad Social", StringComparison.OrdinalIgnoreCase))
                    {
                        periodo = "Seguridad social"; cargoDefecto = "Planillas seguridad social";
                        continue;
                    }
                    if (a.Length == 0 && b.Equals("Liquidaciones", StringComparison.OrdinalIgnoreCase))
                    {
                        periodo = "Liquidaciones"; cargoDefecto = "Liquidaciones";
                        continue;
                    }
                    var importe = Num(ws.Cell(r, 8));
                    if (c.Length == 0 || importe == 0) continue;
                    // Detalle = cargo (columna D); encabezados o errores en D toman el cargo del bloque
                    var cargo = d.Length == 0 || d.StartsWith('#') || d.Equals("Fecha de Retiro", StringComparison.OrdinalIgnoreCase)
                        ? cargoDefecto ?? "Sin cargo"
                        : d;
                    Grupo("Salarios", "Salarios y mano de obra").Hitos.Add(LeerHito(ws, r, periodo, a, c, cargo, null));
                    break;
                }
                case "Proyectado":
                {
                    if (c.Equals("CONCEPTO", StringComparison.OrdinalIgnoreCase) || b.Equals("PROYECTADO", StringComparison.OrdinalIgnoreCase)) continue;
                    if (a.Length == 0 && RegexMes().IsMatch(b) && c.Length > 0) { periodo = $"{b} · {c}"; continue; }
                    var total = Num(ws.Cell(r, 11));
                    if (c.Length == 0 || total == 0) continue;
                    var fecha = Fecha(ws.Cell(r, 5));
                    Grupo("Proyectado", "Proyectado (sin OC)").Hitos.Add(new Hito(
                        periodo, NullSiVacio(a), c, NullSiVacio(b), NullSiVacio(d), fecha,
                        0, 0, total, 0, 0, total, false));
                    break;
                }
            }
        }
        return bloques;
    }

    private static Hito LeerHito(IXLWorksheet ws, int r, string? periodo, string a, string c, string? detalle, string? documento) =>
        new(periodo, NullSiVacio(a), c, NullSiVacio(detalle), NullSiVacio(documento), Fecha(ws.Cell(r, 5)),
            Num(ws.Cell(r, 6)), Num(ws.Cell(r, 7)), Num(ws.Cell(r, 8)),
            Num(ws.Cell(r, 9)), Num(ws.Cell(r, 10)), Num(ws.Cell(r, 11)),
            EsVerde(ws.Cell(r, 11)));

    // Verde de "pagado" (92D050 / 00B050): el canal G domina claramente; los verdes muy pálidos no cuentan
    private static bool EsVerde(IXLCell celda)
    {
        var fill = celda.Style.Fill;
        if (fill.PatternType == XLFillPatternValues.None) return false;
        foreach (var color in new[] { fill.BackgroundColor, fill.PatternColor })
        {
            if (color is null || color.ColorType != XLColorType.Color) continue;
            var rgb = color.Color;
            if (rgb.G - Math.Max(rgb.R, rgb.B) >= 40) return true;
        }
        return false;
    }

    private static string Texto(IXLCell celda)
    {
        var s = celda.GetFormattedString().Trim();
        return s.Equals("undefined", StringComparison.OrdinalIgnoreCase) ? "" : s;
    }

    private static string? NullSiVacio(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static decimal Num(IXLCell celda) => BomParser.Num(celda) ?? 0m;

    private static DateTime? Fecha(IXLCell celda)
    {
        if (celda.IsEmpty()) return null;
        if (celda.DataType == XLDataType.DateTime) return celda.GetDateTime().Date;
        var t = celda.GetString().Trim();
        return DateTime.TryParse(t, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out var f) ? f.Date : null;
    }
}
