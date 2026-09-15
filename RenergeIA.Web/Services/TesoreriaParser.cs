using ClosedXML.Excel;

namespace RenergeIA.Web.Services;

public static class TesoreriaParser
{
    public sealed record MovimientoTesoreria(string Codigo, string Moneda, DateTime FechaCorte, decimal Monto);

    public sealed record ResultadoTesoreria(
        List<MovimientoTesoreria> Movimientos,
        List<string> CodigosEncontrados,
        DateTime? PrimerCorte,
        DateTime? UltimoCorte);

    public static ResultadoTesoreria Parse(XLWorkbook wb)
    {
        var ws = wb.Worksheets.First();
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

        // Columnas de cortes semanales: fechas en la fila 2 (desde la col L en adelante)
        var fechasCol = new Dictionary<int, DateTime>();
        for (int c = 12; c <= lastCol; c++)
        {
            var celda = ws.Cell(2, c);
            DateTime? fecha = null;
            if (celda.DataType == XLDataType.DateTime)
                fecha = celda.GetDateTime();
            else
            {
                var txt = celda.GetString().Trim();
                if (DateTime.TryParse(txt, System.Globalization.CultureInfo.GetCultureInfo("en-US"),
                        System.Globalization.DateTimeStyles.None, out var f1)) fecha = f1;
                else if (DateTime.TryParse(txt, out var f2)) fecha = f2;
            }
            if (fecha.HasValue && fecha.Value.Year is >= 2015 and <= 2100)
                fechasCol[c] = fecha.Value.Date;
        }

        var movimientos = new List<MovimientoTesoreria>();
        var codigos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Secciones: los encabezados reales van en MAYÚSCULAS en la columna B
        string? moneda = null;
        for (int r = 3; r <= lastRow; r++)
        {
            var seccion = ws.Cell(r, 2).GetString().Trim();
            if (seccion == "EGRESOS - COP") { moneda = "COP"; continue; }
            if (seccion == "EGRESOS - USD") { moneda = "USD"; continue; }
            if (seccion.StartsWith("Total Acumulado", StringComparison.OrdinalIgnoreCase)) break;
            if (moneda is null) continue;

            var codigo = ws.Cell(r, 1).GetString().Trim().ToUpperInvariant();
            if (codigo.Length < 2 || codigo.Length > 8) continue;
            if (!codigo.All(ch => char.IsLetter(ch) || char.IsDigit(ch))) continue;

            foreach (var (col, fecha) in fechasCol)
            {
                var valor = BomParser.Num(ws.Cell(r, col));
                if (valor is null || valor.Value == 0) continue;
                // Los egresos vienen en negativo → se registran en positivo (un valor positivo en la
                // fuente es un reintegro y resta)
                movimientos.Add(new MovimientoTesoreria(codigo, moneda, fecha, -valor.Value));
                codigos.Add(codigo);
            }
        }

        var fechasConDatos = movimientos.Select(m => m.FechaCorte).Distinct().OrderBy(f => f).ToList();
        return new ResultadoTesoreria(
            movimientos,
            codigos.OrderBy(c => c).ToList(),
            fechasConDatos.FirstOrDefault() == default ? null : fechasConDatos.First(),
            fechasConDatos.LastOrDefault() == default ? null : fechasConDatos.Last());
    }
}
