using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using RenergeIA.Core.Entities;

namespace RenergeIA.Web.Services.Actas;

// Proyecto "efectivo" de un compromiso (definido con la usuaria el 2026-10-26). No modifica lo guardado:
// - Si el acta es de un proyecto (trae código COSxxx), ese es el proyecto.
// - Si el acta es general / multiproyecto, se usa la etiqueta al inicio del texto:
//   "[Kimberly] …", "[COS6AU - La Aurora] …", "[Aurora] …", "[Kimberly / traslado a Villanueva] …".
//   Se toma lo que va antes de " / ", " — " o " - " (salvo que sea un código COS) y se busca en el
//   catálogo de proyectos (nombre, alias, código) y en las etiquetas que sí traen código
//   ("COS6AU - La Aurora" enseña que "Aurora" es COS6AU). "[General …]" = General Renergeia.
// - Las variantes de un mismo código ("COS6VL - Villanueva (plantas 1, 2, 3 y 4)") se unifican
//   con el nombre del catálogo: "COS6VL - Villanueva".
public sealed partial class ProyectoCompromiso
{
    public const string General = "General Renergeia";

    private readonly Dictionary<string, string> _nombrePorCodigo = [];   // COS6AU → La Aurora
    private readonly Dictionary<string, string> _codigoPorNombre = [];   // "la aurora", "aurora" → COS6AU

    // proyectosApp = (Codigo, Nombre) de los proyectos de RenergeIA: reconocen las etiquetas desde ya,
    // aunque todavía no se haya subido ninguna acta de ese proyecto con su código
    public static ProyectoCompromiso Crear(IEnumerable<ProyectoActa> catalogo, IEnumerable<CompromisoReunion> compromisos,
        IEnumerable<(string? Codigo, string? Nombre)>? proyectosApp = null)
    {
        var p = new ProyectoCompromiso();
        foreach (var pa in catalogo.Where(x => !string.IsNullOrWhiteSpace(x.Codigo)))
        {
            var cod = pa.Codigo!.Trim().ToUpperInvariant();
            p._nombrePorCodigo.TryAdd(cod, pa.Nombre.Trim());
            p.Aprender(pa.Nombre, cod);
            foreach (var a in Alias(pa.AliasJson)) p.Aprender(a, cod);
        }
        foreach (var (codigo, nombreApp) in proyectosApp ?? [])
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombreApp)) continue;
            var cod = codigo.Trim().ToUpperInvariant();
            // el nombre de la app puede traer el código ("COS5SO - La Soberana"): se deja solo el nombre
            var nombre = RxCodigo().Match(nombreApp.ToUpperInvariant()) is { Success: true } mn ? NombreTrasCodigo(nombreApp, mn) : nombreApp.Trim();
            if (nombre.Length == 0) continue;
            p._nombrePorCodigo.TryAdd(cod, nombre);
            p.Aprender(nombre, cod);
        }
        // Lo que enseñan los propios datos: textos del acta y etiquetas con código
        var lista = compromisos.ToList();
        foreach (var texto in lista.Select(c => c.ProyectoTexto).Concat(lista.Select(c => Etiqueta(c.Texto))))
        {
            if (string.IsNullOrWhiteSpace(texto) || RxCodigo().Match(texto.ToUpperInvariant()) is not { Success: true } m) continue;
            var nombre = NombreTrasCodigo(texto, m);
            if (nombre.Length == 0) continue;
            p._nombrePorCodigo.TryAdd(m.Value, nombre);   // el catálogo, si existe, ya ganó arriba
            p.Aprender(nombre, m.Value);
        }
        return p;
    }

    public string De(CompromisoReunion c)
    {
        var acta = c.ProyectoTexto?.Trim();
        if (!string.IsNullOrEmpty(acta) && RxCodigo().Match(acta.ToUpperInvariant()) is { Success: true } mc)
            return Mostrar(mc.Value, NombreTrasCodigo(acta, mc));

        var etiqueta = Etiqueta(c.Texto);
        if (etiqueta is not null) return Resolver(etiqueta);

        if (string.IsNullOrEmpty(acta) || Clave(acta).StartsWith("general")) return General;
        return Resolver(acta);
    }

    private string Resolver(string texto)
    {
        var up = texto.ToUpperInvariant();
        if (RxCodigo().Match(up) is { Success: true } m) return Mostrar(m.Value, NombreTrasCodigo(texto, m));
        var primero = RxSeparador().Split(texto)[0].Trim();
        var k = Clave(primero);
        if (k.Length == 0 || k.StartsWith("general")) return General;
        if (_codigoPorNombre.TryGetValue(k, out var cod)) return Mostrar(cod, null);
        return primero;
    }

    private string Mostrar(string codigo, string? nombreTexto)
    {
        var nombre = _nombrePorCodigo.GetValueOrDefault(codigo) ?? nombreTexto;
        return string.IsNullOrWhiteSpace(nombre) ? codigo : $"{codigo} - {nombre}";
    }

    private void Aprender(string? nombre, string codigo)
    {
        var k = Clave(nombre);
        if (k.Length == 0) return;
        _codigoPorNombre.TryAdd(k, codigo);
        // "la aurora" también se reconoce como "aurora"
        foreach (var art in new[] { "la ", "el ", "los ", "las " })
            if (k.StartsWith(art) && k.Length > art.Length + 2) _codigoPorNombre.TryAdd(k[art.Length..], codigo);
    }

    // "[Kimberly / traslado a Villanueva] Organizar…" → "Kimberly / traslado a Villanueva"
    public static string? Etiqueta(string? texto)
    {
        var m = RxEtiqueta().Match(texto ?? "");
        return m.Success ? System.Net.WebUtility.HtmlDecode(m.Groups[1].Value).Trim() : null;
    }

    // "COS6VL - Villanueva (plantas 1, 2, 3 y 4)" → "Villanueva"
    private static string NombreTrasCodigo(string texto, Match m)
    {
        var resto = texto[(m.Index + m.Length)..];
        resto = RxParentesis().Replace(resto, "");
        return resto.Trim(' ', '-', '—', '–', ':').Trim();
    }

    private static IEnumerable<string> Alias(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            using var d = JsonDocument.Parse(json);
            return d.RootElement.ValueKind == JsonValueKind.Array
                ? d.RootElement.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String).Select(e => e.GetString()!).ToList()
                : [];
        }
        catch { return []; }
    }

    private static string Clave(string? s)
    {
        var sb = new StringBuilder();
        foreach (var ch in (s ?? "").Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sb.Append(ch);
        return RxEspacios().Replace(RxNoAlfa().Replace(sb.ToString().ToLowerInvariant(), " "), " ").Trim();
    }

    [GeneratedRegex(@"\bCOS\d+[A-Z]+\d*\b")]
    private static partial Regex RxCodigo();

    [GeneratedRegex(@"^\s*\[([^\]]{1,80})\]")]
    private static partial Regex RxEtiqueta();

    [GeneratedRegex(@"\s+(?:/|—|–|-)\s+")]
    private static partial Regex RxSeparador();

    [GeneratedRegex(@"\([^)]*\)")]
    private static partial Regex RxParentesis();

    [GeneratedRegex(@"[^a-z0-9&]+")]
    private static partial Regex RxNoAlfa();

    [GeneratedRegex(@"\s+")]
    private static partial Regex RxEspacios();
}
