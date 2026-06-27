using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public class HistogramaService(RenergeIADbContext db)
{
    // ── Consultas ──────────────────────────────────────────────────────────────

    public async Task<PlantillaHistograma?> ObtenerAsync(int proyectoId, TipoHistograma tipo)
    {
        return await db.PlantillasHistograma
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId && p.Tipo == tipo);
    }

    public async Task<PlantillaHistograma> CrearOActualizarAsync(int proyectoId, TipoHistograma tipo, List<ItemHistograma> items)
    {
        var existente = await db.PlantillasHistograma
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.ProyectoId == proyectoId && p.Tipo == tipo);

        if (existente is not null)
        {
            db.ItemsHistograma.RemoveRange(existente.Items);
            existente.Items.Clear();
            existente.FechaModificacion = DateTime.UtcNow;
            foreach (var item in items)
            {
                item.PlantillaHistogramaId = existente.Id;
                existente.Items.Add(item);
            }
        }
        else
        {
            existente = new PlantillaHistograma { ProyectoId = proyectoId, Tipo = tipo, Items = items };
            db.PlantillasHistograma.Add(existente);
        }

        await db.SaveChangesAsync();
        return existente;
    }

    // ── Selección automática de plantilla por potencia instalada ───────────────
    //
    // Reglas de asignación:
    //   ≤  5.0 MW  →  Plantilla  1 MW
    //   ≤ 12.5 MW  →  Plantilla 10 MW
    //   ≤ 17.5 MW  →  Plantilla 15 MW
    //   > 17.5 MW  →  Plantilla 20 MW

    public static string NombrePlantilla(decimal capacidadKWp)
    {
        var mw = capacidadKWp / 1000m;
        return mw switch
        {
            <= 5m    => "1 MW",
            <= 12.5m => "10 MW",
            <= 17.5m => "15 MW",
            _        => "20 MW"
        };
    }

    public static List<ItemHistograma> PlantillaPersonalPorCapacidad(decimal capacidadKWp)
    {
        var mw = capacidadKWp / 1000m;
        return mw switch
        {
            <= 5m    => Personal1MW(),
            <= 12.5m => Personal10MW(),
            <= 17.5m => Personal15MW(),
            _        => Personal20MW()
        };
    }

    public static List<ItemHistograma> PlantillaEquiposPorCapacidad(decimal capacidadKWp)
    {
        var mw = capacidadKWp / 1000m;
        return mw switch
        {
            <= 5m    => Equipos1MW(),
            <= 12.5m => Equipos10MW(),
            <= 17.5m => Equipos15MW(),
            _        => Equipos20MW()
        };
    }

    // ── Plantillas de referencia — Personal ────────────────────────────────────

    private static List<ItemHistograma> Personal1MW()  => PersonalConFactor(0.50m);
    private static List<ItemHistograma> Personal10MW() => PersonalConFactor(0.70m);
    private static List<ItemHistograma> Personal15MW() => PersonalConFactor(0.85m);
    private static List<ItemHistograma> Personal20MW() => PersonalConFactor(1.00m);

    // ── Plantillas de referencia — Equipos ────────────────────────────────────

    private static List<ItemHistograma> Equipos1MW()  => EquiposConFactor(0.50m);
    private static List<ItemHistograma> Equipos10MW() => EquiposConFactor(0.70m);
    private static List<ItemHistograma> Equipos15MW() => EquiposConFactor(0.85m);
    private static List<ItemHistograma> Equipos20MW() => EquiposConFactor(1.00m);

    // ── Utilidades ─────────────────────────────────────────────────────────────

    public static decimal[] ValoresMes(ItemHistograma i) =>
        [i.Mes1, i.Mes2, i.Mes3, i.Mes4, i.Mes5, i.Mes6, i.Mes7, i.Mes8, i.Mes9, i.Mes10, i.Mes11, i.Mes12];

    // ── Constructores internos ─────────────────────────────────────────────────

    private static List<ItemHistograma> PersonalConFactor(decimal factor) =>
    [
        Item("Ingeniero Residente",     [1, 1, 1,  1,  1,  1,  1,  1,  1, 1, 1, 1], factor: 1),
        Item("Director de Proyecto",    [0, 1, 1,  1,  1,  1,  1,  1,  1, 1, 1, 0], factor: 1),
        Item("Supervisor HSE",          [1, 1, 1,  1,  1,  1,  1,  1,  1, 1, 1, 1], factor: 1),
        Item("Inspector de Calidad",    [0, 1, 1,  1,  1,  1,  1,  1,  1, 1, 1, 0], factor: 1),
        Item("Capataz Civil",           [0, 1, 2,  2,  2,  2,  1,  1,  0, 0, 0, 0], factor),
        Item("Oficial Civil",           [0, 2, 4,  6,  8,  8,  6,  4,  2, 0, 0, 0], factor),
        Item("Ayudante Civil",          [0, 2, 6, 10, 14, 14, 10,  6,  2, 0, 0, 0], factor),
        Item("Montador Metálico",       [0, 0, 2,  4,  6,  8,  8,  6,  4, 2, 0, 0], factor),
        Item("Electricista Oficial",    [0, 0, 0,  2,  4,  6,  8,  8,  6, 4, 2, 0], factor),
        Item("Electricista Ayudante",   [0, 0, 0,  2,  4,  6,  8,  8,  6, 4, 2, 0], factor),
        Item("Técnico Instrumentación", [0, 0, 0,  0,  0,  2,  2,  2,  2, 2, 1, 0], factor),
        Item("Operador Grúa",           [0, 0, 1,  2,  2,  2,  2,  2,  1, 1, 0, 0], factor),
        Item("Almacenista",             [0, 1, 1,  1,  1,  1,  1,  1,  1, 1, 1, 0], factor: 1),
        Item("Conductor",               [1, 1, 1,  1,  1,  1,  1,  1,  1, 1, 1, 1], factor: 1),
    ];

    private static List<ItemHistograma> EquiposConFactor(decimal factor) =>
    [
        Item("Camioneta 4x4",          [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1], factor: 1),
        Item("Retroexcavadora",        [0, 1, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0], factor),
        Item("Minicargador",           [0, 1, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0], factor),
        Item("Grúa Telescópica",       [0, 0, 1, 1, 2, 2, 2, 2, 1, 1, 0, 0], factor),
        Item("Volqueta",               [0, 1, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0], factor),
        Item("Plataforma Elevadora",   [0, 0, 0, 1, 1, 2, 2, 2, 1, 1, 0, 0], factor),
        Item("Generador Eléctrico",    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0], factor: 1),
        Item("Compresor de Aire",      [0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0], factor),
        Item("Soldadora",              [0, 0, 1, 2, 2, 2, 2, 2, 1, 1, 0, 0], factor),
        Item("Equipo de Izaje Manual", [0, 0, 1, 1, 2, 2, 2, 2, 1, 1, 0, 0], factor),
    ];

    private static ItemHistograma Item(string nombre, decimal[] meses, decimal factor = 1) => new()
    {
        Nombre = nombre,
        Mes1   = Math.Round(meses[0]  * factor),
        Mes2   = Math.Round(meses[1]  * factor),
        Mes3   = Math.Round(meses[2]  * factor),
        Mes4   = Math.Round(meses[3]  * factor),
        Mes5   = Math.Round(meses[4]  * factor),
        Mes6   = Math.Round(meses[5]  * factor),
        Mes7   = Math.Round(meses[6]  * factor),
        Mes8   = Math.Round(meses[7]  * factor),
        Mes9   = Math.Round(meses[8]  * factor),
        Mes10  = Math.Round(meses[9]  * factor),
        Mes11  = Math.Round(meses[10] * factor),
        Mes12  = Math.Round(meses[11] * factor),
    };

    // ── Histograma Real ────────────────────────────────────────────────────────

    public async Task<HistogramaReal?> ObtenerRealAsync(int proyectoId, TipoHistograma tipo)
    {
        return await db.HistogramasReales
            .Include(h => h.Items)
            .FirstOrDefaultAsync(h => h.ProyectoId == proyectoId && h.Tipo == tipo);
    }

    public async Task<HistogramaReal> CrearOActualizarRealAsync(int proyectoId, TipoHistograma tipo,
                                                                List<ItemHistogramaReal> items)
    {
        var existente = await db.HistogramasReales
            .Include(h => h.Items)
            .FirstOrDefaultAsync(h => h.ProyectoId == proyectoId && h.Tipo == tipo);

        if (existente is not null)
        {
            db.ItemsHistogramaReal.RemoveRange(existente.Items);
            existente.Items.Clear();
            existente.FechaModificacion = DateTime.UtcNow;
            foreach (var item in items)
            {
                item.HistogramaRealId = existente.Id;
                existente.Items.Add(item);
            }
        }
        else
        {
            existente = new HistogramaReal { ProyectoId = proyectoId, Tipo = tipo, Items = items };
            db.HistogramasReales.Add(existente);
        }

        await db.SaveChangesAsync();
        return existente;
    }

    public async Task<ComparativoHistogramaVM> ObtenerComparativoAsync(int proyectoId,
                                                                       TipoHistograma tipo,
                                                                       int mesInicial,
                                                                       int anioInicial)
    {
        var plantilla = await ObtenerAsync(proyectoId, tipo);
        var real      = await ObtenerRealAsync(proyectoId, tipo);

        var nombresMes = new[] { "Ene","Feb","Mar","Abr","May","Jun",
                                 "Jul","Ago","Sep","Oct","Nov","Dic" };

        var labels       = new string[12];
        var planificados = new double[12];
        var reales       = new double[12];
        var diferencias  = new double[12];
        var pctDesv      = new double[12];

        for (int pos = 1; pos <= 12; pos++)
        {
            int offset = mesInicial - 1 + pos - 1;
            int mes    = offset % 12 + 1;
            int anio   = anioInicial + offset / 12;
            labels[pos - 1] = nombresMes[mes - 1] + " " + (anio % 100).ToString("D2");
            planificados[pos - 1] = (double)(plantilla?.Items.Sum(i => ValoresMesDecimal(i, pos)) ?? 0);
            reales[pos - 1]       = (double)(real?.Items.Sum(i => ValorMesReal(i, pos)) ?? 0);
            diferencias[pos - 1]  = reales[pos - 1] - planificados[pos - 1];

            pctDesv[pos - 1] = planificados[pos - 1] != 0
                ? Math.Round((diferencias[pos - 1] / planificados[pos - 1]) * 100, 1)
                : reales[pos - 1] > 0 ? 100.0 : 0.0;
        }

        double totalPlan = planificados.Sum();
        double totalReal = reales.Sum();
        double difAcum   = totalReal - totalPlan;
        double pctGlobal = totalPlan != 0 ? Math.Round((difAcum / totalPlan) * 100, 1) : 0;

        int idxMax = 0;
        for (int i = 1; i < 12; i++)
            if (Math.Abs(diferencias[i]) > Math.Abs(diferencias[idxMax])) idxMax = i;

        return new ComparativoHistogramaVM
        {
            Labels             = labels,
            Planificados       = planificados,
            Reales             = reales,
            Diferencias        = diferencias,
            PctDesviacion      = pctDesv,
            TotalPlanificado   = (decimal)totalPlan,
            TotalReal          = (decimal)totalReal,
            DiferenciaAcumulada = (decimal)difAcum,
            PctDesviacionGlobal = (decimal)pctGlobal,
            MesMayorDesviacion  = labels[idxMax]
        };
    }

    private static decimal ValoresMesDecimal(ItemHistograma i, int pos) => pos switch
    {
        1 => i.Mes1,  2 => i.Mes2,  3 => i.Mes3,  4 => i.Mes4,
        5 => i.Mes5,  6 => i.Mes6,  7 => i.Mes7,  8 => i.Mes8,
        9 => i.Mes9, 10 => i.Mes10, 11 => i.Mes11, 12 => i.Mes12,
        _ => 0
    };

    private static decimal ValorMesReal(ItemHistogramaReal i, int pos) => pos switch
    {
        1 => i.Mes1,  2 => i.Mes2,  3 => i.Mes3,  4 => i.Mes4,
        5 => i.Mes5,  6 => i.Mes6,  7 => i.Mes7,  8 => i.Mes8,
        9 => i.Mes9, 10 => i.Mes10, 11 => i.Mes11, 12 => i.Mes12,
        _ => 0
    };
}

public class ComparativoHistogramaVM
{
    public string[] Labels              { get; set; } = [];
    public double[] Planificados        { get; set; } = [];
    public double[] Reales              { get; set; } = [];
    public double[] Diferencias         { get; set; } = [];
    public double[] PctDesviacion       { get; set; } = [];
    public decimal  TotalPlanificado    { get; set; }
    public decimal  TotalReal           { get; set; }
    public decimal  DiferenciaAcumulada { get; set; }
    public decimal  PctDesviacionGlobal { get; set; }
    public string   MesMayorDesviacion  { get; set; } = "";
}
