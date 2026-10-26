using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using RenergeIA.Core.Enums;
using static RenergeIA.Web.Services.Actas.ActasZipParser;

namespace RenergeIA.Web.Services.Actas;

// Lee actas en Word (formato FO-SG-GC-021 que genera Actas IA) para cargarlas una a una,
// sin el paquete ZIP. Secciones reconocidas por su título numerado en mayúsculas:
//   1. INFORMACIÓN GENERAL (tabla etiqueta | valor: Acta No., Fecha, Proyecto, Área, Asunto, Modalidad, Hora)
//   2. ASISTENTES (N° | Nombre | Cargo | Empresa)
//   3. DESARROLLO DE LA REUNIÓN (interna) / RESUMEN DE LA REUNIÓN (externa)
//   4. SEGUIMIENTO DE COMPROMISOS ANTERIORES (Origen | Compromiso y responsable | Estado | Fecha | Observación)
//   4/5. COMPROMISOS [NUEVOS DE ESTA REUNIÓN] (N° | Compromiso | Responsable | Empresa | Fecha límite)
//   FIRMAS (se ignora)
// Los compromisos SOLO se toman del acta INTERNA: la externa trae un subconjunto renumerado y
// el id <codigo>-NN es posicional sobre la lista completa (la misma regla del sistema anterior).
public static partial class ActasWordParser
{
    public sealed record ArchivoWord(string Nombre, byte[] Contenido);

    public sealed class WordLeido
    {
        public string Archivo = "";
        public bool EsInterna;
        public ActaLeida Acta = new();
        public List<string> Advertencias = [];
    }

    // Lee varios Word y los agrupa por código de acta (interna + externa de la misma reunión
    // se combinan en una sola acta). Los archivos ilegibles se devuelven en "errores".
    public static (PaqueteActas Paquete, List<WordLeido> Leidos, List<string> Errores) LeerVarios(IEnumerable<ArchivoWord> archivos)
    {
        var leidos = new List<WordLeido>();
        var errores = new List<string>();
        foreach (var f in archivos)
        {
            try { leidos.Add(Leer(f.Contenido, f.Nombre)); }
            catch (Exception ex) { errores.Add($"{f.Nombre}: {ex.Message}"); }
        }

        var p = new PaqueteActas { DesdeWord = true };
        foreach (var g in leidos.GroupBy(l => l.Acta.Codigo))
        {
            var interna = g.FirstOrDefault(x => x.EsInterna);
            var externa = g.FirstOrDefault(x => !x.EsInterna);
            if (g.Count(x => x.EsInterna) > 1 || g.Count(x => !x.EsInterna) > 1)
                p.Advertencias.Add($"{g.Key}: se subió más de un Word {(g.Count(x => x.EsInterna) > 1 ? "interno" : "externo")}; se tomó el primero.");

            var baseActa = (interna ?? externa)!.Acta;
            var a = new ActaLeida
            {
                Codigo = baseActa.Codigo, Carpeta = string.Join(" + ", g.Select(x => x.Archivo)),
                Fecha = baseActa.Fecha, FechaTexto = baseActa.FechaTexto, ProyectoTexto = baseActa.ProyectoTexto,
                Area = baseActa.Area, Asunto = baseActa.Asunto, Modalidad = baseActa.Modalidad, Hora = baseActa.Hora,
                AsistentesJson = baseActa.AsistentesJson,
                PuntosJson = interna?.Acta.PuntosJson,
                Resumen = externa?.Acta.Resumen ?? interna?.Acta.Resumen,
                TrazabilidadJson = interna?.Acta.TrazabilidadJson ?? externa?.Acta.TrazabilidadJson,
                SeguimientoAnterior = interna?.Acta.SeguimientoAnterior ?? externa?.Acta.SeguimientoAnterior ?? [],
                Compromisos = interna?.Acta.Compromisos ?? [],
                TieneInterna = interna is not null,
                EstadoTexto = "borrador",
            };
            if (interna is not null) a.Archivos.Add(interna.Acta.Archivos[0]);
            if (externa is not null) a.Archivos.Add(externa.Acta.Archivos[0]);
            a.Json = JsonOrigen(a);
            p.Actas.Add(a);
            foreach (var l in g) p.Advertencias.AddRange(l.Advertencias.Select(x => $"{l.Archivo}: {x}"));
            if (interna is null)
                p.Advertencias.Add($"{a.Codigo}: solo se subió el acta EXTERNA — se guarda el documento, pero los compromisos se cargan cuando subas la INTERNA (la externa no trae la lista completa).");
        }
        p.Actas = p.Actas.OrderBy(a => a.Fecha ?? DateTime.MaxValue).ThenBy(a => a.Codigo).ToList();
        return (p, leidos, errores);
    }

    public static WordLeido Leer(byte[] contenido, string nombreArchivo)
    {
        var w = new WordLeido { Archivo = nombreArchivo };
        using var ms = new MemoryStream(contenido);
        WordprocessingDocument doc;
        try { doc = WordprocessingDocument.Open(ms, false); }
        catch { throw new InvalidOperationException("no es un documento Word (.docx) válido."); }

        using (doc)
        {
            var body = doc.MainDocumentPart?.Document?.Body
                ?? throw new InvalidOperationException("el documento está vacío.");

            // Recorrido en orden de párrafos y tablas de primer nivel, agrupados por sección
            var secciones = new List<(string Titulo, List<string> Parrafos, List<List<List<List<string>>>> Tablas)>();
            foreach (var el in Bloques(body))
            {
                if (el is Paragraph par)
                {
                    var t = TextoParrafo(par).Trim();
                    if (t.Length == 0) continue;
                    if (EsTituloSeccion(t)) secciones.Add((Normalizar(t), [], []));
                    else if (secciones.Count > 0) secciones[^1].Parrafos.Add(t);
                }
                else if (el is Table tb && secciones.Count > 0)
                    secciones[^1].Tablas.Add(LeerTabla(tb));
            }

            var a = w.Acta;
            var tieneDesarrollo = secciones.Any(s => s.Titulo.Contains("DESARROLLO"));
            w.EsInterna = nombreArchivo.Contains("externa", StringComparison.OrdinalIgnoreCase) ? false
                : nombreArchivo.Contains("interna", StringComparison.OrdinalIgnoreCase) || tieneDesarrollo;

            foreach (var (titulo, parrafos, tablas) in secciones)
            {
                if (titulo.Contains("FIRMAS")) continue;
                if (titulo.Contains("INFORMACION GENERAL")) LeerInformacion(tablas, a);
                else if (titulo.Contains("SEGUIMIENTO")) LeerSeguimiento(tablas, a);
                else if (titulo.Contains("COMPROMISOS")) { if (w.EsInterna) LeerCompromisos(tablas, a); }
                else if (titulo.Contains("ASISTENTES")) LeerAsistentes(tablas, a);
                else if (titulo.Contains("DESARROLLO")) a.PuntosJson = JsonSerializer.Serialize(Puntos(parrafos));
                else if (titulo.Contains("RESUMEN")) a.Resumen = string.Join("\n", parrafos);
            }

            if (string.IsNullOrWhiteSpace(a.Codigo))
                throw new InvalidOperationException("no se encontró el número del acta (\"Acta No.\" en 1. INFORMACIÓN GENERAL). ¿Es un acta en formato FO-SG-GC-021?");
            a.Codigo = a.Codigo.Trim().ToUpperInvariant();
            if (!nombreArchivo.Contains(a.Codigo, StringComparison.OrdinalIgnoreCase))
                w.Advertencias.Add($"el nombre del archivo no coincide con el código del acta ({a.Codigo}); se usa el código del documento.");
            a.Fecha = FechaDesdeTexto(a.FechaTexto);
            if (a.Fecha is null && !string.IsNullOrWhiteSpace(a.FechaTexto))
                w.Advertencias.Add($"no se pudo convertir la fecha \"{a.FechaTexto}\"; se conserva como texto.");
            if (w.EsInterna && a.Compromisos.Count == 0)
                w.Advertencias.Add("no se encontró la tabla de compromisos (o está vacía).");

            var tipo = w.EsInterna ? TipoArchivoActa.InternaDocx : TipoArchivoActa.ExternaDocx;
            a.Archivos.Add(new ArchivoLeido(tipo, nombreArchivo, contenido));
        }
        return w;
    }

    // ── secciones ───────────────────────────────────────────────────────────
    private static void LeerInformacion(List<List<List<List<string>>>> tablas, ActaLeida a)
    {
        foreach (var fila in tablas.SelectMany(t => t))
            for (int i = 0; i < fila.Count - 1; i++)
            {
                var valor = Unir(fila[i + 1], " ");
                switch (Normalizar(Unir(fila[i], " ")).TrimEnd(':', '.').Trim())
                {
                    case "ACTA NO" or "ACTA N" or "ACTA NO." or "ACTA N°" or "ACTA": a.Codigo = valor; i++; break;
                    case "FECHA": a.FechaTexto = valor; i++; break;
                    case "PROYECTO": a.ProyectoTexto = valor; i++; break;
                    case "AREA": a.Area = valor; i++; break;
                    case "ASUNTO": a.Asunto = valor; i++; break;
                    case "MODALIDAD": a.Modalidad = valor; i++; break;
                    case "HORA": a.Hora = valor; i++; break;
                }
            }
    }

    private static void LeerAsistentes(List<List<List<List<string>>>> tablas, ActaLeida a)
    {
        var lista = new List<Dictionary<string, string>>();
        foreach (var t in tablas)
        {
            if (t.Count < 2) continue;
            var cols = Columnas(t[0]);
            if (!cols.TryGetValue("NOMBRE", out var cNombre)) continue;
            foreach (var f in t.Skip(1))
            {
                var nombre = Celda(f, cNombre);
                if (string.IsNullOrWhiteSpace(nombre)) continue;
                var d = new Dictionary<string, string> { ["nombre"] = nombre };
                if (cols.TryGetValue("CARGO", out var cc) && Celda(f, cc) is { Length: > 0 } cargo) d["cargo"] = cargo;
                if (cols.TryGetValue("EMPRESA", out var ce) && Celda(f, ce) is { Length: > 0 } emp) d["empresa"] = emp;
                lista.Add(d);
            }
        }
        if (lista.Count > 0) a.AsistentesJson = JsonSerializer.Serialize(lista);
    }

    private static void LeerCompromisos(List<List<List<List<string>>>> tablas, ActaLeida a)
    {
        foreach (var t in tablas)
        {
            if (t.Count < 2) continue;
            var cols = Columnas(t[0]);
            if (!cols.TryGetValue("COMPROMISO", out var cTexto)) continue;
            cols.TryGetValue("RESPONSABLE", out var cResp);
            cols.TryGetValue("EMPRESA", out var cEmp);
            var cFecha = cols.GetValueOrDefault("FECHA LIMITE", cols.GetValueOrDefault("FECHA", -1));
            foreach (var f in t.Skip(1))
            {
                var texto = Celda(f, cTexto);
                if (string.IsNullOrWhiteSpace(texto)) continue;
                var orden = a.Compromisos.Count + 1;
                a.Compromisos.Add(new CompromisoLeido(
                    $"{a.Codigo.Trim().ToUpperInvariant()}-{orden.ToString().PadLeft(2, '0')}", orden, texto,
                    NoVacio(Celda(f, cResp, " / ")) ?? "Por confirmar",
                    NoVacio(Celda(f, cEmp)), NoVacio(Celda(f, cFecha)), null, null, null));
            }
        }
    }

    private static void LeerSeguimiento(List<List<List<List<string>>>> tablas, ActaLeida a)
    {
        var lista = new List<Dictionary<string, string>>();
        foreach (var t in tablas)
        {
            if (t.Count < 2) continue;
            var cols = Columnas(t[0]);
            if (!cols.TryGetValue("ORIGEN", out var cOrigen)) continue;
            var cComp = cols.Where(k => k.Key.StartsWith("COMPROMISO")).Select(k => k.Value).DefaultIfEmpty(-1).First();
            var cEstado = cols.Where(k => k.Key.StartsWith("ESTADO")).Select(k => k.Value).DefaultIfEmpty(-1).First();
            var cFecha = cols.GetValueOrDefault("FECHA", -1);
            var cObs = cols.Where(k => k.Key.StartsWith("OBSERVACION")).Select(k => k.Value).DefaultIfEmpty(-1).First();
            foreach (var f in t.Skip(1))
            {
                var candidatos = CandidatosOrigen(Celda(f, cOrigen), a.Codigo);
                var origen = candidatos.FirstOrDefault() ?? Regex.Replace(Celda(f, cOrigen), @"\s+", " ").Trim().ToUpperInvariant();
                if (origen.Length == 0) continue;
                // "Compromiso y responsable": el último párrafo es el responsable, los anteriores el compromiso
                var parr = cComp >= 0 && cComp < f.Count ? f[cComp] : [];
                var responsable = parr.Count > 0 ? parr[^1].Trim() : "";
                var titulo = string.Join(" ", parr.Take(Math.Max(0, parr.Count - 1)).Where(x => x.Trim().Length > 0)).Trim();
                var s = new SeguimientoLeido(origen, candidatos, NoVacio(titulo), NoVacio(responsable),
                    NoVacio(Celda(f, cEstado)), NoVacio(Celda(f, cFecha)), NoVacio(Celda(f, cObs, "\n")));
                a.SeguimientoAnterior.Add(s);
                var d = new Dictionary<string, string> { ["origen"] = s.Origen };
                if (s.Titulo is not null) d["titulo"] = s.Titulo;
                if (s.Responsable is not null) d["responsable"] = s.Responsable;
                if (s.Estado is not null) d["estado"] = s.Estado;
                if (s.Fecha is not null) d["fecha"] = s.Fecha;
                if (s.Observacion is not null) d["observacion"] = s.Observacion;
                lista.Add(d);
            }
        }
        if (lista.Count > 0) a.TrazabilidadJson = JsonSerializer.Serialize(lista);
    }

    // Los Word escriben el origen de varias formas; se completan las abreviadas con la serie del acta:
    //   "001-09"     en ACT-SEG-2026-002        → ACT-SEG-2026-001-09
    //   "ING002-01"  en COS5SO-ACT-ING-2026-005 → COS5SO-ACT-ING-2026-002-01, o la serie corporativa
    //                ACT-ING-2026-002-01 (en COS6ML-ACT-SST-2026-002, "SST-001-02" era ACT-SST-2026-001-02)
    // Las celdas que solo nombran actas (sin número de compromiso) no dan candidatos: no se adivinan.
    public static List<string> CandidatosOrigen(string celda, string codigoActa)
    {
        var o = Regex.Replace(celda, @"\s+", " ").Trim().ToUpperInvariant();
        var cod = codigoActa.Trim().ToUpperInvariant();
        if (RxIdCompromiso().IsMatch(o)) return [o];
        if (RxOrigenCorto().IsMatch(o) && RxSerie().Match(cod) is { Success: true } serie)
            return [serie.Groups[1].Value + o];
        if (RxOrigenArea().Match(o) is { Success: true } m && RxPrefijoAnio().Match(cod) is { Success: true } pa)
        {
            var resto = $"{m.Groups[1].Value}-{pa.Groups[2].Value}-{m.Groups[2].Value}-{m.Groups[3].Value}";
            var conProyecto = pa.Groups[1].Value + resto;
            var corporativo = "ACT-" + resto;
            return conProyecto == corporativo ? [corporativo] : [conProyecto, corporativo];
        }
        return [];
    }

    // Desarrollo: cada subtítulo "3.1. …" abre un punto; los párrafos siguientes son su texto
    private static List<string> Puntos(List<string> parrafos)
    {
        var puntos = new List<StringBuilder>();
        foreach (var p in parrafos)
        {
            if (RxSubtitulo().IsMatch(p) || puntos.Count == 0) puntos.Add(new StringBuilder(p));
            else puntos[^1].Append('\n').Append(p);
        }
        return puntos.Select(x => x.ToString()).ToList();
    }

    // JSON con la misma forma de acta.json (queda en DatosOrigenJson como copia de lo leído)
    private static string JsonOrigen(ActaLeida a)
    {
        static JsonElement? Raw(string? j) => j is null ? null : JsonDocument.Parse(j).RootElement.Clone();
        var o = new Dictionary<string, object?>
        {
            ["codigo"] = a.Codigo, ["fecha"] = a.Fecha?.ToString("yyyy-MM-dd"), ["fechaTexto"] = a.FechaTexto,
            ["proyecto"] = a.ProyectoTexto, ["area"] = a.Area, ["asunto"] = a.Asunto, ["modalidad"] = a.Modalidad,
            ["hora"] = a.Hora, ["asistentes"] = Raw(a.AsistentesJson), ["resumen"] = a.Resumen, ["puntos"] = Raw(a.PuntosJson),
            ["trazabilidad"] = Raw(a.TrazabilidadJson),
            ["compromisos"] = a.Compromisos.Select(c => new { texto = c.Texto, responsable = c.Responsable, empresa = c.Empresa, fecha = c.FechaTexto }),
            ["fuente"] = new { tipo = "word", archivos = a.Archivos.Select(x => x.Filename) },
        };
        return JsonSerializer.Serialize(o);
    }

    // ── util OpenXML ────────────────────────────────────────────────────────
    private static IEnumerable<OpenXmlElement> Bloques(OpenXmlElement padre)
    {
        foreach (var el in padre.ChildElements)
        {
            if (el is Paragraph or Table) yield return el;
            else if (el is SdtBlock or SdtContentBlock or CustomXmlBlock)
                foreach (var x in Bloques(el)) yield return x;
        }
    }

    // fila → celda → párrafos
    private static List<List<List<string>>> LeerTabla(Table tb) =>
        tb.Elements<TableRow>()
          .Select(r => r.Elements<TableCell>()
              .Select(c => c.Elements<Paragraph>().Select(p => TextoParrafo(p).Trim()).ToList())
              .ToList())
          .ToList();

    private static string TextoParrafo(Paragraph p)
    {
        var sb = new StringBuilder();
        foreach (var el in p.Descendants())
        {
            if (el is Text t) sb.Append(t.Text);
            else if (el is TabChar) sb.Append(' ');
            else if (el is Break) sb.Append('\n');
        }
        return sb.ToString();
    }

    private static Dictionary<string, int> Columnas(List<List<string>> encabezado)
    {
        var d = new Dictionary<string, int>();
        for (int i = 0; i < encabezado.Count; i++)
            d.TryAdd(Normalizar(Unir(encabezado[i], " ")).TrimEnd(':', '.').Trim(), i);
        return d;
    }

    private static string Celda(List<List<string>> fila, int i, string sep = " ") =>
        i >= 0 && i < fila.Count ? Unir(fila[i], sep) : "";

    private static string Unir(List<string> parrafos, string sep) =>
        string.Join(sep, parrafos.Where(x => x.Length > 0)).Trim();

    private static string? NoVacio(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static bool EsTituloSeccion(string t)
    {
        var m = RxTitulo().Match(t);
        return m.Success && m.Groups[1].Value.Any(char.IsLetter) && m.Groups[1].Value == m.Groups[1].Value.ToUpper(CultureInfo.GetCultureInfo("es-CO"));
    }

    private static string Normalizar(string s)
    {
        var sb = new StringBuilder();
        foreach (var ch in s.Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sb.Append(ch);
        return Regex.Replace(sb.ToString().ToUpperInvariant(), @"\s+", " ").Trim();
    }

    [GeneratedRegex(@"^\d+\.\s+(.{4,})$")]
    private static partial Regex RxTitulo();

    [GeneratedRegex(@"^\d+\.\d+\.?\s")]
    private static partial Regex RxSubtitulo();

    [GeneratedRegex(@"^\d{1,4}-\d{2}$")]
    private static partial Regex RxOrigenCorto();

    [GeneratedRegex(@"^(.*-)\d+$")]
    private static partial Regex RxSerie();

    [GeneratedRegex(@"^([A-Z]{2,4})-?(\d{3})-(\d{2})$")]
    private static partial Regex RxOrigenArea();

    [GeneratedRegex(@"^(.*ACT-)[A-Z]+-(\d{4})-\d+$")]
    private static partial Regex RxPrefijoAnio();

    [GeneratedRegex(@"^(?:[A-Z0-9]+-)?ACT-[A-Z]+-\d{4}-\d{3}-\d{2}$")]
    private static partial Regex RxIdCompromiso();
}
