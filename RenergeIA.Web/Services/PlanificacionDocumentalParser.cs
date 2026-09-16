using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using RenergeIA.Core.Enums;

namespace RenergeIA.Web.Services;

// Lector de la Planificación Documental (FO-SI-GC-002-1): hojas Construcción → Procedimientos, HSE e Ingeniería.
// Encabezados en la fila que contiene "Código Cliente" (fila 14 en el formato), datos debajo.
public static partial class PlanificacionDocumentalParser
{
    public sealed class FilaPlan
    {
        public CategoriaDocumento Categoria { get; set; }
        public string Hoja { get; set; } = "";
        public int Fila { get; set; }

        public string? CodigoCliente { get; set; }
        public string? Codigo { get; set; }
        public string? Titulo { get; set; }
        public EstadoDocumento? Estado { get; set; }
        public string? Version { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaEntrega1 { get; set; }
        public DateTime? FechaDevolucion1 { get; set; }
        public DateTime? FechaEntrega2 { get; set; }
        public DateTime? FechaDevolucion2 { get; set; }
        public DateTime? FechaEntrega3 { get; set; }
        public DateTime? FechaDevolucion3 { get; set; }
        public DateTime? FechaEntrega4 { get; set; }
        public DateTime? FechaDevolucion4 { get; set; }
        public DateTime? FechaValidacion { get; set; }
        public int? Fase { get; set; }
        public AreaDocumento? Area { get; set; }
        public string? Transmittal { get; set; }
        public string? Observaciones { get; set; }
        public string? Responsable { get; set; }
        public string? ObservacionRetraso { get; set; }

        public string? ObservacionRedline { get; set; }
        public bool? RegistraCambios { get; set; }
        public string? ResponsableRedline { get; set; }
        public bool? RequiereRedline { get; set; }
        public decimal? AvanceRedline { get; set; }
        public bool? RedlineAprobadoInterventoria { get; set; }
        public decimal? AvanceAsBuilt { get; set; }
        public string? ResponsableAsBuilt { get; set; }
        public bool? AsBuiltAprobadoInterventoria { get; set; }
        public string? ObservacionAsBuilt { get; set; }

        public List<string> Advertencias { get; } = [];

        public string Etiqueta => !string.IsNullOrEmpty(Codigo) ? Codigo
            : !string.IsNullOrEmpty(CodigoCliente) ? CodigoCliente
            : Titulo ?? $"fila {Fila}";
    }

    public sealed class Resultado
    {
        public string? Proyecto { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public List<FilaPlan> Filas { get; } = [];
        public List<string> Advertencias { get; } = [];
        public Dictionary<CategoriaDocumento, string> Hojas { get; } = [];
    }

    private static readonly (CategoriaDocumento Cat, string[] Nombres)[] HojasEsperadas =
    [
        (CategoriaDocumento.Procedimientos, ["construccion", "procedimientos"]),
        (CategoriaDocumento.HSE, ["hse"]),
        (CategoriaDocumento.Ingenieria, ["ingenieria"])
    ];

    // Valores de la columna "Responsable" de Ingeniería que indican en cancha de quién está (fórmula), no una persona
    private static readonly HashSet<string> ValoresCancha = ["renergeia", "cliente", "ok para construccion", "0", "false"];

    public static Resultado Leer(Stream stream)
    {
        using var wb = new XLWorkbook(stream);
        var res = new Resultado();

        foreach (var (cat, nombres) in HojasEsperadas)
        {
            var ws = wb.Worksheets.FirstOrDefault(w => nombres.Contains(Normalizar(w.Name)));
            if (ws is null)
            {
                res.Advertencias.Add($"No se encontró la hoja de {NombreCategoria(cat)} ({string.Join(" / ", nombres)}).");
                continue;
            }
            res.Hojas[cat] = ws.Name;
            LeerHoja(ws, cat, res);
        }

        if (res.Hojas.Count == 0)
            res.Advertencias.Add("El archivo no tiene las hojas Construcción, HSE o Ingeniería de la planificación documental.");
        return res;
    }

    private static void LeerHoja(IXLWorksheet ws, CategoriaDocumento cat, Resultado res)
    {
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        var lastCol = Math.Min(ws.LastColumnUsed()?.ColumnNumber() ?? 0, 60);

        // Encabezado del formato: Proyecto / Fecha de actualización (valor a la derecha de la etiqueta)
        for (int r = 1; r <= Math.Min(4, lastRow); r++)
        {
            for (int c = 1; c <= Math.Min(lastCol, 6); c++)
            {
                var etiqueta = Normalizar(ws.Cell(r, c).GetString());
                if (etiqueta.StartsWith("proyecto"))
                {
                    var v = ValorDerecha(ws, r, c, lastCol);
                    if (!string.IsNullOrWhiteSpace(v?.GetString())) res.Proyecto ??= v!.GetString().Trim();
                }
                else if (etiqueta.StartsWith("fecha de actualizacion"))
                {
                    var v = ValorDerecha(ws, r, c, lastCol);
                    var f = v is null ? null : LeerFecha(v, out _);
                    if (f.HasValue && (res.FechaActualizacion is null || f > res.FechaActualizacion))
                        res.FechaActualizacion = f;
                }
            }
        }

        int filaEnc = 0;
        for (int r = 1; r <= Math.Min(40, lastRow) && filaEnc == 0; r++)
            for (int c = 1; c <= lastCol; c++)
                if (Normalizar(ws.Cell(r, c).GetString()) == "codigo cliente") { filaEnc = r; break; }

        if (filaEnc == 0)
        {
            res.Advertencias.Add($"Hoja {ws.Name}: no se encontró la fila de encabezados (Código Cliente).");
            return;
        }

        var cols = MapearColumnas(ws, filaEnc, lastCol);
        if (!cols.ContainsKey("titulo"))
        {
            res.Advertencias.Add($"Hoja {ws.Name}: falta la columna \"Nombre del documento\".");
            return;
        }

        for (int r = filaEnc + 1; r <= lastRow; r++)
        {
            var fila = new FilaPlan { Categoria = cat, Hoja = ws.Name, Fila = r };
            fila.CodigoCliente = Texto(ws, r, cols, "codigocliente");
            if (fila.CodigoCliente is not null && Normalizar(fila.CodigoCliente) is "na" or "n/a" or "-") fila.CodigoCliente = null;
            fila.Codigo = Texto(ws, r, cols, "codigo");
            fila.Titulo = Texto(ws, r, cols, "titulo");

            // Filas del formato sin diligenciar (solo traen el consecutivo #)
            if (fila.Codigo is null && fila.CodigoCliente is null && fila.Titulo is null) continue;

            // Pie de firmas del formato ("Elaborado por / Revisado por / Aprobado por"): fin de la tabla
            if (Enumerable.Range(1, Math.Min(lastCol, 12)).Select(c => Normalizar(ws.Cell(r, c).GetString()))
                    .Any(t => t.StartsWith("elaborado por") || t.StartsWith("revisado por") || t.StartsWith("aprobado por")))
                break;

            var estadoTxt = Texto(ws, r, cols, "estado");
            fila.Estado = ParsearEstado(estadoTxt);
            if (estadoTxt is not null && fila.Estado is null)
                fila.Advertencias.Add($"Estado \"{estadoTxt}\" no reconocido; se deja Pendiente Emitir.");
            fila.Estado ??= EstadoDocumento.PendienteEmitir;

            fila.Version = Texto(ws, r, cols, "version");
            fila.FechaEmision = Fecha(ws, r, cols, "fechaemision", fila);
            fila.FechaEntrega1 = Fecha(ws, r, cols, "entrega1", fila);
            fila.FechaDevolucion1 = Fecha(ws, r, cols, "devolucion1", fila);
            fila.FechaEntrega2 = Fecha(ws, r, cols, "entrega2", fila);
            fila.FechaDevolucion2 = Fecha(ws, r, cols, "devolucion2", fila);
            fila.FechaEntrega3 = Fecha(ws, r, cols, "entrega3", fila);
            fila.FechaDevolucion3 = Fecha(ws, r, cols, "devolucion3", fila);
            fila.FechaEntrega4 = Fecha(ws, r, cols, "entrega4", fila);
            fila.FechaDevolucion4 = Fecha(ws, r, cols, "devolucion4", fila);
            fila.FechaValidacion = Fecha(ws, r, cols, "fechavalidacion", fila);

            var faseTxt = Texto(ws, r, cols, "fase");
            if (faseTxt is not null)
            {
                var m = RegexDigito().Match(faseTxt);
                if (m.Success && int.Parse(m.Value) is >= 1 and <= 3) fila.Fase = int.Parse(m.Value);
                else fila.Advertencias.Add($"Fase \"{faseTxt}\" no reconocida.");
            }

            if (cols.TryGetValue("area", out var colArea) && !ws.Cell(r, colArea).IsEmpty())
            {
                var celda = ws.Cell(r, colArea);
                fila.Area = celda.DataType is XLDataType.DateTime or XLDataType.Number ? null : ParsearArea(celda.GetString());
                if (fila.Area is null) fila.Advertencias.Add($"Área \"{celda.GetFormattedString()}\" no reconocida; se conserva la del documento.");
            }

                    // La columna "Observaciones" de la planificación ES el transmittal (HSEEXT-129, COS5SO-GY-019)
            fila.Transmittal = Texto(ws, r, cols, "observaciones");

            var resp = Texto(ws, r, cols, "responsable");
            if (resp is not null && !ValoresCancha.Contains(Normalizar(resp))) fila.Responsable = resp;

            fila.ObservacionRetraso = Texto(ws, r, cols, "obsretraso");
            fila.ObservacionRedline = Texto(ws, r, cols, "obsredline");
            fila.RegistraCambios = SiNo(Texto(ws, r, cols, "registracambios"));
            fila.ResponsableRedline = Texto(ws, r, cols, "respredline");
            fila.RequiereRedline = SiNo(Texto(ws, r, cols, "requiereredline"));
            fila.AvanceRedline = Porcentaje(ws, r, cols, "avanceredline");
            fila.RedlineAprobadoInterventoria = SiNo(Texto(ws, r, cols, "aprobadoredline"));
            fila.AvanceAsBuilt = Porcentaje(ws, r, cols, "avanceasbuilt");
            fila.ResponsableAsBuilt = Texto(ws, r, cols, "respasbuilt");
            fila.AsBuiltAprobadoInterventoria = SiNo(Texto(ws, r, cols, "aprobadoasbuilt"));
            fila.ObservacionAsBuilt = Texto(ws, r, cols, "obsasbuilt");

            res.Filas.Add(fila);
        }
    }

    private static Dictionary<string, int> MapearColumnas(IXLWorksheet ws, int filaEnc, int lastCol)
    {
        var map = new Dictionary<string, int>();
        bool zonaRedline = false, zonaAsBuilt = false;

        for (int c = 1; c <= lastCol; c++)
        {
            var h = Normalizar(ws.Cell(filaEnc, c).GetString());
            if (h.Length == 0) continue;

            string? clave = null;
            if (h.Contains("tiempo de retraso") && !h.Contains("observacion")) clave = null;
            else if (h.Contains("observacion") && h.Contains("retraso")) clave = "obsretraso";
            else if (h.Contains("observacion") && h.Contains("redline")) { clave = "obsredline"; zonaRedline = true; }
            else if (h.Contains("codigo") && h.Contains("cliente")) clave = "codigocliente";
            else if (h.Contains("codigo")) clave = "codigo";
            else if (h.Contains("nombre")) clave = "titulo";
            else if (h == "estado") clave = "estado";
            else if (h.StartsWith("version")) clave = "version";
            else if (h.Contains("emision")) clave = "fechaemision";
            else if (h.Contains("entrega") || h.Contains("devolucion"))
            {
                var n = RegexDigito().Match(h);
                clave = (h.Contains("entrega") ? "entrega" : "devolucion") + (n.Success ? n.Value : "1");
            }
            else if (h.StartsWith("fecha") && h.Contains("validacion")) clave = "fechavalidacion";
            else if (h == "fase") clave = "fase";
            else if (h == "area" || h.Contains("especialidad")) clave = "area";
            else if (h.Contains("registra cambios")) { clave = "registracambios"; zonaRedline = true; }
            else if (h.Contains("requiere redline")) { clave = "requiereredline"; zonaRedline = true; }
            else if (h.Contains("avance") && h.Contains("redline")) { clave = "avanceredline"; zonaRedline = true; }
            else if (h.Contains("interventoria") && h.Contains("as-built")) { clave = "aprobadoasbuilt"; zonaAsBuilt = true; }
            else if (h.Contains("interventoria")) { clave = "aprobadoredline"; zonaRedline = true; }
            else if (h.Contains("as-built") || h.Contains("as built")) { clave = "avanceasbuilt"; zonaAsBuilt = true; }
            else if (h.StartsWith("responsable"))
                clave = zonaAsBuilt ? "respasbuilt" : zonaRedline ? "respredline" : "responsable";
            else if (h.StartsWith("observacion"))
                clave = zonaAsBuilt ? "obsasbuilt" : "observaciones";

            if (clave is not null) map.TryAdd(clave, c);
        }
        return map;
    }

    // ── Lectura de celdas ───────────────────────────────────────────────────

    private static IXLCell? ValorDerecha(IXLWorksheet ws, int r, int c, int lastCol)
    {
        for (int k = c + 1; k <= Math.Min(lastCol, c + 8); k++)
            if (!ws.Cell(r, k).IsEmpty()) return ws.Cell(r, k);
        return null;
    }

    private static string? Texto(IXLWorksheet ws, int r, Dictionary<string, int> cols, string clave)
    {
        if (!cols.TryGetValue(clave, out var c)) return null;
        var cell = ws.Cell(r, c);
        if (cell.IsEmpty()) return null;
        string s;
        try { s = cell.GetFormattedString(); } catch { s = cell.GetString(); }
        s = s.Trim();
        return s.Length == 0 || s.StartsWith('#') ? null : s;
    }

    private static DateTime? Fecha(IXLWorksheet ws, int r, Dictionary<string, int> cols, string clave, FilaPlan fila)
    {
        if (!cols.TryGetValue(clave, out var c)) return null;
        var cell = ws.Cell(r, c);
        if (cell.IsEmpty()) return null;
        var f = LeerFecha(cell, out var nota);
        if (nota is not null) fila.Advertencias.Add($"Columna {cell.Address.ColumnLetter}: {nota}");
        return f;
    }

    private static DateTime? LeerFecha(IXLCell cell, out string? nota)
    {
        nota = null;
        try
        {
            if (cell.DataType == XLDataType.DateTime) return cell.GetDateTime().Date;
            if (cell.DataType == XLDataType.Number)
            {
                var n = cell.GetDouble();
                if (n is > 30000 and < 70000) return DateTime.FromOADate(n).Date;
                nota = $"valor \"{n}\" no es una fecha";
                return null;
            }
        }
        catch { }

        var txt = cell.GetString().Trim();
        if (txt.Length == 0) return null;

        var fechas = new List<DateTime>();
        var limite = DateTime.UtcNow.AddHours(-5).Date.AddDays(1);
        foreach (Match m in RegexFechaTexto().Matches(txt))
        {
            int a = int.Parse(m.Groups[1].Value), b = int.Parse(m.Groups[2].Value);
            int y = int.Parse(m.Groups[3].Success ? m.Groups[3].Value : m.Groups[4].Value);
            if (y < 100) y += 2000;
            if (y is < 2000 or > 2100) continue;
            DateTime? ddmm = b is >= 1 and <= 12 && a >= 1 && a <= DateTime.DaysInMonth(y, b) ? new DateTime(y, b, a) : null;
            DateTime? mmdd = a is >= 1 and <= 12 && b >= 1 && b <= DateTime.DaysInMonth(y, a) ? new DateTime(y, a, b) : null;
            // En el archivo conviven dd/MM y MM/dd (texto pegado): preferir dd/MM salvo que quede en el futuro
            var elegida = ddmm is not null && mmdd is not null && ddmm > limite && mmdd <= limite ? mmdd : ddmm ?? mmdd;
            if (elegida is not null) fechas.Add(elegida.Value);
        }
        if (fechas.Count > 0 && RegexFechaTexto().Matches(txt).Any(x => !x.Groups[3].Success)) nota = $"fecha mal escrita \"{txt}\"; se interpretó como {fechas.Max():dd/MM/yyyy}";
        if (fechas.Count == 0)
        {
            if (DateTime.TryParse(txt, new CultureInfo("es-CO"), DateTimeStyles.None, out var d)) return d.Date;
            nota = $"\"{txt}\" no es una fecha válida";
            return null;
        }
        if (fechas.Count > 1) nota = $"la celda trae {fechas.Count} fechas (\"{txt}\"); se tomó la más reciente";
        return fechas.Max();
    }

    private static decimal? Porcentaje(IXLWorksheet ws, int r, Dictionary<string, int> cols, string clave)
    {
        if (!cols.TryGetValue(clave, out var c)) return null;
        var cell = ws.Cell(r, c);
        if (cell.IsEmpty()) return null;
        try
        {
            if (cell.DataType == XLDataType.Number)
            {
                var v = (decimal)cell.GetDouble();
                return Math.Round(v <= 1 ? v * 100 : v, 1);
            }
        }
        catch { }
        var t = cell.GetString().Replace("%", "").Trim();
        return decimal.TryParse(t, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ? (p <= 1 ? p * 100 : p) : null;
    }

    private static bool? SiNo(string? t) => t is null ? null : Normalizar(t) switch
    {
        "si" or "sí" or "x" or "yes" => true,
        "no" => false,
        _ => null
    };

    public static EstadoDocumento? ParsearEstado(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return null;
        var t = Normalizar(texto);
        if (t.Contains("no validado") || t.Contains("rechazado")) return EstadoDocumento.NoValidado;
        if (t.Contains("validado") && t.Contains("comentario")) return EstadoDocumento.ValidadoConComentarios;
        if (t.Contains("pendiente") && t.Contains("valid")) return EstadoDocumento.PendienteValidacion;
        if (t.Contains("pendiente") || t.Contains("emitir")) return EstadoDocumento.PendienteEmitir;
        if (t.Contains("validado") || t.Contains("aprobado")) return EstadoDocumento.Validado;
        if (t.Contains("informativo")) return EstadoDocumento.Informativos;
        return null;
    }

    public static AreaDocumento? ParsearArea(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return null;
        var t = Normalizar(texto);
        if (t.Contains("civil")) return AreaDocumento.Civil;
        if (t.StartsWith("mec")) return AreaDocumento.Mecanico;
        if (t.StartsWith("elec")) return AreaDocumento.Electrico;
        if (t.StartsWith("general")) return AreaDocumento.General;
        if (t.StartsWith("calidad")) return AreaDocumento.Calidad;
        if (t.StartsWith("ambiental")) return AreaDocumento.Ambiental;
        if (t.StartsWith("seguridad") || t == "sst") return AreaDocumento.Seguridad;
        if (t.StartsWith("comunica")) return AreaDocumento.Comunicaciones;
        return null;
    }

    public static string Normalizar(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        var d = s.Trim().ToLowerInvariant().Replace('\n', ' ').Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(d.Length);
        foreach (var ch in d)
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark && ch != '¿' && ch != '?' && ch != ':')
                sb.Append(ch);
        return RegexEspacios().Replace(sb.ToString(), " ").Trim();
    }

    private static string NombreCategoria(CategoriaDocumento c) => c switch
    {
        CategoriaDocumento.Procedimientos => "Procedimientos",
        CategoriaDocumento.HSE => "HSE",
        _ => "Ingeniería"
    };

    [GeneratedRegex(@"\d")] private static partial Regex RegexDigito();
    [GeneratedRegex(@"\s+")] private static partial Regex RegexEspacios();
    // dd/MM/aaaa, d-M-aa y la variante mal escrita sin separador antes del año ("03/042026")
    [GeneratedRegex(@"(\d{1,2})[/\-](\d{1,2})(?:[/\-](\d{4}|\d{2})\b|(\d{4})\b)")] private static partial Regex RegexFechaTexto();
}
