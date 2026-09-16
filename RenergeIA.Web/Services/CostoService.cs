using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public class CostoService(RenergeIADbContext db)
{
    public async Task<List<Partida>> ObtenerPartidasConCostosAsync(int proyectoId)
    {
        return await db.Partidas
            .Include(p => p.CostosReales)
            .Where(p => p.ProyectoId == proyectoId)
            .OrderBy(p => p.Nivel)
            .ThenBy(p => p.Numero)
            .ThenBy(p => p.Codigo)
            .ToListAsync();
    }

    public static decimal CostoEjecutado(Partida p)
        => p.CostosReales.Sum(c => c.Cantidad * c.PrecioUnitario);

    public static decimal PorcentajeEjecucion(Partida p)
    {
        var presup = p.CantidadPresupuestada * p.PrecioUnitario;
        if (presup == 0) return 0;
        return Math.Round(CostoEjecutado(p) / presup * 100, 1);
    }

    // Ejecutado y Comprometido de cada código salen del Flujo de Caja, respetando la moneda de la partida.
    // Usa ExecuteUpdate para no arrastrar cambios pendientes (sin guardar) del contexto compartido.
    public async Task SincronizarEjecutadoDesdeFlujoAsync(int proyectoId)
    {
        var proyecto = await db.Proyectos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == proyectoId);
        var trm = proyecto is { TasaCambioCOPUSD: > 0 } ? proyecto.TasaCambioCOPUSD : 4000m;

        var partidas = await db.Partidas.Where(p => p.ProyectoId == proyectoId).ToListAsync();
        var porId = partidas.ToDictionary(p => p.Id);
        var codigos = partidas.Where(p => !p.EsPrincipal && !p.Codigo.Contains('-')).ToList();

        // Código de flujo al que pertenece un pago (subdetalles "-001" suben a su código padre)
        string? CodigoDe(int partidaId)
        {
            if (!porId.TryGetValue(partidaId, out var pa) || pa.EsPrincipal) return null;
            if (!pa.Codigo.Contains('-')) return pa.Codigo.ToUpperInvariant();
            return pa.PadreId is int padre && porId.TryGetValue(padre, out var pp) && !pp.EsPrincipal
                ? pp.Codigo.ToUpperInvariant() : null;
        }

        // Corte = lunes de la semana en curso (hora Colombia).
        // Ejecutado    = flujo ANTES del corte (ya pagado, hasta la semana anterior)
        // Comprometido = flujo DESDE el corte en adelante (por pagar)
        var limite = InicioSemanaActual();
        var pagos = (await db.PagosCorteSemanal.AsNoTracking()
                .Where(p => p.ProyectoId == proyectoId)
                .Select(p => new { p.PartidaId, p.Monto, p.Moneda, p.FechaCorte })
                .ToListAsync())
            .Select(p => new { Codigo = CodigoDe(p.PartidaId), p.Monto, Moneda = p.Moneda == "USD" ? "USD" : "COP", Pasado = p.FechaCorte < limite })
            .Where(p => p.Codigo is not null)
            .ToList();

        var totEjec = pagos.Where(p => p.Pasado).GroupBy(p => (p.Codigo!, p.Moneda)).ToDictionary(g => g.Key, g => g.Sum(x => x.Monto));
        var totComp = pagos.Where(p => !p.Pasado).GroupBy(p => (p.Codigo!, p.Moneda)).ToDictionary(g => g.Key, g => g.Sum(x => x.Monto));

        foreach (var p in codigos)
        {
            var codigo = p.Codigo.ToUpperInvariant();
            var hermanos = codigos.Where(c => c.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)).ToList();
            var monedaPropia = p.MonedaOriginal == "USD" ? "USD" : "COP";
            var otraMoneda = monedaPropia == "USD" ? "COP" : "USD";
            var esPrimeraDeSuMoneda = hermanos.First(h => (h.MonedaOriginal == "USD" ? "USD" : "COP") == monedaPropia).Id == p.Id;
            var codigoSoloEnSuMoneda = !hermanos.Any(h => (h.MonedaOriginal == "USD" ? "USD" : "COP") == otraMoneda);

            // Pagos en la moneda de la partida: directo. Pagos en la otra moneda: solo si el código
            // NO tiene una partida en esa moneda (si la tiene, le pertenecen a ella); se convierten con la TRM.
            decimal Calcular(Dictionary<(string, string), decimal> tot)
            {
                if (!esPrimeraDeSuMoneda) return 0m;
                var valor = tot.GetValueOrDefault((codigo, monedaPropia));
                if (codigoSoloEnSuMoneda)
                {
                    var otro = tot.GetValueOrDefault((codigo, otraMoneda));
                    valor += monedaPropia == "USD" ? otro / trm : otro * trm;
                }
                return Math.Round(valor, 2);
            }

            var ejec = Calcular(totEjec);
            var comp = Calcular(totComp);
            if (p.ValorEjecutado == ejec && p.MontoComprometido == comp) continue;

            await db.Partidas.Where(x => x.Id == p.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.ValorEjecutado, ejec)
                    .SetProperty(x => x.MontoComprometido, comp));

            var entry = db.Entry(p);
            foreach (var (nombre, valor) in new[] { (nameof(Partida.ValorEjecutado), ejec), (nameof(Partida.MontoComprometido), comp) })
            {
                var prop = entry.Property(nombre);
                prop.CurrentValue = valor;
                prop.OriginalValue = valor;
                prop.IsModified = false;
            }
        }
    }

    // Lunes de la semana en curso en hora de Colombia (UTC-5)
    public static DateTime InicioSemanaActual()
    {
        var hoy = DateTime.UtcNow.AddHours(-5).Date;
        return hoy.AddDays(-(((int)hoy.DayOfWeek + 6) % 7));
    }

    private async Task EliminarPartidasProyectoAsync(int proyectoId)
    {
        var pagos = await db.PagosCorteSemanal.Where(p => p.ProyectoId == proyectoId).ToListAsync();
        if (pagos.Count > 0) db.PagosCorteSemanal.RemoveRange(pagos);
        var exclusiones = await db.FlujoCajaExclusiones.Where(e => e.ProyectoId == proyectoId).ToListAsync();
        if (exclusiones.Count > 0) db.FlujoCajaExclusiones.RemoveRange(exclusiones);
        var todas = await db.Partidas.Where(p => p.ProyectoId == proyectoId).ToListAsync();
        if (todas.Count > 0) db.Partidas.RemoveRange(todas);
        await db.SaveChangesAsync();
    }

    public async Task CargarDesdeCodificacionAsync(int proyectoId, List<CodificacionParser.RubroCodificacion> rubros)
    {
        await EliminarPartidasProyectoAsync(proyectoId);

        // Se respeta el orden de llegada (el orden del catálogo / archivo)
        var categorias = rubros.GroupBy(r => r.Disciplina).ToList();

        int numCat = 0;
        foreach (var grupo in categorias)
        {
            numCat++;
            var principal = new Partida
            {
                ProyectoId = proyectoId,
                Numero = numCat.ToString(),
                Codigo = CodigoDisciplina(grupo.Key),
                Descripcion = grupo.Key.ToUpperInvariant(),
                Unidad = "Global",
                Nivel = 1,
                EsPrincipal = true
            };
            db.Partidas.Add(principal);
            await db.SaveChangesAsync();

            int numSub = 0;
            foreach (var r in grupo)
            {
                numSub++;
                db.Partidas.Add(new Partida
                {
                    ProyectoId = proyectoId,
                    PadreId = principal.Id,
                    Numero = $"{numCat}.{numSub}",
                    Codigo = r.Rubro,
                    Descripcion = r.Actividad,
                    Unidad = "Global",
                    CantidadPresupuestada = 1,
                    PrecioUnitario = r.Presupuesto,
                    MonedaOriginal = r.Moneda,
                    Nivel = 2,
                    EsPrincipal = false
                });
            }
            await db.SaveChangesAsync();
        }
    }

    public static string CodigoDisciplina(string disciplina)
    {
        var norm = disciplina.Trim().ToUpperInvariant();
        return norm switch
        {
            "SUMINISTROS PRINCIPALES"     => "SUM",
            "TRABAJOS CIVILES"            => "TC",
            "INSTALACIÓN MECÁNICA"        => "IM",
            "HOT COMMISSIONING"           => "HC",
            "INSTALACIÓN ELÉCTRICA"       => "IE",
            "CCTV"                        => "CCTV",
            "SCADA"                       => "SCADA",
            "ESTACIONES METEOROLÓGICAS"   => "EM",
            "COSTOS GENERALES"            => "CG",
            "ESTUDIOS"                    => "EST",
            _ => DerivarIniciales(disciplina)
        };
    }

    private static string DerivarIniciales(string disciplina)
    {
        var palabras = disciplina.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(p => p.Length > 2 || p.ToUpperInvariant() == p)
            .ToArray();
        if (palabras.Length == 0) return "OTR";
        var iniciales = string.Concat(palabras.Select(p => char.ToUpperInvariant(p[0])));
        return iniciales.Length >= 2 ? iniciales[..Math.Min(4, iniciales.Length)]
                                     : palabras[0][..Math.Min(3, palabras[0].Length)].ToUpperInvariant();
    }

    public async Task CargarPlantillaEstandarAsync(int proyectoId)
    {
        var existentes = await db.Partidas.AnyAsync(p => p.ProyectoId == proyectoId);
        if (existentes)
        {
            await EliminarPartidasProyectoAsync(proyectoId);
        }

        var rubros = CatalogoMaestro
            .Select(c => new CodificacionParser.RubroCodificacion("COP", c.Codigo, c.Descripcion, c.Categoria, 0m))
            .ToList();
        await CargarDesdeCodificacionAsync(proyectoId, rubros);
    }

    // Catálogo maestro de códigos del módulo de costos (listado definido por la usuaria 2026-09-14).
    // Estos son los códigos que salen en Presupuesto y Flujo de Caja.
    public static readonly (string Categoria, string Codigo, string Descripcion)[] CatalogoMaestro =
    [
        ("Suministros principales", "PASP", "Suministro de Paneles"),
        ("Suministros principales", "ESSP", "Suministro de Estructuras"),
        ("Suministros principales", "CTSP", "Suministro de Centro de transformación"),
        ("Suministros principales", "CISP", "Suministro de Centro de Inversión"),
        ("Suministros principales", "DSSP", "Suministro de Estación de Entrega"),
        ("Suministros principales", "SISP", "Suministro de String inverter"),
        ("Suministros principales", "SBSP", "Suministro de String box"),
        ("Suministros principales", "MTSP", "Suministro de Media tensión"),
        ("Suministros principales", "BTSP", "Suministro de Baja tensión"),
        ("Suministros principales", "SOSP", "Suministro de Solar"),
        ("Suministros principales", "CMSP", "Suministro de Comunicación"),
        ("Suministros principales", "CCSP", "Suministro de CCTV"),
        ("Suministros principales", "EMSP", "Suministro de Estación meterológica"),
        ("Suministros principales", "SCSP", "Suministro de Sistema SCADA"),
        ("Suministros principales", "PTSP", "Suministro de Sistema puesta a tierra"),
        ("Suministros principales", "UPSP", "Suministro de Sistema UPS"),
        ("Suministros principales", "SHSP", "Suministro de Contenedores"),
        ("Suministros principales", "TBSP", "Suministro de Tableros baja tensión"),
        ("Suministros principales", "CASP", "Suministro de Accesorios cableado"),
        ("Suministros principales", "VASP", "Suministro de Varios-Cimentaciones"),
        ("Suministros principales", "LTSP", "Suministro de Línea de Media Tensión"),

        ("Trabajos civiles", "SPSP", "Suministro de Preparación de sitio"),
        ("Trabajos civiles", "IRSP", "Suministro de Caminos internos"),
        ("Trabajos civiles", "SDSP", "Suministro de Sistema de drenaje"),
        ("Trabajos civiles", "VPSP", "Suministro de Vallado perimetral"),
        ("Trabajos civiles", "CTTC", "Trabajo civil de Centro de transformación"),
        ("Trabajos civiles", "CITC", "Trabajo civil de Centro de inversión"),
        ("Trabajos civiles", "DSTC", "Trabajo civil de Estación de Entrega"),
        ("Trabajos civiles", "SITC", "Trabajo civil de String inverter"),
        ("Trabajos civiles", "SBTC", "Trabajo civil de String box"),
        ("Trabajos civiles", "MTTC", "Trabajo civil de Media tensión"),
        ("Trabajos civiles", "BTTC", "Trabajo civil de Baja tensión"),
        ("Trabajos civiles", "SOTC", "Trabajo civil de Solar"),
        ("Trabajos civiles", "SPTC", "Trabajo civil de Preparación de sitio"),
        ("Trabajos civiles", "CMTC", "Trabajo civil de Preparación de sitio"),
        ("Trabajos civiles", "MOTC", "Trabajo civil de Movimiento de tierras"),
        ("Trabajos civiles", "ARTC", "Trabajo civil de Camino de acceso"),
        ("Trabajos civiles", "IRTC", "Trabajo civil de Caminos internos"),
        ("Trabajos civiles", "SDTC", "Trabajo civil de Sistema de drenaje"),
        ("Trabajos civiles", "SHTC", "Trabajo civil de Contenedores"),

        ("Instalación mecánica", "PAIM", "Instalación mecánica de Paneles"),
        ("Instalación mecánica", "ESIM", "Instalación mecánica de Estructuras"),
        ("Instalación mecánica", "ESTC", "Instalación mecánica de Estructuras (Hincado)"),
        ("Instalación mecánica", "CTIM", "Instalación mecánica de Centro de transformación"),
        ("Instalación mecánica", "CIIM", "Instalación mecánica de Centro de inversión"),

        ("Hot Commissioning", "HC", "Hot Commissioning"),

        ("Instalación Eléctrica", "PAIE", "Instalación Eléctrica de Paneles"),
        ("Instalación Eléctrica", "CTIE", "Instalación Eléctrica de Centro de transformación"),
        ("Instalación Eléctrica", "CIIE", "Instalación Eléctrica de Centro de inversión"),
        ("Instalación Eléctrica", "DSIE", "Instalación Eléctrica de Estación de Entrega"),
        ("Instalación Eléctrica", "MTIE", "Instalación Eléctrica de Media tensión"),
        ("Instalación Eléctrica", "BTIE", "Instalación Eléctrica de Baja tensión"),
        ("Instalación Eléctrica", "SOIE", "Conexionado Eléctrica de Solar"),
        ("Instalación Eléctrica", "CMIE", "Instalación Eléctrica de Comunicación"),
        ("Instalación Eléctrica", "FCIE", "Instalación Eléctrica de Fuerza y control"),
        ("Instalación Eléctrica", "PTIE", "Instalación Eléctrica de Sistema puesta a tierra"),
        ("Instalación Eléctrica", "UPIE", "Instalación Eléctrica de Sistema UPS"),
        ("Instalación Eléctrica", "SHIE", "Instalación Eléctrica de Contenedores"),
        ("Instalación Eléctrica", "CAIE", "Instalación Eléctrica de Accesorios cableado"),
        ("Instalación Eléctrica", "DSIM", "Instalación mecánica de Estación de Entrega"),
        ("Instalación Eléctrica", "SIIM", "Instalación mecánica de String inverter"),
        ("Instalación Eléctrica", "SBIM", "Instalación mecánica de String box"),
        ("Instalación Eléctrica", "MTIM", "Instalación mecánica de Media tensión"),
        ("Instalación Eléctrica", "BTIM", "Instalación mecánica de Baja tensión"),
        ("Instalación Eléctrica", "SOIM", "Tendido mecánica de Solar"),
        ("Instalación Eléctrica", "CMIM", "Instalación mecánica de Comunicación"),
        ("Instalación Eléctrica", "FCIM", "Instalación mecánica de Fuerza y control"),
        ("Instalación Eléctrica", "UPIM", "Instalación mecánica de Sistema UPS"),
        ("Instalación Eléctrica", "SHIM", "Instalación mecánica de Contenedores"),
        ("Instalación Eléctrica", "TBIM", "Instalación mecánica de Tableros baja tensión"),
        ("Instalación Eléctrica", "CC", "Instalación mecánica de Cold commisioning"),

        ("CCTV", "CCIM", "Instalación mecánica de CCTV"),
        ("CCTV", "CCIE", "Instalación Eléctrica de CCTV"),
        ("CCTV", "CCTC", "Instalación Eléctrica de CCTV"),
        ("CCTV", "CCHC", "Hot commisioning de CCTV"),

        ("SCADA", "SCIM", "Instalación mecánica de Sistema SCADA"),
        ("SCADA", "SCIE", "Instalación Eléctrica de Sistema SCADA"),
        ("SCADA", "SCHC", "Hot commisioning de Sistema SCADA"),

        ("Estaciones meteorológicas", "EMIM", "Instalación mecánica de Estación meterológica"),
        ("Estaciones meteorológicas", "EMIE", "Instalación Eléctrica de Estación meterológica"),

        ("Costos generales", "CGPE", "Costos generales de Permisos"),
        ("Costos generales", "CGIN", "Costos generales de Ingeniería"),
        ("Costos generales", "CGES", "Costos generales de Estudios"),
        ("Costos generales", "CGPO", "Costos generales de Pólizas"),
        ("Costos generales", "CGST", "Costos generales de Personal de gestión del proyecto"),
        ("Costos generales", "CGPR", "Costos generales de Trabajo preliminar"),
        ("Costos generales", "CGCG", "Costos generales de Costo general"),
        ("Costos generales", "CGHS", "Costos generales de Seguridad y salud en el trabajo"),
        ("Costos generales", "CGHC", "Costos generales de Hot commisioning"),

        ("Estudios", "CGAF", "Aprovechamiento Forestal y Ahuyentamiento"),
        ("Estudios", "CGAR", "Arqueologo"),
        ("Estudios", "CGTO", "Topografía (límites, curvas de nivel, objetos)"),
        ("Estudios", "CGPOT", "Prueba de extracción"),
        ("Estudios", "CGGEO", "Estudio de suelos"),
        ("Estudios", "CGHI", "Estudio hidrológico"),
        ("Estudios", "CGCO", "Estudio de corrosividad DIN 50929"),
        ("Estudios", "CGCP", "Estudio Coordinación de Protecciones"),
        ("Estudios", "CGRM", "Modelo RMS"),
        ("Estudios", "CGRA", "Modelo RMS de Auditoría en campo"),
        ("Estudios", "CGPQ", "Estudio Curva PQ"),
        ("Estudios", "CGAE", "Estudio arco eléctrico"),
        ("Estudios", "CGEA", "Estudio armónico"),
        ("Estudios", "CGEP", "Estudio Puesta a tierra"),
    ];
}
