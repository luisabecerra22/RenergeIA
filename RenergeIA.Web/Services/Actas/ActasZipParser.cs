using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using RenergeIA.Core.Enums;

namespace RenergeIA.Web.Services.Actas;

// Lee un paquete ZIP con la información de "Actas IA" SIN tocar la base de datos.
// Estructura esperada (la misma de la carpeta local, comprimida):
//   Actas/<año>/<código>/acta.json + "<código> - Acta Interna|Externa.docx|.pdf"
//   datos/proyectos.json (catálogo de proyectos con alias y códigos anteriores)
//   datos/personas.json, alias.json, externos.json (solo se inventarían)
//   opcional: cualquier *.json con { "actas": [...], "compromisos": [...] } = export del
//   tablero (ej. estado_actual.json) con los estados/cierres/historial vigentes.
// Las carpetas con acta.json marcado "muestra": true se EXCLUYEN (igual que cargar_todas_actas.js).
public static partial class ActasZipParser
{
    public sealed record ArchivoLeido(TipoArchivoActa Tipo, string Filename, byte[] Contenido);

    public sealed record CompromisoLeido(
        string IdOrigen, int Orden, string Texto, string Responsable, string? Empresa,
        string? FechaTexto, string? Categoria, string? TipoTexto, string? ContinuidadDe);

    public sealed class ActaLeida
    {
        public string Codigo = "";
        public string Carpeta = "";
        public string Json = "";
        public DateTime? Fecha;
        public string? FechaTexto, ProyectoTexto, Area, Asunto, Modalidad, Hora, Resumen,
            ActaAnterior, FuenteUrl, AsistentesJson, PuntosJson, TrazabilidadJson, EstadoTexto;
        public bool SeguimientoPendienteConfirmacion;
        public List<CompromisoLeido> Compromisos = [];
        public List<ArchivoLeido> Archivos = [];
        public List<string> ArchivosFaltantes = [];
    }

    public sealed record CompromisoTablero(
        string Id, string Json, string? Estado, string? NotaCierre, string? CerradoPor, DateTime? CerradoEl,
        string? FechaTexto, DateTime? FechaLimite, string? Categoria, string? TipoTexto, string? Responsable,
        string? Empresa, bool RequiereConfirmacion, string? ObservacionSeguimiento, string? HistorialJson,
        string? OrigenOriginalJson, DateTime? Creado, string? EditadoPor, DateTime? EditadoEl);

    public sealed record ProyectoLeido(string Nombre, string? Codigo, string? AreasJson, string? AliasJson, string? CodigosAnterioresJson, string? IdOrigen);

    public sealed class PaqueteActas
    {
        public List<ActaLeida> Actas = [];
        public List<ProyectoLeido> Proyectos = [];
        public Dictionary<string, CompromisoTablero> Tablero = [];      // por id acta-NN
        public Dictionary<string, string> EstadosActaTablero = [];      // codigo acta -> estado del export
        public List<(string IdOrigen, string Json)> Seguimientos = [];
        public int Personas, Alias, Externos;
        public List<string> Advertencias = [];
    }

    private static readonly string[] _clientes = ["GreenYellow", "Atera Energy", "Atera", "Celsia"]; // misma lista del sistema anterior

    public static bool EsEmpresaCliente(string? empresa) =>
        !string.IsNullOrWhiteSpace(empresa) && _clientes.Any(c => empresa.Trim().Contains(c, StringComparison.OrdinalIgnoreCase));

    public static PaqueteActas Leer(Stream zipStream)
    {
        var p = new PaqueteActas();
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        // Agrupar entradas por carpeta de acta (la que contiene acta.json).
        // Los ZIP creados en Windows usan "\" como separador y el servidor corre en Linux,
        // así que las rutas se normalizan SIEMPRE a "/" antes de usarlas.
        var porCarpeta = zip.Entries
            .Where(e => e.Length > 0)
            .GroupBy(e => CarpetaDe(e.FullName))
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (carpeta, entradas) in porCarpeta)
        {
            var actaJson = entradas.FirstOrDefault(e => NombreDe(e.FullName).Equals("acta.json", StringComparison.OrdinalIgnoreCase));
            if (actaJson is null) continue;
            try
            {
                var json = LeerTexto(actaJson);
                using var doc = JsonDocument.Parse(json);
                var r = doc.RootElement;
                if (r.TryGetProperty("muestra", out var m) && m.ValueKind == JsonValueKind.True)
                {
                    p.Advertencias.Add($"{carpeta}: excluida (marcada como muestra).");
                    continue;
                }
                var acta = LeerActa(r, json, carpeta);
                if (string.IsNullOrWhiteSpace(acta.Codigo))
                {
                    p.Advertencias.Add($"{carpeta}: acta.json sin campo 'codigo' — excluida.");
                    continue;
                }
                LeerArchivosActa(acta, entradas);
                p.Actas.Add(acta);
            }
            catch (Exception ex)
            {
                p.Advertencias.Add($"{carpeta}: acta.json ilegible ({ex.Message}).");
            }
        }

        // Catálogos y exports sueltos
        foreach (var e in zip.Entries.Where(e => e.Length > 0 && e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
        {
            var nombre = NombreDe(e.FullName).ToLowerInvariant();
            if (nombre == "acta.json" || nombre == "firebase-admin-key.json") continue; // credenciales: jamás se leen
            try
            {
                if (nombre == "proyectos.json") LeerProyectos(LeerTexto(e), p);
                else if (nombre == "personas.json") p.Personas = ContarArray(LeerTexto(e));
                else if (nombre == "alias.json") p.Alias = ContarObjetoOArray(LeerTexto(e));
                else if (nombre == "externos.json") p.Externos = ContarArray(LeerTexto(e));
                else LeerPosibleExportTablero(LeerTexto(e), e.FullName, p);
            }
            catch (Exception ex)
            {
                p.Advertencias.Add($"{e.FullName}: no se pudo leer ({ex.Message}).");
            }
        }

        p.Actas = p.Actas.OrderBy(a => a.Fecha ?? DateTime.MaxValue).ThenBy(a => a.Codigo).ToList();
        return p;
    }

    private static ActaLeida LeerActa(JsonElement r, string json, string carpeta)
    {
        var a = new ActaLeida
        {
            Codigo = Texto(r, "codigo")?.Trim().ToUpperInvariant() ?? "",
            Carpeta = carpeta,
            Json = json,
            Fecha = FechaIso(Texto(r, "fecha")),
            FechaTexto = Texto(r, "fechaTexto"),
            ProyectoTexto = Texto(r, "proyecto"),
            Area = Texto(r, "area"),
            Asunto = Texto(r, "asunto"),
            Modalidad = Texto(r, "modalidad"),
            Hora = Texto(r, "hora"),
            Resumen = Texto(r, "resumen"),
            ActaAnterior = Texto(r, "actaAnterior")?.Trim().ToUpperInvariant(),
            EstadoTexto = Texto(r, "estado"),
            AsistentesJson = Crudo(r, "asistentes"),
            PuntosJson = Crudo(r, "puntos"),
            TrazabilidadJson = Crudo(r, "trazabilidad") ?? Crudo(r, "seguimiento"), // acepta ambos nombres, igual que el sistema anterior
            SeguimientoPendienteConfirmacion = r.TryGetProperty("seguimientoPendienteConfirmacion", out var spc) && spc.ValueKind == JsonValueKind.True,
        };
        if (r.TryGetProperty("fuente", out var fu) && fu.ValueKind == JsonValueKind.Object)
            a.FuenteUrl = Texto(fu, "url");

        if (r.TryGetProperty("compromisos", out var comps) && comps.ValueKind == JsonValueKind.Array)
        {
            int i = 0;
            foreach (var c in comps.EnumerateArray())
            {
                i++;
                // Mismo ID que el sistema anterior: <codigo>-NN con NN = posición 1-based a 2 dígitos
                var nn = i.ToString().PadLeft(2, '0');
                a.Compromisos.Add(new CompromisoLeido(
                    $"{a.Codigo}-{nn}", i,
                    Texto(c, "texto") ?? "",
                    Texto(c, "responsable") ?? "Por confirmar",
                    Texto(c, "empresa"),
                    Texto(c, "fecha"),
                    Texto(c, "categoria"),
                    Texto(c, "tipo"),
                    Texto(c, "continuidadDe")?.Trim().ToUpperInvariant()));
            }
        }
        return a;
    }

    private static void LeerArchivosActa(ActaLeida acta, List<ZipArchiveEntry> entradas)
    {
        (TipoArchivoActa Tipo, string Sufijo)[] esperados =
        [
            (TipoArchivoActa.InternaDocx, "- Acta Interna.docx"),
            (TipoArchivoActa.InternaPdf,  "- Acta Interna.pdf"),
            (TipoArchivoActa.ExternaDocx, "- Acta Externa.docx"),
            (TipoArchivoActa.ExternaPdf,  "- Acta Externa.pdf"),
        ];
        foreach (var (tipo, sufijo) in esperados)
        {
            var e = entradas.FirstOrDefault(x => NombreDe(x.FullName).EndsWith(sufijo, StringComparison.OrdinalIgnoreCase));
            if (e is null) { acta.ArchivosFaltantes.Add(sufijo.TrimStart('-', ' ')); continue; }
            using var ms = new MemoryStream();
            using var s = e.Open();
            s.CopyTo(ms);
            acta.Archivos.Add(new ArchivoLeido(tipo, NombreDe(e.FullName), ms.ToArray()));
        }
    }

    private static void LeerProyectos(string json, PaqueteActas p)
    {
        using var doc = JsonDocument.Parse(json);
        var raiz = doc.RootElement;
        var lista = raiz.ValueKind == JsonValueKind.Array ? raiz
            : raiz.TryGetProperty("proyectos", out var pr) ? pr : raiz;
        if (lista.ValueKind != JsonValueKind.Array) return;
        foreach (var e in lista.EnumerateArray())
        {
            var nombre = Texto(e, "nombre") ?? Texto(e, "proyecto");
            if (string.IsNullOrWhiteSpace(nombre)) continue;
            p.Proyectos.Add(new ProyectoLeido(nombre.Trim(), Texto(e, "codigo")?.Trim().ToUpperInvariant(),
                Crudo(e, "areas"), Crudo(e, "alias"), Crudo(e, "codigosAnteriores"), Texto(e, "id")));
        }
    }

    // Export del tablero (ej. estado_actual.json): { actas: [...], compromisos: [...] }
    private static void LeerPosibleExportTablero(string json, string ruta, PaqueteActas p)
    {
        using var doc = JsonDocument.Parse(json);
        var r = doc.RootElement;
        if (r.ValueKind != JsonValueKind.Object) return;
        var esExport = r.TryGetProperty("compromisos", out var comps) && comps.ValueKind == JsonValueKind.Array;
        if (!esExport) return;

        int n = 0;
        foreach (var c in comps.EnumerateArray())
        {
            var id = (Texto(c, "id") ?? "").Trim().ToUpperInvariant();
            if (id.Length == 0) continue;
            p.Tablero[id] = new CompromisoTablero(
                id, c.GetRawText(),
                Texto(c, "estado"), Texto(c, "notaCierre"), Texto(c, "cerradoPor"), FechaFlexible(Texto(c, "cerradoEl")),
                Texto(c, "fechaTexto"), FechaIso(Texto(c, "fechaLimite")), Texto(c, "categoria"), Texto(c, "tipo"),
                Texto(c, "responsable"), Texto(c, "empresa"),
                c.TryGetProperty("requiereConfirmacion", out var rq) && rq.ValueKind == JsonValueKind.True,
                Texto(c, "observacionSeguimiento"), Crudo(c, "historial"), Crudo(c, "origenOriginal"),
                FechaFlexible(Texto(c, "creado")), Texto(c, "editadoPor"), FechaFlexible(Texto(c, "editadoEl")));
            n++;
        }
        if (r.TryGetProperty("actas", out var actas) && actas.ValueKind == JsonValueKind.Array)
            foreach (var a in actas.EnumerateArray())
            {
                var cod = Texto(a, "codigo")?.Trim().ToUpperInvariant();
                var est = Texto(a, "estado");
                if (!string.IsNullOrEmpty(cod) && !string.IsNullOrEmpty(est)) p.EstadosActaTablero[cod] = est;
            }
        if (r.TryGetProperty("seguimientos", out var segs) && segs.ValueKind == JsonValueKind.Array)
            foreach (var s in segs.EnumerateArray())
            {
                var id = Texto(s, "id") ?? Texto(s, "idOrigen");
                if (!string.IsNullOrWhiteSpace(id)) p.Seguimientos.Add((id.Trim().ToUpperInvariant(), s.GetRawText()));
            }
        p.Advertencias.Add($"Export del tablero detectado: {ruta} ({n} compromisos con estado).");
    }

    // ── Fechas ──────────────────────────────────────────────────────────────
    // Solo se convierten fechas EXPLÍCITAS (ISO o "D de <mes> de AAAA").
    // Las fechas descriptivas ("Por definir", rangos, condicionales) se conservan como texto.
    private static readonly string[] _meses = ["enero","febrero","marzo","abril","mayo","junio","julio","agosto","septiembre","octubre","noviembre","diciembre"];

    [GeneratedRegex(@"\b(\d{4})-(\d{2})-(\d{2})\b")]
    private static partial Regex RxIso();

    [GeneratedRegex(@"\b(\d{1,2})\s+de\s+([a-záéíóú]+)\s+de\s+(\d{4})", RegexOptions.IgnoreCase)]
    private static partial Regex RxLarga();

    public static DateTime? FechaDesdeTexto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return null;
        var t = texto.Trim();
        var iso = RxIso().Match(t);
        if (iso.Success && DateTime.TryParseExact(iso.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fi))
            return fi;
        var lg = RxLarga().Match(t.ToLowerInvariant());
        if (lg.Success)
        {
            var mes = Array.IndexOf(_meses, Quitar(lg.Groups[2].Value)) + 1;
            if (mes > 0 && int.TryParse(lg.Groups[1].Value, out var dia) && int.TryParse(lg.Groups[3].Value, out var anio))
            {
                try { return new DateTime(anio, mes, dia); } catch { return null; }
            }
        }
        return null;
    }

    private static DateTime? FechaIso(string? t) =>
        DateTime.TryParse(t, CultureInfo.InvariantCulture, DateTimeStyles.None, out var f) && f.Year is >= 2000 and <= 2100 ? f.Date : null;

    private static DateTime? FechaFlexible(string? t) =>
        DateTime.TryParse(t, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var f) ? DateTime.SpecifyKind(f, DateTimeKind.Utc) : null;

    private static string Quitar(string s)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var ch in s.Normalize(System.Text.NormalizationForm.FormD))
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sb.Append(ch);
        return sb.ToString();
    }

    // Código de proyecto dentro del texto del acta (misma heurística del sistema anterior)
    [GeneratedRegex(@"\bCOS\d+[A-Z]+\d*\b")]
    private static partial Regex RxProyecto();

    public static string? CodigoProyectoDe(string? proyectoTexto)
    {
        if (string.IsNullOrWhiteSpace(proyectoTexto)) return null;
        var m = RxProyecto().Match(proyectoTexto.ToUpperInvariant());
        return m.Success ? m.Value : null;
    }

    // ── util json ───────────────────────────────────────────────────────────
    private static string? Texto(JsonElement e, string prop) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(prop, out var v)
            ? v.ValueKind switch
            {
                JsonValueKind.String => v.GetString(),
                JsonValueKind.Number => v.GetRawText(),
                _ => null
            }
            : null;

    private static string? Crudo(JsonElement e, string prop) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(prop, out var v) && v.ValueKind is JsonValueKind.Array or JsonValueKind.Object
            ? v.GetRawText() : null;

    private static string LeerTexto(ZipArchiveEntry e)
    {
        using var s = e.Open();
        using var r = new StreamReader(s);
        return r.ReadToEnd();
    }

    private static int ContarArray(string json)
    {
        using var d = JsonDocument.Parse(json);
        return d.RootElement.ValueKind == JsonValueKind.Array ? d.RootElement.GetArrayLength() : 0;
    }

    private static int ContarObjetoOArray(string json)
    {
        using var d = JsonDocument.Parse(json);
        return d.RootElement.ValueKind switch
        {
            JsonValueKind.Array => d.RootElement.GetArrayLength(),
            JsonValueKind.Object => d.RootElement.EnumerateObject().Count(),
            _ => 0
        };
    }

    // Separadores de ZIPs de Windows ("\") y de Linux ("/") por igual
    private static string NombreDe(string fullName)
    {
        var r = fullName.Replace('\\', '/').TrimEnd('/');
        var i = r.LastIndexOf('/');
        return i < 0 ? r : r[(i + 1)..];
    }

    private static string CarpetaDe(string fullName)
    {
        var r = fullName.Replace('\\', '/').TrimEnd('/');
        var i = r.LastIndexOf('/');
        return i < 0 ? "" : r[..i];
    }
}
