using ClosedXML.Excel;

namespace RenergeIA.Web.Services;

public static class CodificacionParser
{
    public sealed record RubroCodificacion(string Moneda, string Rubro, string Actividad, string Disciplina, decimal Presupuesto);

    private static readonly string[] _excluidos = ["CMIE", "PTIE"];

    public static List<RubroCodificacion> Parse(XLWorkbook wb)
    {
        var discCOP = LeerDisciplinas(wb, "COD COP");
        var discUSD = LeerDisciplinas(wb, "COD USD");

        var resultado = new List<RubroCodificacion>();
        resultado.AddRange(LeerPresupuesto(wb, "PPTO COP", "COP", discCOP));
        resultado.AddRange(LeerPresupuesto(wb, "PPTO USD", "USD", discUSD));
        return resultado;
    }

    private static Dictionary<string, string> LeerDisciplinas(XLWorkbook wb, string hoja)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var ws = wb.Worksheets.FirstOrDefault(w => string.Equals(w.Name.Trim(), hoja, StringComparison.OrdinalIgnoreCase));
        if (ws is null) return dict;

        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        for (int r = 2; r <= lastRow; r++)
        {
            var rubro = ws.Cell(r, 2).GetString().Trim().ToUpperInvariant();
            var disciplina = ws.Cell(r, 4).GetString().Trim();
            if (string.IsNullOrEmpty(rubro) || string.IsNullOrEmpty(disciplina)) continue;
            if (!dict.ContainsKey(rubro)) dict[rubro] = disciplina;
        }
        return dict;
    }

    private static List<RubroCodificacion> LeerPresupuesto(XLWorkbook wb, string hoja, string moneda,
        Dictionary<string, string> disciplinas)
    {
        var resultado = new List<RubroCodificacion>();
        var ws = wb.Worksheets.FirstOrDefault(w => string.Equals(w.Name.Trim(), hoja, StringComparison.OrdinalIgnoreCase));
        if (ws is null) return resultado;

        var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        for (int r = 2; r <= lastRow; r++)
        {
            var rubro = ws.Cell(r, 1).GetString().Trim().ToUpperInvariant();
            var actividad = ws.Cell(r, 2).GetString().Trim();
            var presupuesto = BomParser.Num(ws.Cell(r, 3)) ?? 0;

            if (string.IsNullOrEmpty(rubro) || string.IsNullOrEmpty(actividad)) continue;
            if (_excluidos.Contains(rubro)) continue;
            if (!vistos.Add(rubro)) continue;

            var disciplina = disciplinas.TryGetValue(rubro, out var d) ? d : "Otros";
            resultado.Add(new RubroCodificacion(moneda, rubro, actividad, disciplina, presupuesto));
        }
        return resultado;
    }

    public static Dictionary<string, string> MapaCodigoARubro(XLWorkbook wb)
    {
        var mapa = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var hoja in new[] { "COD COP", "COD USD" })
        {
            var ws = wb.Worksheets.FirstOrDefault(w => string.Equals(w.Name.Trim(), hoja, StringComparison.OrdinalIgnoreCase));
            if (ws is null) continue;
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            for (int r = 2; r <= lastRow; r++)
            {
                var codigo = ws.Cell(r, 1).GetString().Trim().ToUpperInvariant();
                var rubro = ws.Cell(r, 2).GetString().Trim().ToUpperInvariant();
                if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(rubro)) continue;
                if (!mapa.ContainsKey(codigo)) mapa[codigo] = rubro;
            }
        }
        return mapa;
    }
}
