using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Infrastructure.Data;
using static RenergeIA.Web.Services.CodificacionParser;

namespace RenergeIA.Web.Services;

public static class CodificacionSeeder
{
    // Presupuestos de La Soberana (COS5SO) — fuente: Codificación.xlsx (PPTO COP y PPTO USD).
    // La estructura de categorías/códigos sale del CatalogoMaestro; estos valores llenan los códigos que coinciden.
    private static readonly Dictionary<string, decimal> _pptoCop = new(StringComparer.OrdinalIgnoreCase)
    {
        {"SCSP",16858800.00m},{"VASP",92335667.36m},{"SPSP",239447083.20m},{"IRSP",19880000.00m},
        {"SDSP",19526400.00m},{"VPSP",153944427.06m},{"MTTC",15260000.00m},{"BTTC",9940000.00m},
        {"SOTC",15498000.00m},{"CMTC",1428000.00m},{"PAIM",27982194.64m},{"ESIM",35201723.33m},
        {"ESTC",338489651.14m},{"CTIM",29660130.91m},{"PAIE",2548980.00m},{"MTIE",9824466.63m},
        {"BTIE",1539781.25m},{"SOIE",12060851.16m},{"FCIE",19823920.93m},{"SIIM",43095113.10m},
        {"MTIM",47716425.61m},{"BTIM",55004478.82m},{"SOIM",33051262.60m},{"CMIM",29452036.36m},
        {"UPIM",9045863.07m},{"TBIM",2402500.00m},{"CCIM",24566472.00m},{"CCTC",11900000.00m},
        {"SCIM",12062724.00m},{"SCIE",4524659.98m},{"EMIM",6446400.00m},{"CGPE",9000000.00m},
        {"CGIN",22491000.00m},{"CGES",87399102.00m},{"CGPO",62039459.23m},{"CGST",224405517.18m},
        {"CGPR",19000000.00m},{"CGCG",290443553.34m},{"CGHS",28720000.00m},{"CGHC",89793311.67m},
    };

    private static readonly Dictionary<string, decimal> _pptoUsd = new(StringComparer.OrdinalIgnoreCase)
    {
        {"MTSP",19177.01m},{"BTSP",41544.45m},{"SOSP",13281.48m},{"CMSP",720.63m},{"CCSP",552.02m},
        {"PTSP",5724.47m},{"CASP",303.60m},{"DSSP",9523.81m},{"VASP",103571.43m},
    };

    private static List<RubroCodificacion> ConstruirLaSoberana()
    {
        var lista = new List<RubroCodificacion>();
        foreach (var (categoria, codigo, descripcion) in CostoService.CatalogoMaestro)
        {
            var tieneCop = _pptoCop.TryGetValue(codigo, out var vCop);
            var tieneUsd = _pptoUsd.TryGetValue(codigo, out var vUsd);
            if (tieneCop || !tieneUsd)
                lista.Add(new RubroCodificacion("COP", codigo, descripcion, categoria, tieneCop ? vCop : 0m));
            if (tieneUsd)
                lista.Add(new RubroCodificacion("USD", codigo, descripcion, categoria, vUsd));
        }
        return lista;
    }

    // Mapa código de tesorería → rubro (Codificación.xlsx, hojas COD COP y COD USD)
    public static readonly Dictionary<string, string> MapaCodigoARubro = new(StringComparer.OrdinalIgnoreCase)
    {
        {"ACSH","CAIE"},{"BTIM","BTIM"},{"BTSB","BTIM"},{"BTSC","BTTC"},{"BTSH","BTIM"},{"BTSM","BTIE"},{"BTSP","BTSP"},
        {"BTST","BTIM"},{"BTTC","BTTC"},{"BTTU","BTIE"},{"CASP","CASP"},{"CCSC","CCIM"},{"CCSP","CCSP"},{"CCST","CCIM"},
        {"CCTC","CCTC"},{"CGAC","CGPR"},{"CGAL","CGST"},{"CGAO","CGCG"},{"CGAP","CGCG"},{"CGAY","CGST"},{"CGCA","CGHS"},
        {"CGCC","CGCG"},{"CGCG","CGCG"},{"CGCGV5","CGCG"},{"CGCI","CGST"},{"CGCM","CGCG"},{"CGCN","CGCG"},{"CGCO","CGPR"},
        {"CGCR","CGCG"},{"CGDM","CGCG"},{"CGDO","CGST"},{"CGEC","CGCG"},{"CGEL","CGST"},{"CGEO","CGCG"},{"CGES","CGES"},
        {"CGHC","CGHC"},{"CGHE","CGST"},{"CGHS","CGST"},{"CGIN","CGIN"},{"CGLE","CGCG"},{"CGLQ","CGST"},{"CGMA","CGHS"},
        {"CGME","CGST"},{"CGMM","CGCG"},{"CGOM","CGST"},{"CGPE","CGPE"},{"CGPR","CGPR"},{"CGPRM","CGPRM"},{"CGQA","CGST"},
        {"CGRC","CGCG"},{"CGRS","CGES"},{"CGSA","CGCG"},{"CGSC","CGCG"},{"CGSI","CGCG"},{"CGSS","CGST"},{"CGSTM","CGSTM"},
        {"CGSV","CGCG"},{"CGTM","CGCG"},{"CGTP","CGST"},{"CGTR","CGCG"},{"CGVI","CGCG"},{"CGVP","CGCG"},{"CGVR","CGCG"},
        {"CMFO","CMIE"},{"CMIM","CMIM"},{"CMSC","CMTC"},{"CMSP","CMSP"},{"CMST","CMIM"},{"CTAG","CTIM"},{"CTSH","CTIM"},
        {"DSSP","DSSP"},{"DSTM","DSSP"},{"EMIM","EMIM"},{"ESBA","ESTC"},{"ESCO","ESTC"},{"ESDC","ESTC"},{"ESET","ESTC"},
        {"ESHI","ESTC"},{"ESIM","ESIM"},{"ESSH","EMIM"},{"ESTC","ESTC"},{"ESTCS","ESTC"},{"ESTH","ESIM"},{"ESTL","ESTC"},
        {"ESTM","ESTC"},{"FCIE","FCIE"},{"HSDO","CGHS"},{"HSEP","CGHS"},{"HSMT","CGHS"},{"IRSP","IRSP"},{"LTSP","LTSP"},
        {"MABS","CGHS"},{"MACA","CGHS"},{"MADR","CGHS"},{"MAPS","CGHS"},{"MTIE","MTIE"},{"MTIM","MTIM"},{"MTME","MTSP"},
        {"MTPP","CGHS"},{"MTSC","MTTC"},{"MTSH","MTIM"},{"MTSP","MTSP"},{"MTST","MTIM"},{"MTTC","MTTC"},{"MTTU","MTIE"},
        {"PADE","PAIM"},{"PAIE","PAIE"},{"PAIM","PAIM"},{"PASH","PAIM"},{"PECI","CGHS"},{"PEEB","CGHS"},{"PEPA","CGHS"},
        {"PESÑ","CGHS"},{"POCS","CGPO"},{"POCU","CGPO"},{"POMA","CGPO"},{"PORC","CGPO"},{"POSP","CGPO"},{"POTR","CGPO"},
        {"PTIE","PTIE"},{"PTPF","PTIE"},{"PTSP","PTSP"},{"SCIE","SCIE"},{"SCIM","SCIM"},{"SCSH","SCIM"},{"SCSP","SCSP"},
        {"SDAV","SDSP"},{"SDCG","SDSP"},{"SDSC","SDSP"},{"SIIM","SIIM"},{"SIMT","VASP"},{"SISH","SIIM"},{"SOIE","SOIE"},
        {"SOIM","SOIM"},{"SOSB","SOIM"},{"SOSH","SOIM"},{"SOSM","SOIE"},{"SOSP","SOSP"},{"SOST","SOIM"},{"SOTC","SOTC"},
        {"SPAB","SPSP"},{"SPAP","SPSP"},{"SPAV","SPSP"},{"SPCM","SPSP"},{"SPET","SPSP"},{"SPMT","MTSP"},{"SPPT","PTSP"},
        {"SPSC","SPSP"},{"TBIM","TBIM"},{"UPIM","UPIM"},{"UPSH","UPIM"},{"VASP","VASP"},{"VPCH","VPSP"},{"VPMO","VPSP"},
        {"VPMP","VPSP"},{"VPSC","VPSP"},
    };

    // Categoría principal a la que pertenece un código (según el catálogo maestro)
    public static string CategoriaDe(string codigo)
    {
        var entrada = CostoService.CatalogoMaestro
            .FirstOrDefault(c => string.Equals(c.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
        return entrada.Codigo is not null ? entrada.Categoria : "Costos generales";
    }

    // Repara rubros que el importador de tesorería creó como categorías principales:
    // los reubica como sub-códigos dentro de su categoría correspondiente
    public static async Task RepararRubrosTesoreriaAsync(RenergeIADbContext db)
    {
        var huerfanas = await db.Partidas
            .Where(p => p.EsPrincipal && p.Descripcion.EndsWith("(tesorería)"))
            .ToListAsync();
        if (huerfanas.Count == 0) return;

        foreach (var grupo in huerfanas.GroupBy(h => h.ProyectoId))
        {
            var principales = await db.Partidas
                .Where(p => p.ProyectoId == grupo.Key && p.EsPrincipal)
                .ToListAsync();
            var subs = await db.Partidas
                .Where(p => p.ProyectoId == grupo.Key && !p.EsPrincipal)
                .ToListAsync();

            foreach (var h in grupo)
            {
                var categoria = CategoriaDe(h.Codigo);
                var padre = principales.FirstOrDefault(p =>
                    !p.Descripcion.EndsWith("(tesorería)") &&
                    string.Equals(p.Descripcion.Trim(), categoria, StringComparison.OrdinalIgnoreCase));
                if (padre is null) continue;

                h.EsPrincipal = false;
                h.Nivel = 2;
                h.PadreId = padre.Id;
                h.Numero = $"{padre.Numero}.{subs.Count(s => s.PadreId == padre.Id) + 1}";
                subs.Add(h);
                db.Partidas.Update(h);
            }
        }
        await db.SaveChangesAsync();
    }

    // Migra a la plantilla nueva (catálogo maestro) los proyectos que aún tengan la
    // plantilla vieja con consecutivos "-001" (decisión de la usuaria 2026-09-15)
    public static async Task ActualizarPlantillasViejasAsync(RenergeIADbContext db)
    {
        var proyectosViejos = await db.Partidas
            .Where(p => !p.EsPrincipal && p.Codigo.Contains("-"))
            .Select(p => p.ProyectoId)
            .Distinct()
            .ToListAsync();
        if (proyectosViejos.Count == 0) return;

        var svc = new CostoService(db);
        foreach (var proyectoId in proyectosViejos)
            await svc.CargarPlantillaEstandarAsync(proyectoId);
    }

    // Agrega a los proyectos existentes los códigos nuevos del catálogo maestro que les falten,
    // dentro de su categoría y al final de la numeración (no toca valores existentes)
    public static async Task AgregarCodigosFaltantesCatalogoAsync(RenergeIADbContext db)
    {
        var principales = await db.Partidas.Where(p => p.EsPrincipal).ToListAsync();
        var subs = await db.Partidas.Where(p => !p.EsPrincipal).ToListAsync();
        var huboCambios = false;

        foreach (var proyectoId in principales.Select(p => p.ProyectoId).Distinct())
        {
            var subsProyecto = subs.Where(s => s.ProyectoId == proyectoId).ToList();
            // Solo proyectos que ya usan el catálogo maestro (tienen códigos sin consecutivo "-")
            if (!subsProyecto.Any(s => !s.Codigo.Contains('-'))) continue;

            foreach (var grupo in CostoService.CatalogoMaestro.GroupBy(c => c.Categoria))
            {
                var padre = principales.FirstOrDefault(p => p.ProyectoId == proyectoId &&
                    string.Equals(p.Descripcion.Trim(), grupo.Key, StringComparison.OrdinalIgnoreCase));
                if (padre is null) continue;

                foreach (var (_, codigo, descripcion) in grupo)
                {
                    if (subsProyecto.Any(s => s.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))) continue;

                    var siguiente = subsProyecto
                        .Where(s => s.PadreId == padre.Id)
                        .Select(s => int.TryParse(s.Numero?.Split('.').LastOrDefault(), out var n) ? n : 0)
                        .DefaultIfEmpty(0).Max() + 1;

                    var nueva = new Partida
                    {
                        ProyectoId = proyectoId,
                        PadreId = padre.Id,
                        Numero = $"{padre.Numero}.{siguiente}",
                        Codigo = codigo,
                        Descripcion = descripcion,
                        Unidad = "Global",
                        CantidadPresupuestada = 1,
                        PrecioUnitario = 0,
                        MonedaOriginal = "COP",
                        Nivel = 2,
                        EsPrincipal = false
                    };
                    db.Partidas.Add(nueva);
                    subsProyecto.Add(nueva);
                    huboCambios = true;
                }
            }
        }

        if (huboCambios) await db.SaveChangesAsync();
    }

    public static async Task SeedLaSoberanaAsync(RenergeIADbContext db)
    {
        var proyecto = await db.Proyectos.FirstOrDefaultAsync(p => p.Codigo == "COS5SO");
        if (proyecto is null) return;

        // Ya actualizado a la estructura del catálogo maestro si existe el código marcador CGAF
        var yaActualizado = await db.Partidas.AnyAsync(p =>
            p.ProyectoId == proyecto.Id && !p.EsPrincipal && p.Codigo == "CGAF");
        if (yaActualizado) return;

        var svc = new CostoService(db);
        await svc.CargarDesdeCodificacionAsync(proyecto.Id, ConstruirLaSoberana());
    }
}
