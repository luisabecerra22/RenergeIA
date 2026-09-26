using System.Globalization;
using System.Text.RegularExpressions;

namespace RenergeIA.Web.Services;

/// <summary>
/// Interpreta texto pegado desde Excel (dos columnas: fecha y % real acumulado) para cargar el
/// histórico de la Curva S. Tolera separadores tab / ; / | / espacios, fechas dd/MM/yyyy,
/// d-MMM-yy (meses en español o inglés), yyyy-MM-dd y seriales de Excel, y porcentajes como
/// "41,50%", "41.5", "0,415" (si TODOS los valores son ≤ 1 se asumen fracciones).
/// </summary>
public static class CurvaRealHistoricaParser
{
    public sealed record Fila(int Linea, string Texto, DateTime? Fecha, decimal? Porcentaje, string? Error)
    {
        public bool EsValida => Fecha.HasValue && Porcentaje.HasValue && Error is null;
    }

    public sealed record Resultado(List<Fila> Filas)
    {
        public List<(DateTime Fecha, decimal Porcentaje)> Puntos => Filas
            .Where(f => f.EsValida)
            .GroupBy(f => f.Fecha!.Value.Date)
            .Select(g => (g.Key, g.Last().Porcentaje!.Value))
            .OrderBy(p => p.Key)
            .ToList();
        public int Ignoradas => Filas.Count(f => !f.EsValida);
    }

    private static readonly Dictionary<string, int> Meses = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ene"] = 1, ["enero"] = 1, ["jan"] = 1, ["january"] = 1,
        ["feb"] = 2, ["febrero"] = 2, ["february"] = 2,
        ["mar"] = 3, ["marzo"] = 3, ["march"] = 3,
        ["abr"] = 4, ["abril"] = 4, ["apr"] = 4, ["april"] = 4,
        ["may"] = 5, ["mayo"] = 5,
        ["jun"] = 6, ["junio"] = 6, ["june"] = 6,
        ["jul"] = 7, ["julio"] = 7, ["july"] = 7,
        ["ago"] = 8, ["agosto"] = 8, ["aug"] = 8, ["august"] = 8,
        ["sep"] = 9, ["sept"] = 9, ["septiembre"] = 9, ["setiembre"] = 9, ["september"] = 9,
        ["oct"] = 10, ["octubre"] = 10, ["october"] = 10,
        ["nov"] = 11, ["noviembre"] = 11, ["november"] = 11,
        ["dic"] = 12, ["diciembre"] = 12, ["dec"] = 12, ["december"] = 12,
    };

    public static Resultado Parse(string? texto)
    {
        var filas = new List<Fila>();
        if (string.IsNullOrWhiteSpace(texto)) return new Resultado(filas);

        var lineas = texto.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var crudos = new List<(int Linea, string Texto, DateTime? Fecha, string? PctTexto, string? Error)>();

        for (var i = 0; i < lineas.Length; i++)
        {
            var linea = lineas[i].Trim();
            if (linea.Length == 0) continue;

            var partes = Dividir(linea);
            if (partes.Count < 2)
            {
                crudos.Add((i + 1, linea, null, null, "Se esperan dos columnas: fecha y % real"));
                continue;
            }

            var fecha = ParsearFecha(partes[0]);
            if (fecha is null)
            {
                // Encabezado (Fecha / % real) u otra fila sin fecha: se ignora sin error grave
                var esEncabezado = partes[0].Contains("fecha", StringComparison.OrdinalIgnoreCase)
                                   || partes[1].Contains("%", StringComparison.Ordinal)
                                   && !partes[1].Any(char.IsDigit);
                crudos.Add((i + 1, linea, null, null, esEncabezado ? "Encabezado" : "Fecha no reconocida"));
                continue;
            }

            // El % es la última columna con dígitos (por si pegan también el planificado en medio se toma la 2ª)
            crudos.Add((i + 1, linea, fecha, partes[1], null));
        }

        // Interpretar porcentajes: si ningún valor trae "%" y todos son ≤ 1 → fracciones
        var valores = crudos.Where(c => c.Error is null).Select(c => ParsearNumero(c.PctTexto!)).ToList();
        var conSigno = crudos.Any(c => c.Error is null && c.PctTexto!.Contains('%'));
        var todosFraccion = !conSigno && valores.Count > 0 && valores.All(v => v is null || v <= 1m)
                            && valores.Any(v => v is > 0m);

        foreach (var c in crudos)
        {
            if (c.Error is not null) { filas.Add(new Fila(c.Linea, c.Texto, null, null, c.Error)); continue; }
            var v = ParsearNumero(c.PctTexto!);
            if (v is null) { filas.Add(new Fila(c.Linea, c.Texto, c.Fecha, null, "Porcentaje no reconocido")); continue; }
            var pct = todosFraccion ? v.Value * 100m : v.Value;
            if (pct < 0m || pct > 100.5m) { filas.Add(new Fila(c.Linea, c.Texto, c.Fecha, null, "Porcentaje fuera de 0-100")); continue; }
            filas.Add(new Fila(c.Linea, c.Texto, c.Fecha, Math.Round(Math.Min(pct, 100m), 2), null));
        }

        return new Resultado(filas);
    }

    private static List<string> Dividir(string linea)
    {
        foreach (var sep in new[] { '\t', ';', '|' })
            if (linea.Contains(sep))
                return linea.Split(sep).Select(p => p.Trim()).Where(p => p.Length > 0).ToList();

        // Coma como separador solo si no parece decimal ("41,5")
        if (Regex.IsMatch(linea, @"^[^,]+,\s*[^,]+$") && !Regex.IsMatch(linea, @"\d,\d"))
            return linea.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();

        return Regex.Split(linea, @"\s{1,}").Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
    }

    private static readonly string[] FormatosFecha =
    [
        "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "d/M/yy", "dd-MM-yyyy", "d-M-yyyy", "dd.MM.yyyy",
        "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy HH:mm", "dd/MM/yyyy H:mm:ss", "yyyy-MM-dd HH:mm:ss"
    ];

    public static DateTime? ParsearFecha(string s)
    {
        s = s.Trim().Trim('"');
        if (s.Length == 0) return null;

        if (DateTime.TryParseExact(s, FormatosFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var f))
            return f.Date;

        // d-MMM-yy / d-MMM-yyyy / d MMM yy con mes en texto (9-feb-26, 19-Feb-26, 1 mar 2026)
        var m = Regex.Match(s, @"^(\d{1,2})[\s\-/\.]*([A-Za-zÁÉÍÓÚáéíóú]{3,10})\.?[\s\-/\.]*(\d{2,4})$");
        if (m.Success && Meses.TryGetValue(Quitar(m.Groups[2].Value), out var mes))
        {
            var dia = int.Parse(m.Groups[1].Value);
            var anio = int.Parse(m.Groups[3].Value);
            if (anio < 100) anio += 2000;
            if (dia >= 1 && dia <= DateTime.DaysInMonth(anio, mes)) return new DateTime(anio, mes, dia);
        }

        // Serial de Excel (p. ej. 46062 = 09/02/2026)
        if (Regex.IsMatch(s, @"^\d{5}$") && int.TryParse(s, out var serial) && serial is > 30000 and < 80000)
            return new DateTime(1899, 12, 30).AddDays(serial);

        return null;
    }

    private static string Quitar(string s) => s
        .Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u')
        .Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U');

    public static decimal? ParsearNumero(string s)
    {
        s = s.Trim().Trim('"').Replace("%", "").Replace(" ", "");
        if (s.Length == 0) return null;
        // "1.234,5" → 1234.5 ; "41,5" → 41.5 ; "41.5" → 41.5
        if (s.Contains(',') && s.Contains('.'))
            s = s.LastIndexOf(',') > s.LastIndexOf('.') ? s.Replace(".", "").Replace(',', '.') : s.Replace(",", "");
        else s = s.Replace(',', '.');
        return decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;
    }
}
