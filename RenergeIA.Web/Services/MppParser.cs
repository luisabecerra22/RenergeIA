using MPXJ.Net;

namespace RenergeIA.Web.Services;

public static class MppParser
{
    public sealed record TareaMpp(
        string Codigo, string Nombre, int Nivel, bool EsResumen, bool EsHito,
        DateTime? Inicio, DateTime? Fin, decimal Avance);

    // Plantilla EPC estándar (cronograma Timing Template embebido): se omite la tarea raíz,
    // los niveles suben uno y las fechas se desplazan para que el cronograma arranque en `inicioProyecto`
    public static List<TareaMpp> PlantillaEPC(DateTime inicioProyecto)
    {
        using var recurso = typeof(MppParser).Assembly.GetManifestResourceStream("Plantilla.CronogramaEPC.mpp")
            ?? throw new InvalidOperationException("No se encontró la plantilla de cronograma EPC embebida.");
        using var stream = new MemoryStream();
        recurso.CopyTo(stream);
        stream.Position = 0;
        var tareas = Parse(stream);

        var raices = tareas.Where(t => t.Nivel == 1).ToList();
        if (raices.Count == 1)
            tareas = tareas.Where(t => t.Nivel > 1).Select(t => t with { Nivel = t.Nivel - 1 }).ToList();

        var inicioPlantilla = tareas.Where(t => t.Inicio.HasValue).Min(t => t.Inicio!.Value).Date;
        var desplazamiento = inicioProyecto.Date - inicioPlantilla;
        return tareas.Select(t => t with
        {
            Inicio = t.Inicio?.Date + desplazamiento,
            Fin = t.Fin?.Date + desplazamiento,
            Avance = 0
        }).ToList();
    }

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
