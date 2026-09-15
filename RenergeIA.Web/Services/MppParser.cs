using MPXJ.Net;

namespace RenergeIA.Web.Services;

public static class MppParser
{
    public sealed record TareaMpp(
        string Codigo, string Nombre, int Nivel, bool EsResumen, bool EsHito,
        DateTime? Inicio, DateTime? Fin, decimal Avance);

    public static List<TareaMpp> Parse(Stream stream)
    {
        var reader = new UniversalProjectReader();
        var project = reader.Read(stream);

        var lista = new List<TareaMpp>();
        foreach (var t in project.Tasks)
        {
            if (string.IsNullOrWhiteSpace(t.Name)) continue;
            var nivel = t.OutlineLevel ?? 0;
            if (nivel < 1) continue; // se omite la tarea raíz del proyecto

            decimal avance = 0;
            if (t.PercentageComplete is not null)
                avance = Math.Clamp(Convert.ToDecimal(t.PercentageComplete), 0m, 100m);

            lista.Add(new TareaMpp(
                (t.OutlineNumber ?? "").Trim(),
                t.Name.Trim(),
                nivel,
                t.Summary,
                t.Milestone,
                t.Start,
                t.Finish,
                avance));
        }
        return lista;
    }
}
