using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;

namespace RenergeIA.Web.Services.Actas;

// Seguimiento de compromisos por persona (definido con la usuaria el 2026-10-26):
// - Solo compromisos PROPIOS de Renergeia (los recordatorios a clientes y los de proveedores no entran).
// - Cada compromiso cuenta SOLO para su responsable principal (ResponsablePrincipal o, si no se ha
//   definido, el primero de la lista "A / B").
// - % de cumplimiento = cumplidos (cerrados) ÷ total asignado.
// - Los nombres se unifican AUTOMÁTICAMENTE: "Miguel Prada" = "Miguel Angel Prada Muñoz" porque todas
//   sus palabras están en el nombre largo y no hay otro candidato; si hay dos candidatos (dos Miguel)
//   no se unifican. Los responsables genéricos ("Equipo del proyecto", "Por confirmar") van aparte.
public static partial class SeguimientoPersonas
{
    public const string SinPersona = "Equipos / sin responsable definido";

    public sealed record CompromisoPersona(CompromisoReunion Compromiso, string Situacion, int? DiasAtraso, bool? ATiempo);

    public sealed class FichaPersona
    {
        public string Persona = "";
        public List<string> TambienComo = [];
        public List<CompromisoPersona> Compromisos = [];
        public int Total => Compromisos.Count;
        public int Cumplidos => Compromisos.Count(c => c.Situacion == "cerr");
        public int Vencidos => Compromisos.Count(c => c.Situacion == "venc");
        public int Pendientes => Total - Cumplidos;
        public int PendientesEnPlazo => Compromisos.Count(c => c.Situacion is "ok" or "prox");
        public int Proximos => Compromisos.Count(c => c.Situacion == "prox");
        public int SinFecha => Compromisos.Count(c => c.Situacion == "sinfecha");
        public int CumplidosATiempo => Compromisos.Count(c => c.ATiempo == true);
        public int CumplidosTarde => Compromisos.Count(c => c.ATiempo == false);
        public double PorcentajeCumplimiento => Total == 0 ? 0 : 100.0 * Cumplidos / Total;
        public DateTime? UltimoCierre => Compromisos.Where(c => c.Situacion == "cerr").Max(c => c.Compromiso.CerradoEl);
    }

    public static bool EsDeRenergeia(CompromisoReunion c) =>
        c.Tipo == TipoCompromisoReunion.Propio &&
        (string.IsNullOrWhiteSpace(c.Empresa) || c.Empresa.Contains("renergeia", StringComparison.OrdinalIgnoreCase));

    public static string Situacion(CompromisoReunion c, DateTime hoy) =>
        c.Estado == EstadoCompromisoReunion.Cerrado ? "cerr"
        : c.FechaLimite is null ? "sinfecha"
        : c.FechaLimite.Value.Date < hoy ? "venc"
        : c.FechaLimite.Value.Date <= hoy.AddDays(7) ? "prox" : "ok";

    public static string Principal(CompromisoReunion c) =>
        !string.IsNullOrWhiteSpace(c.ResponsablePrincipal) ? c.ResponsablePrincipal.Trim()
        : Partes(c.Responsable).FirstOrDefault() ?? "";

    public static IEnumerable<string> Partes(string? responsable) =>
        (responsable ?? "").Split(['/', ';', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    // Arma las fichas. "nombres" = todas las formas en que aparecen las personas (de todos los
    // compromisos, incluso compartidos) para unificar mejor; solo se cuentan los de Renergeia.
    public static List<FichaPersona> Fichas(IEnumerable<CompromisoReunion> todos, DateTime hoy)
    {
        var lista = todos.ToList();
        var unif = Unificador.Crear(lista.SelectMany(c => Partes(c.Responsable).Append(c.ResponsablePrincipal ?? "")));
        var fichas = new Dictionary<string, FichaPersona>();
        foreach (var c in lista.Where(EsDeRenergeia))
        {
            var original = Principal(c);
            var persona = unif.Canonico(original) ?? SinPersona;
            if (!fichas.TryGetValue(persona, out var f)) fichas[persona] = f = new FichaPersona { Persona = persona };
            if (persona != SinPersona && !string.Equals(original, persona, StringComparison.OrdinalIgnoreCase)
                && !f.TambienComo.Contains(original, StringComparer.OrdinalIgnoreCase))
                f.TambienComo.Add(original);
            var sit = Situacion(c, hoy);
            int? atraso = sit == "venc" ? (hoy - c.FechaLimite!.Value.Date).Days : null;
            bool? aTiempo = null;
            if (sit == "cerr" && c.FechaLimite is not null && c.CerradoEl is not null)
            {
                var cierre = c.CerradoEl.Value.ToUniversalTime().AddHours(-5).Date; // hora Colombia
                aTiempo = cierre <= c.FechaLimite.Value.Date;
                if (aTiempo == false) atraso = (cierre - c.FechaLimite.Value.Date).Days;
            }
            f.Compromisos.Add(new CompromisoPersona(c, sit, atraso, aTiempo));
        }
        return fichas.Values.OrderBy(f => f.Persona == SinPersona).ThenBy(f => f.Persona).ToList();
    }

    // ── Unificación automática de nombres ───────────────────────────────────
    public sealed class Unificador
    {
        private readonly Dictionary<string, string> _canonico = []; // clave normalizada → nombre a mostrar

        public string? Canonico(string? nombre)
        {
            var k = Clave(nombre);
            if (k is null) return null;
            return _canonico.TryGetValue(k, out var c) ? c : null;
        }

        public static Unificador Crear(IEnumerable<string> nombres)
        {
            var u = new Unificador();
            // variantes: clave → (tokens, forma original más larga)
            var variantes = new Dictionary<string, (string[] Tokens, string Original)>();
            foreach (var n in nombres)
            {
                var k = Clave(n);
                if (k is null) continue;
                var limpio = Limpiar(n);
                if (!variantes.TryGetValue(k, out var v) || limpio.Length > v.Original.Length)
                    variantes[k] = (k.Split(' '), limpio);
            }

            // Texto libre ("Richard Suarez apoya el reenvío…"): se resuelve a la persona contenida en él
            var libres = variantes.Where(x => x.Value.Tokens.Any(_palabrasLibres.Contains)).Select(x => x.Key).ToHashSet();
            var nombresReales = variantes.Where(x => !libres.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            // Raíz de cada variante: el nombre más largo que la contiene, si es ÚNICO
            var raiz = nombresReales.Keys.ToDictionary(k => k, k => k);
            foreach (var (k, v) in nombresReales.OrderByDescending(x => x.Value.Tokens.Length))
            {
                var candidatos = nombresReales
                    .Where(w => w.Key != k && Llenas(w.Value.Tokens).Length > Llenas(v.Tokens).Length && Contiene(w.Value.Tokens, v.Tokens))
                    .Select(w => Raiz(raiz, w.Key)).Distinct().ToList();
                if (candidatos.Count == 1) raiz[k] = candidatos[0];
            }
            foreach (var k in libres)
            {
                var toks = variantes[k].Tokens;
                var mejor = nombresReales.Where(w => Llenas(w.Value.Tokens).Length >= 2 && Contiene(toks, w.Value.Tokens))
                    .OrderByDescending(w => w.Value.Tokens.Length).Select(w => Raiz(raiz, w.Key)).Distinct().ToList();
                if (mejor.Count == 1) raiz[k] = mejor[0];
            }

            foreach (var k in variantes.Keys)
            {
                if (!raiz.TryGetValue(k, out var r)) continue;                 // texto libre sin persona: queda sin responsable
                r = Raiz(raiz, r);
                u._canonico[k] = variantes[r].Original;
            }
            return u;
        }

        private static string Raiz(Dictionary<string, string> raiz, string k)
        {
            var vistos = new HashSet<string>();
            while (raiz.TryGetValue(k, out var r) && r != k && vistos.Add(k)) k = r;
            return k;
        }

        // Cada palabra completa de "corto" está en "largo"; cada inicial ("F.") coincide con alguna palabra
        private static bool Contiene(string[] largo, string[] corto) =>
            corto.All(t => t.Length == 1 ? largo.Any(l => l.Length > 1 && l[0] == t[0]) : largo.Contains(t));

        private static string[] Llenas(string[] t) => t.Where(x => x.Length > 1).ToArray();
    }

    private static readonly HashSet<string> _genericos =
    [
        "equipo", "equipos", "todo", "todos", "cada", "por", "responsable", "responsables", "compras",
        "ingenieria", "hseq", "recursos", "contable", "area", "proveedor", "gerencia", "sistemas", "calidad",
        "seguridad", "ambiental", "construccion", "operacion", "o&m", "oym", "renergeia", "consultores"
    ];

    private static readonly HashSet<string> _palabrasLibres =
        ["el", "la", "los", "las", "si", "se", "y", "en", "para", "que", "con", "apoya", "requiere", "envio", "contactos", "accesos", "repositorio", "completar"];

    // Clave normalizada de un nombre de persona, o null si es genérico / vacío
    public static string? Clave(string? nombre)
    {
        var t = Limpiar(nombre);
        if (t.Length == 0) return null;
        var sb = new StringBuilder();
        foreach (var ch in t.Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sb.Append(ch);
        var tokens = RxNoLetra().Replace(sb.ToString().ToLowerInvariant(), " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0 || _genericos.Contains(tokens[0])) return null;
        if (tokens.Length == 1 && tokens[0].Length < 3) return null;
        return string.Join(' ', tokens);
    }

    // Quita aclaraciones entre paréntesis y lo que sigue a ":" ("Ginny Niño (Ingeniería)" → "Ginny Niño")
    private static string Limpiar(string? nombre)
    {
        var t = nombre ?? "";
        var i = t.IndexOf(':');
        if (i >= 0) t = t[..i];
        t = RxParentesis().Replace(t, " ");
        i = t.IndexOf('(');
        if (i >= 0) t = t[..i];
        return RxEspacios().Replace(t.Replace(")", " "), " ").Trim();
    }

    [GeneratedRegex(@"\([^)]*\)")]
    private static partial Regex RxParentesis();

    [GeneratedRegex(@"[^a-z]+")]
    private static partial Regex RxNoLetra();

    [GeneratedRegex(@"\s+")]
    private static partial Regex RxEspacios();
}
