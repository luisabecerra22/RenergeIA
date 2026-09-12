using ClosedXML.Excel;
using RenergeIA.Core.Entities;

namespace RenergeIA.Web.Services;

public static class BomParser
{
    public sealed class FichaBom
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Cliente { get; set; }
        public string? Pais { get; set; }
        public string? Alcance { get; set; }
        public decimal? PotenciaMWp { get; set; }
        public decimal? Trm { get; set; }
        public decimal? CostoCOP { get; set; }
        public decimal? PrecioCOP { get; set; }
        public decimal? MargenCOP { get; set; }
        public decimal? MargenPct { get; set; }
        public decimal? CostoUSD { get; set; }
        public decimal? PrecioUSD { get; set; }
        public decimal? MargenUSD { get; set; }
        public decimal? ObrasCivilesPrecio { get; set; }
        public decimal? CostoPlenoPrecio { get; set; }
        public decimal? TotalIvaPrecio { get; set; }
        public decimal? ObrasCivilesCosto { get; set; }
        public decimal? CostoPlenoCosto { get; set; }
        public decimal? TotalIvaCosto { get; set; }
    }

    public static FichaBom? ParseEjecutivo(XLWorkbook wb)
    {
        var ws = wb.Worksheets.FirstOrDefault(w =>
            string.Equals(w.Name.Trim(), "Ejecutivo", StringComparison.OrdinalIgnoreCase));
        if (ws is null) return null;

        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        var lastCol = Math.Min(ws.LastColumnUsed()?.ColumnNumber() ?? 0, 15);
        if (lastRow < 5) return null;

        var ficha = new FichaBom
        {
            Codigo  = TextoDerecha(ws, lastRow, lastCol, "codigo del proyecto"),
            Nombre  = TextoDerecha(ws, lastRow, lastCol, "nombre del proyecto"),
            Cliente = TextoDerecha(ws, lastRow, lastCol, "cliente"),
            Pais    = TextoDerecha(ws, lastRow, lastCol, "pais proyecto"),
            Alcance = TextoDerecha(ws, lastRow, lastCol, "alcance"),
            PotenciaMWp = NumeroDerecha(ws, lastRow, lastCol, "potencia")
        };

        // Primer bloque "Datos Generales" (el vigente; la "OPCIÓN anterior" se ignora)
        int filaDatos = BuscarFila(ws, lastRow, lastCol, h => h == "datos generales");
        if (filaDatos > 0)
        {
            int filaHeader = 0;
            var cols = new Dictionary<string, int>();
            for (int r = filaDatos + 1; r <= Math.Min(filaDatos + 4, lastRow) && filaHeader == 0; r++)
            {
                for (int c = 1; c <= lastCol; c++)
                {
                    if (Norm(ws.Cell(r, c).GetString()) == "descripcion") { filaHeader = r; break; }
                }
            }
            if (filaHeader > 0)
            {
                for (int c = 1; c <= lastCol; c++)
                {
                    var h = Norm(ws.Cell(filaHeader, c).GetString());
                    if (!string.IsNullOrEmpty(h) && !cols.ContainsKey(h)) cols[h] = c;
                }
                int cDesc = cols.GetValueOrDefault("descripcion");
                int cCosto = cols.GetValueOrDefault("costo");
                int cPrecio = cols.GetValueOrDefault("precio");
                int cMargen = cols.GetValueOrDefault("margin");
                int cMargenPct = cols.GetValueOrDefault("margin %");

                for (int r = filaHeader + 1; r <= Math.Min(filaHeader + 10, lastRow); r++)
                {
                    var label = cDesc > 0 ? Norm(ws.Cell(r, cDesc).GetString()) : "";
                    if (label.StartsWith("parque fotovoltaico") && ficha.CostoCOP is null)
                    {
                        ficha.CostoCOP  = Num(ws.Cell(r, cCosto));
                        ficha.PrecioCOP = Num(ws.Cell(r, cPrecio));
                        ficha.MargenCOP = Num(ws.Cell(r, cMargen));
                        ficha.MargenPct = Num(ws.Cell(r, cMargenPct)) is { } pct ? pct * 100 : null;
                    }
                    else if (label.StartsWith("usd") && ficha.CostoUSD is null)
                    {
                        ficha.CostoUSD  = Num(ws.Cell(r, cCosto));
                        ficha.PrecioUSD = Num(ws.Cell(r, cPrecio));
                        ficha.MargenUSD = Num(ws.Cell(r, cMargen));
                    }
                    if (ficha.CostoCOP is not null && ficha.CostoUSD is not null) break;
                }
            }
        }

        // Bloques PRECIO / COSTO con TRM y desglose de obras civiles, costo pleno y total con IVA
        string modo = "";
        for (int r = 1; r <= lastRow; r++)
        {
            var celdas = new List<string>();
            for (int c = 1; c <= lastCol; c++) celdas.Add(Norm(ws.Cell(r, c).GetString()));

            if (celdas.Contains("trm"))
            {
                if (celdas.Contains("precio")) modo = "precio";
                else if (celdas.Contains("costo")) modo = "costo";
                ficha.Trm ??= PrimerNumero(ws, r, lastCol);
                continue;
            }
            if (string.IsNullOrEmpty(modo)) continue;

            var etiqueta = celdas.FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "";
            if (etiqueta.Contains("subtotal obras civiles"))
            {
                var v = PrimerNumero(ws, r, lastCol);
                if (modo == "precio") ficha.ObrasCivilesPrecio ??= v; else ficha.ObrasCivilesCosto ??= v;
            }
            else if (etiqueta.Contains("costo pleno") && !etiqueta.Contains("+"))
            {
                var v = PrimerNumero(ws, r, lastCol);
                if (modo == "precio") ficha.CostoPlenoPrecio ??= v; else ficha.CostoPlenoCosto ??= v;
            }
            else if (etiqueta == "total")
            {
                var v = PrimerNumero(ws, r, lastCol);
                if (modo == "precio") ficha.TotalIvaPrecio ??= v; else ficha.TotalIvaCosto ??= v;
                modo = "";
            }
        }

        return string.IsNullOrEmpty(ficha.Codigo) && ficha.CostoCOP is null ? null : ficha;
    }

    // ==================== LÍNEAS BOM (General, Sum PPAL, Rend MAT) ====================

    private sealed record ConfigHoja(string NombreHoja, string ColDescripcion);

    private static readonly ConfigHoja[] _hojasBOM =
    [
        new("General",  "tipo concepto"),
        new("Sum PPAL", "descripcion"),
        new("Rend MAT", "tipo concepto"),
    ];

    public static (List<LineaBOM> Lineas, List<string> HojasLeidas, List<string> HojasFaltantes)
        ParseLineas(XLWorkbook wb, int proyectoId)
    {
        var lineas = new List<LineaBOM>();
        var leidas = new List<string>();
        var faltantes = new List<string>();

        foreach (var cfg in _hojasBOM)
        {
            var ws = wb.Worksheets.FirstOrDefault(w =>
                string.Equals(w.Name.Trim(), cfg.NombreHoja, StringComparison.OrdinalIgnoreCase));
            if (ws is null) { faltantes.Add(cfg.NombreHoja); continue; }

            var filas = LeerHoja(ws, cfg, proyectoId);
            if (filas.Count > 0) { lineas.AddRange(filas); leidas.Add($"{cfg.NombreHoja} ({filas.Count})"); }
            else faltantes.Add($"{cfg.NombreHoja} (sin filas válidas)");
        }
        return (lineas, leidas, faltantes);
    }

    private static List<LineaBOM> LeerHoja(IXLWorksheet ws, ConfigHoja cfg, int proyectoId)
    {
        var resultado = new List<LineaBOM>();
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
        if (lastRow < 2) return resultado;

        int headerRow = 0;
        for (int r = 1; r <= Math.Min(15, lastRow) && headerRow == 0; r++)
        {
            for (int c = 1; c <= lastCol; c++)
            {
                if (Norm(ws.Cell(r, c).GetString()) == "check") { headerRow = r; break; }
            }
        }
        if (headerRow == 0) return resultado;

        var headers = new Dictionary<int, string>();
        for (int c = 1; c <= lastCol; c++)
        {
            var h = Norm(ws.Cell(headerRow, c).GetString());
            if (!string.IsNullOrEmpty(h)) headers[c] = h;
        }

        int Col(Func<string, bool> pred) => headers.FirstOrDefault(kv => pred(kv.Value)).Key;

        int colCheck    = Col(h => h == "check");
        int colDescr    = Col(h => h == cfg.ColDescripcion || h.StartsWith(cfg.ColDescripcion));
        int colConcepto = Col(h => h == "concepto" || h == "tipo");
        int colUnidad   = Col(h => h == "unidad");
        int colCant     = Col(h => h == "cantidad total");
        if (colCant == 0) colCant = Col(h => h == "cantidad proyecto");
        if (colCant == 0) colCant = Col(h => h == "cantidad");
        int colCU       = Col(h => h == "costo unitario");
        if (colCU == 0)  colCU = Col(h => h.StartsWith("costo unitario") && !h.Contains("proyecto") && !h.Contains("trans"));
        int colMoneda   = Col(h => h == "moneda costo" || h == "moneda inicial");
        int colTotal    = Col(h => h.StartsWith("costo total"));

        if (colCheck == 0 || colDescr == 0 || colTotal == 0) return resultado;

        for (int r = headerRow + 1; r <= lastRow; r++)
        {
            var codigo = ws.Cell(r, colCheck).GetString().Trim();
            var descr = ws.Cell(r, colDescr).GetString().Trim();
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(descr)) continue;

            var cantidad = Num(ws.Cell(r, colCant)) ?? 0;
            var cu       = Num(ws.Cell(r, colCU)) ?? 0;
            var total    = Num(ws.Cell(r, colTotal)) ?? 0;
            if (cantidad == 0 && total == 0) continue;

            var moneda = colMoneda > 0 ? ws.Cell(r, colMoneda).GetString().Trim().ToUpperInvariant() : "COP";
            if (moneda.Length != 3) moneda = "COP";

            resultado.Add(new LineaBOM
            {
                ProyectoId       = proyectoId,
                Codigo           = codigo.ToUpperInvariant(),
                Fuente           = cfg.NombreHoja,
                Concepto         = NullSiVacio(colConcepto > 0 ? ws.Cell(r, colConcepto).GetString() : ""),
                Descripcion      = descr.Length > 300 ? descr[..300] : descr,
                Unidad           = NullSiVacio(colUnidad > 0 ? ws.Cell(r, colUnidad).GetString() : ""),
                CantidadBOM      = cantidad,
                CostoUnitarioBOM = cu,
                MonedaCosto      = moneda,
                CostoTotalBOM    = total
            });
        }
        return resultado;
    }

    // ==================== HELPERS ====================

    private static int BuscarFila(IXLWorksheet ws, int lastRow, int lastCol, Func<string, bool> pred)
    {
        for (int r = 1; r <= lastRow; r++)
            for (int c = 1; c <= lastCol; c++)
                if (pred(Norm(ws.Cell(r, c).GetString()))) return r;
        return 0;
    }

    private static string? TextoDerecha(IXLWorksheet ws, int lastRow, int lastCol, string etiqueta)
    {
        for (int r = 1; r <= lastRow; r++)
        {
            for (int c = 1; c <= lastCol; c++)
            {
                if (Norm(ws.Cell(r, c).GetString()) != etiqueta) continue;
                for (int c2 = c + 1; c2 <= lastCol; c2++)
                {
                    var v = ws.Cell(r, c2).GetString().Trim();
                    if (!string.IsNullOrEmpty(v)) return v;
                }
                return null;
            }
        }
        return null;
    }

    private static decimal? NumeroDerecha(IXLWorksheet ws, int lastRow, int lastCol, string etiqueta)
    {
        for (int r = 1; r <= lastRow; r++)
        {
            for (int c = 1; c <= lastCol; c++)
            {
                if (Norm(ws.Cell(r, c).GetString()) != etiqueta) continue;
                for (int c2 = c + 1; c2 <= lastCol; c2++)
                {
                    var v = Num(ws.Cell(r, c2));
                    if (v is not null && v != 0) return v;
                }
                return null;
            }
        }
        return null;
    }

    private static decimal? PrimerNumero(IXLWorksheet ws, int fila, int lastCol)
    {
        for (int c = 1; c <= lastCol; c++)
        {
            var v = Num(ws.Cell(fila, c));
            if (v is not null && v != 0) return v;
        }
        return null;
    }

    private static string? NullSiVacio(string s)
    {
        s = s.Trim();
        return string.IsNullOrEmpty(s) ? null : s;
    }

    public static string Norm(string s) => s.Trim().ToLowerInvariant()
        .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
        .Replace("\n", " ").Replace("  ", " ");

    private static readonly System.Globalization.CultureInfo _co = new("es-CO");

    public static decimal? Num(IXLCell celda)
    {
        if (celda.IsEmpty()) return null;
        if (celda.DataType == XLDataType.Number) return (decimal)celda.GetDouble();
        var texto = celda.GetString().Trim().Replace("$", "").Replace("%", "").Trim();
        if (string.IsNullOrEmpty(texto) || texto == "-") return null;

        // Texto con coma pero sin punto ("1,7993") → coma decimal (formato es-CO)
        if (texto.Contains(',') && !texto.Contains('.'))
        {
            if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any, _co, out var vEsp)) return vEsp;
        }
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var vInv)) return vInv;
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any, _co, out var vCO)) return vCO;
        return null;
    }
}
