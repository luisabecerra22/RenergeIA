using System.Globalization;
using System.Text.RegularExpressions;

namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Lectura determinista de lo que escribe obra al reportar (documento §7.1).
/// No adivina: si algo no es claro (ej. "80 pilotes" sin decir si es del día o
/// acumulado), lo deja en null y Helvis pregunta. Es puro para poder probarlo.
/// </summary>
public static partial class ReporteTexto
{
    public record Lectura(decimal? Cantidad, decimal? Porcentaje, bool? EsAcumulado, bool EsDeAyer);

    // Palabras que indican trabajo DEL DÍA: "hoy hincamos", "hicimos", "se instalaron"…
    private static readonly string[] PistasDia =
        ["hoy", "hicimos", "hincamos", "instalamos", "ejecutamos", "avanzamos", "montamos", "terminamos hoy",
         "en el dia", "durante el dia", "se hicieron", "se instalaron", "se hincaron", "realizamos", "ayer"];

    // Palabras que indican ACUMULADO: "llevamos 400", "vamos en", "en total", "acumulado"…
    private static readonly string[] PistasAcumulado =
        ["llevamos", "vamos en", "vamos por", "van ", "en total", "acumulad", "total de", "a la fecha", "hasta hoy"];

    public static Lectura Leer(string texto)
    {
        var t = Normalizar(texto);
        decimal? porcentaje = null, cantidad = null;

        var mPct = Porcentual().Match(t);
        if (mPct.Success && Numero(mPct.Groups[1].Value) is decimal p) porcentaje = p;
        else
        {
            var mPor = PorCiento().Match(t);
            if (mPor.Success && Numero(mPor.Groups[1].Value) is decimal p2) porcentaje = p2;
        }

        if (porcentaje is null)
        {
            // Primer número que no sea parte de una fecha ni de horas/personas
            foreach (Match m in Numeros().Matches(t))
            {
                var resto = t[(m.Index + m.Length)..].TrimStart();
                if (resto.StartsWith("hora") || resto.StartsWith("h ") || resto.StartsWith("persona") ||
                    resto.StartsWith("ayudante") || resto.StartsWith("trabajador")) continue;
                if (Numero(m.Value) is decimal c) { cantidad = c; break; }
            }
        }

        bool? acumulado = null;
        if (porcentaje is not null) acumulado = true; // un porcentaje siempre es acumulado
        else if (cantidad is not null)
        {
            var dia = PistasDia.Any(t.Contains);
            var acu = PistasAcumulado.Any(t.Contains);
            if (dia && !acu) acumulado = false;
            else if (acu && !dia) acumulado = true;
        }

        var deAyer = Regex.IsMatch(t, @"\bayer\b");
        return new Lectura(cantidad, porcentaje, acumulado, deAyer);
    }

    /// <summary>Lee "8 personas 9 horas", "8 y 9", "8 personas", "9 h". Devuelve lo que encuentre.</summary>
    public static (int? Personas, decimal? Horas) LeerRecursos(string texto)
    {
        var t = Normalizar(texto);
        int? personas = null;
        decimal? horas = null;

        var mp = Regex.Match(t, @"(\d+)\s*(personas?|ayudantes?|trabajadores?|operarios?|hombres?|pers\b|p\b)");
        if (mp.Success) personas = int.Parse(mp.Groups[1].Value, CultureInfo.InvariantCulture);
        var mh = Regex.Match(t, @"(\d+(?:[.,]\d+)?)\s*(horas?|hrs?|h\b)");
        if (mh.Success) horas = Numero(mh.Groups[1].Value);

        if (personas is null && horas is null)
        {
            var nums = Numeros().Matches(t).Select(m => Numero(m.Value)).Where(n => n is not null).ToList();
            if (nums.Count >= 1) personas = (int)nums[0]!.Value;
            if (nums.Count >= 2) horas = nums[1];
        }
        return (personas, horas);
    }

    /// <summary>
    /// Elige las actividades cuyo nombre coincide con el texto (raíces de 5 letras:
    /// "hincamos" encuentra "Hincado de pilotes"). Devuelve todas las que empatan en
    /// el mejor puntaje, para preguntar si hay más de una.
    /// </summary>
    public static List<T> Coincidencias<T>(string texto, IEnumerable<T> actividades, Func<T, string> nombre)
    {
        var raicesTexto = Raices(texto);
        if (raicesTexto.Count == 0) return [];

        var puntajes = actividades
            .Select(a => (a, puntos: Raices(nombre(a)).Count(raicesTexto.Contains)))
            .Where(x => x.puntos > 0)
            .ToList();
        if (puntajes.Count == 0) return [];
        var mejor = puntajes.Max(x => x.puntos);
        return puntajes.Where(x => x.puntos == mejor).Select(x => x.a).ToList();
    }

    private static readonly HashSet<string> Vacias =
        ["hoy", "ayer", "para", "con", "los", "las", "del", "que", "por", "una", "uno", "unos", "unas", "de",
         "en", "el", "la", "se", "al", "y", "a", "llevamos", "vamos", "hicimos", "total", "acumulado",
         "avance", "reporte", "actividad", "frente", "metros", "unidades", "horas", "personas"];

    private static HashSet<string> Raices(string texto) =>
        Normalizar(texto)
            .Split([' ', ',', '.', ';', ':', '-', '/', '(', ')', '%'], StringSplitOptions.RemoveEmptyEntries)
            .Where(p => p.Length >= 4 && !Vacias.Contains(p) && !p.Any(char.IsDigit))
            .Select(p => p.Length > 5 ? p[..5] : p)
            .ToHashSet();

    public static decimal? Numero(string s) =>
        decimal.TryParse(s.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var n) ? n : null;

    public static string Normalizar(string texto)
    {
        var sinTildes = new string((texto ?? "").Trim().ToLowerInvariant()
            .Normalize(System.Text.NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        return Regex.Replace(sinTildes.Normalize(System.Text.NormalizationForm.FormC), @"\s+", " ");
    }

    [GeneratedRegex(@"(\d+(?:[.,]\d+)?)\s*%")]
    private static partial Regex Porcentual();

    [GeneratedRegex(@"(\d+(?:[.,]\d+)?)\s*(?:por ciento|porciento)")]
    private static partial Regex PorCiento();

    [GeneratedRegex(@"\d+(?:[.,]\d+)?")]
    private static partial Regex Numeros();
}
