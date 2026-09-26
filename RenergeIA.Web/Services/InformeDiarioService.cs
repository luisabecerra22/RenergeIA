using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public class InformeDiarioService(RenergeIADbContext db)
{
    // Avance esperado (lineal) de una actividad a una fecha.
    // Hitos (fin <= inicio): 0 % antes de la fecha del hito y 100 % desde ese día.
    public decimal CalcularAvanceEsperado(DateTime fechaInicio, DateTime fechaFin, DateTime fechaInforme)
    {
        if (fechaFin <= fechaInicio) return fechaInforme.Date >= fechaFin.Date ? 100m : 0m;
        if (fechaInforme <= fechaInicio) return 0m;
        var total = (fechaFin - fechaInicio).TotalDays;
        var transcurrido = Math.Min((fechaInforme - fechaInicio).TotalDays, total);
        return Math.Round((decimal)(transcurrido / total) * 100, 2);
    }

    public decimal CalcularSPI(decimal avanceReal, decimal avanceEsperado)
        => avanceEsperado == 0 ? 1m : Math.Round(avanceReal / avanceEsperado, 2);

    public EstadoAvance DeterminarEstado(decimal desviacion) => desviacion switch
    {
        >= 5m   => EstadoAvance.Adelantado,
        >= 0m   => EstadoAvance.EnTiempo,
        >= -10m => EstadoAvance.Atrasado,
        _       => EstadoAvance.Critico
    };

    public async Task ActualizarCalculosAsync(RegistroAvanceDiario r, ActividadWBS act)
    {
        r.AvanceEsperado = CalcularAvanceEsperado(act.FechaInicioPlaneada, act.FechaFinPlaneada, r.Fecha);

        var previo = await db.RegistrosAvanceDiario
            .Where(x => x.ActividadWBSId == r.ActividadWBSId
                     && x.ProyectoId == r.ProyectoId
                     && x.Fecha < r.Fecha
                     && x.Id != r.Id)
            .OrderByDescending(x => x.Fecha)
            .Select(x => x.AvanceAcumulado)
            .FirstOrDefaultAsync();

        r.AvanceAcumulado = Math.Max(r.PorcentajeAvance, previo);
        r.Desviacion = Math.Round(r.AvanceAcumulado - r.AvanceEsperado, 2);
        r.DiasAtraso = r.Desviacion >= 0 ? 0 :
            (int)Math.Round(Math.Abs((double)r.Desviacion / 100)
                * (act.FechaFinPlaneada - act.FechaInicioPlaneada).TotalDays);
        r.Estado = DeterminarEstado(r.Desviacion);

        // Sincronizar avance real en la actividad WBS
        act.AvanceReal = r.AvanceAcumulado;
        act.FechaModificacion = DateTime.UtcNow;
    }

    public async Task RecalcularPadresWBSAsync(int proyectoId)
    {
        var versionVigente = await db.CronogramasVersion
            .FirstOrDefaultAsync(v => v.ProyectoId == proyectoId && v.EsVigente);

        var actividades = await db.ActividadesWBS
            .Where(a => a.ProyectoId == proyectoId && a.Activo
                     && (versionVigente == null || a.CronogramaVersionId == versionVigente.Id))
            .ToListAsync();

        var codigosSet = actividades.Select(a => a.CodigoWBS).ToHashSet();
        var padreIds = actividades
            .Where(a => a.ActividadPadreId.HasValue)
            .Select(a => a.ActividadPadreId!.Value)
            .ToHashSet();

        var padres = actividades
            .Where(a => padreIds.Contains(a.Id))
            .OrderByDescending(a => a.NivelWBS)
            .ToList();

        foreach (var padre in padres)
        {
            var hijos = actividades.Where(h => h.ActividadPadreId == padre.Id && h.Activo).ToList();
            if (!hijos.Any()) continue;
            padre.AvanceReal = Math.Round(hijos.Average(h => h.AvanceReal), 1);
            padre.FechaModificacion = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    public async Task<List<ResumenDisciplina>> ResumenPorDisciplinaAsync(int proyectoId, DateTime fecha)
    {
        var registros = await db.RegistrosAvanceDiario
            .Include(r => r.ActividadWBS)
            .Where(r => r.ProyectoId == proyectoId && r.Fecha == fecha.Date)
            .ToListAsync();

        return registros
            .Where(r => r.ActividadWBS.Disciplina.HasValue)
            .GroupBy(r => r.ActividadWBS.Disciplina!.Value)
            .Select(g => new ResumenDisciplina
            {
                Disciplina       = g.Key,
                AvancePromedio   = Math.Round(g.Average(r => r.AvanceAcumulado), 1),
                EsperadoPromedio = Math.Round(g.Average(r => r.AvanceEsperado), 1),
                SPI              = Math.Round(g.Average(r => CalcularSPI(r.AvanceAcumulado, r.AvanceEsperado)), 2),
                Total            = g.Count(),
                Atrasadas        = g.Count(r => r.Estado is EstadoAvance.Atrasado or EstadoAvance.Critico)
            })
            .OrderBy(r => r.Disciplina)
            .ToList();
    }

    // Resumen por disciplina usando el último registro de cada actividad (para el dashboard general).
    public async Task<List<ResumenDisciplina>> ResumenDisciplinasProyectoAsync(int proyectoId)
    {
        // Para cada actividad, tomar el registro más reciente
        var ultimosRegistros = await db.RegistrosAvanceDiario
            .Include(r => r.ActividadWBS)
            .Where(r => r.ProyectoId == proyectoId)
            .GroupBy(r => r.ActividadWBSId)
            .Select(g => g.OrderByDescending(r => r.Fecha).First())
            .ToListAsync();

        return ultimosRegistros
            .Where(r => r.ActividadWBS?.Disciplina.HasValue == true)
            .GroupBy(r => r.ActividadWBS!.Disciplina!.Value)
            .Select(g => new ResumenDisciplina
            {
                Disciplina       = g.Key,
                AvancePromedio   = Math.Round(g.Average(r => r.AvanceAcumulado), 1),
                EsperadoPromedio = Math.Round(g.Average(r => r.AvanceEsperado), 1),
                SPI              = Math.Round(g.Average(r => CalcularSPI(r.AvanceAcumulado, r.AvanceEsperado)), 2),
                Total            = g.Count(),
                Atrasadas        = g.Count(r => r.Estado is EstadoAvance.Atrasado or EstadoAvance.Critico)
            })
            .OrderBy(r => r.Disciplina)
            .ToList();
    }

    // KPIs globales del proyecto.
    public async Task<KPIsProyecto> KPIsProyectoAsync(int proyectoId)
    {
        var ultimosRegistros = await db.RegistrosAvanceDiario
            .Where(r => r.ProyectoId == proyectoId)
            .GroupBy(r => r.ActividadWBSId)
            .Select(g => g.OrderByDescending(r => r.Fecha).First())
            .ToListAsync();

        if (ultimosRegistros.Count == 0)
            return new KPIsProyecto();

        var avanceGeneral    = Math.Round(ultimosRegistros.Average(r => r.AvanceAcumulado), 1);
        var esperadoGeneral  = Math.Round(ultimosRegistros.Average(r => r.AvanceEsperado), 1);
        var spiGlobal        = CalcularSPI(avanceGeneral, esperadoGeneral);
        var atrasadas        = ultimosRegistros.Count(r => r.Estado is EstadoAvance.Atrasado or EstadoAvance.Critico);
        var totalInformes    = await db.InformesDiarios.CountAsync(i => i.ProyectoId == proyectoId);

        return new KPIsProyecto
        {
            AvanceGeneral   = avanceGeneral,
            EsperadoGeneral = esperadoGeneral,
            SPIGlobal       = spiGlobal,
            ActividadesAtrasadas = atrasadas,
            TotalActividades     = ultimosRegistros.Count,
            TotalInformes        = totalInformes
        };
    }

    // ── Curva S ──────────────────────────────────────────────────────────────────
    // Planificado: suma ponderada del avance lineal esperado de cada actividad HOJA del
    //   cronograma vigente. Peso = duración planeada en días (los hitos, con duración 0,
    //   no aportan peso). Así una actividad de 6 meses pesa más que una de 2 días y la
    //   curva toma la forma de S clásica (arranque lento, máximo ritmo cuando más frentes
    //   están activos, cierre suave).
    // Real: para cada fecha se toma, por actividad, el último AvanceAcumulado informado hasta
    //   esa fecha (0 si aún no se había informado) y se pondera con los mismos pesos. Las
    //   actividades sin ningún informe aportan el avance manual/importado del WBS repartido
    //   linealmente desde su inicio (real o planeado) hasta hoy. El tramo anterior al primer
    //   informe diario es estimado y se dibuja punteado; desde el primer informe, sólido.

    // Disciplinas que se muestran en "Avance por disciplina" del Dashboard y su orden (definido por la
    // usuaria el 2026-09-26): Contractual, General y Construcción no se grafican.
    public static readonly Disciplina[] DisciplinasDashboard =
        [Disciplina.Suministros, Disciplina.Ingenieria, Disciplina.Civil, Disciplina.Mecanica,
         Disciplina.Electrica, Disciplina.PuestaEnMarcha, Disciplina.CierreProyecto];

    public static readonly Disciplina[] DisciplinasConstruccion =
        [Disciplina.Civil, Disciplina.Mecanica, Disciplina.Electrica, Disciplina.Construccion];

    private sealed record PuntoCurva(DateTime Fecha, double Planificado, double? Real);

    private sealed class CalculadoraCurvaS
    {
        private readonly List<ActividadWBS> _hojas;
        private readonly Dictionary<int, double> _pesos;
        private readonly double _pesoTotal;
        private readonly Dictionary<int, List<(DateTime Fecha, decimal Avance)>> _registros;
        public DateTime? PrimeraFechaReal { get; }
        public int ActividadesConInforme => _registros.Count;

        public CalculadoraCurvaS(List<ActividadWBS> hojas, List<RegistroCurva> registros)
        {
            _hojas = hojas;
            _pesos = hojas.ToDictionary(a => a.Id,
                a => Math.Max(0d, (a.FechaFinPlaneada.Date - a.FechaInicioPlaneada.Date).TotalDays));
            if (_pesos.Values.Sum() <= 0)
                foreach (var id in _pesos.Keys.ToList()) _pesos[id] = 1;
            _pesoTotal = _pesos.Values.Sum();

            var idsHojas = hojas.Select(a => a.Id).ToHashSet();
            _registros = registros
                .Where(r => idsHojas.Contains(r.ActividadId))
                .GroupBy(r => r.ActividadId)
                .ToDictionary(g => g.Key,
                    g => g.OrderBy(r => r.Fecha).Select(r => (r.Fecha.Date, r.AvanceAcumulado)).ToList());

            PrimeraFechaReal = _registros.Count > 0 ? _registros.Values.Min(l => l[0].Fecha) : null;
        }

        public double Planificado(DateTime fecha, Func<DateTime, DateTime, DateTime, decimal> esperado)
        {
            if (_pesoTotal <= 0) return 0;
            double suma = 0;
            foreach (var a in _hojas)
                suma += _pesos[a.Id] * (double)esperado(a.FechaInicioPlaneada, a.FechaFinPlaneada, fecha);
            return Math.Round(suma / _pesoTotal, 2);
        }

        // Avance real ponderado a una fecha. `hoy` es la fecha de corte.
        // - Con informes: último AvanceAcumulado informado hasta esa fecha. Antes del primer
        //   informe de la actividad se ESTIMA lineal desde su inicio (real o planeado) hasta el
        //   valor de ese primer informe.
        // - Sin informes: el avance del WBS se ESTIMA repartido linealmente desde el inicio de la
        //   actividad (real o planeado) hasta hoy (o hasta su fin real si ya está al 100 %).
        public double Real(DateTime fecha, DateTime hoy)
        {
            if (_pesoTotal <= 0) return 0;
            var d = fecha.Date;
            double suma = 0;
            foreach (var a in _hojas)
            {
                var inicio = (a.FechaInicioReal ?? a.FechaInicioPlaneada).Date;
                decimal avance;
                if (_registros.TryGetValue(a.Id, out var lista))
                {
                    var (f0, v0) = lista[0];
                    if (d < f0)
                        avance = v0 * Fraccion(d, inicio, f0);
                    else
                    {
                        avance = v0;
                        foreach (var (f, v) in lista)
                        {
                            if (f > d) break;
                            avance = v;
                        }
                    }
                }
                else
                {
                    var objetivo = a.AvanceReal;
                    var fin = (objetivo >= 100m && a.FechaFinReal.HasValue) ? a.FechaFinReal.Value.Date : hoy.Date;
                    avance = objetivo * Fraccion(d, inicio, fin);
                }

                suma += _pesos[a.Id] * (double)Math.Clamp(avance, 0m, 100m);
            }
            return Math.Round(suma / _pesoTotal, 2);
        }

        // Fracción [0,1] de recorrido de `d` entre `inicio` y `fin`.
        private static decimal Fraccion(DateTime d, DateTime inicio, DateTime fin)
        {
            if (fin <= inicio) return d >= fin ? 1m : 0m;
            if (d <= inicio) return 0m;
            if (d >= fin) return 1m;
            return (decimal)((d - inicio).TotalDays / (fin - inicio).TotalDays);
        }

        // Primera fecha con avance real (estimado o informado) > 0.
        public DateTime? PrimeraFechaConAvance(DateTime hoy)
        {
            DateTime? min = null;
            foreach (var a in _hojas)
            {
                var tiene = _registros.ContainsKey(a.Id) || a.AvanceReal > 0m;
                if (!tiene) continue;
                var inicio = (a.FechaInicioReal ?? a.FechaInicioPlaneada).Date;
                if (inicio > hoy.Date) inicio = hoy.Date;
                if (min is null || inicio < min) min = inicio;
            }
            return min;
        }
    }

    // Registro de avance ya resuelto a una actividad hoja del cronograma vigente.
    private sealed record RegistroCurva(int ActividadId, DateTime Fecha, decimal AvanceAcumulado);

    // Histórico real cargado (informe interno): interpolación lineal entre puntos.
    private sealed class HistoricoCurva
    {
        private readonly List<(DateTime Fecha, double Valor)> _puntos;
        public HistoricoCurva(IEnumerable<PuntoCurvaReal> puntos)
        {
            _puntos = puntos.GroupBy(p => p.Fecha.Date)
                .Select(g => (g.Key, (double)g.Last().PorcentajeReal))
                .OrderBy(p => p.Key).ToList();
        }
        public int Count => _puntos.Count;
        public DateTime Desde => _puntos[0].Fecha;
        public DateTime Hasta => _puntos[^1].Fecha;
        public IEnumerable<DateTime> Fechas => _puntos.Select(p => p.Fecha);

        public double Valor(DateTime d)
        {
            d = d.Date;
            if (d <= Desde) return _puntos[0].Valor;
            if (d >= Hasta) return _puntos[^1].Valor;
            for (var i = 1; i < _puntos.Count; i++)
            {
                if (_puntos[i].Fecha < d) continue;
                var (fa, va) = _puntos[i - 1];
                var (fb, vb) = _puntos[i];
                if (fb == fa) return vb;
                var r = (d - fa).TotalDays / (fb - fa).TotalDays;
                return Math.Round(va + (vb - va) * r, 2);
            }
            return _puntos[^1].Valor;
        }
    }

    private static List<ActividadWBS> SoloHojas(List<ActividadWBS> actividades)
    {
        var codigosSet = actividades.Select(a => a.CodigoWBS).ToHashSet();
        var hojas = actividades
            .Where(a => !codigosSet.Any(c => c != a.CodigoWBS && c.StartsWith(a.CodigoWBS + ".")))
            .ToList();
        return hojas.Count == 0 ? actividades : hojas;
    }

    private static DateTime HoyColombia() => RenergeIA.Core.Helpers.SeguimientoDocumento.HoyColombia();

    // Registros de avance de los informes diarios del proyecto, resueltos a las actividades hoja
    // del cronograma vigente. Si un informe apunta a una actividad de otra versión del cronograma
    // (o inactiva), se cruza por CódigoWBS con la actividad vigente del mismo código.
    private async Task<List<RegistroCurva>> RegistrosCurvaAsync(int proyectoId, List<ActividadWBS> hojas)
    {
        var registros = await db.RegistrosAvanceDiario.AsNoTracking()
            .Where(r => r.ProyectoId == proyectoId)
            .Select(r => new { r.ActividadWBSId, r.Fecha, r.AvanceAcumulado })
            .ToListAsync();
        if (registros.Count == 0) return [];

        var idsHojas = hojas.Select(h => h.Id).ToHashSet();
        var porCodigo = hojas.GroupBy(h => h.CodigoWBS).ToDictionary(g => g.Key, g => g.First().Id);

        var idsAjenos = registros.Select(r => r.ActividadWBSId).Where(id => !idsHojas.Contains(id)).Distinct().ToList();
        var codigoDeAjeno = idsAjenos.Count == 0
            ? new Dictionary<int, string>()
            : await db.ActividadesWBS.AsNoTracking().IgnoreQueryFilters()
                .Where(a => idsAjenos.Contains(a.Id))
                .Select(a => new { a.Id, a.CodigoWBS })
                .ToDictionaryAsync(a => a.Id, a => a.CodigoWBS);

        var lista = new List<RegistroCurva>(registros.Count);
        foreach (var r in registros)
        {
            var id = r.ActividadWBSId;
            if (!idsHojas.Contains(id))
            {
                if (!codigoDeAjeno.TryGetValue(id, out var codigo) || !porCodigo.TryGetValue(codigo, out id))
                    continue; // no se puede cruzar: se ignora
            }
            lista.Add(new RegistroCurva(id, r.Fecha.Date, r.AvanceAcumulado));
        }
        return lista;
    }

    public Task<List<PuntoCurvaReal>> HistoricoCurvaAsync(int proyectoId, bool soloConstruccion) =>
        db.PuntosCurvaReal.AsNoTracking()
            .Where(p => p.ProyectoId == proyectoId && p.SoloConstruccion == soloConstruccion)
            .OrderBy(p => p.Fecha)
            .ToListAsync();

    // Reemplaza el histórico real del proyecto para ese alcance.
    public async Task<int> GuardarHistoricoCurvaAsync(int proyectoId, bool soloConstruccion,
        IEnumerable<(DateTime Fecha, decimal Porcentaje)> puntos, string? origen)
    {
        await db.PuntosCurvaReal
            .Where(p => p.ProyectoId == proyectoId && p.SoloConstruccion == soloConstruccion)
            .ExecuteDeleteAsync();

        var nuevos = puntos
            .GroupBy(p => p.Fecha.Date)
            .Select(g => new PuntoCurvaReal
            {
                ProyectoId       = proyectoId,
                SoloConstruccion = soloConstruccion,
                Fecha            = DateTime.SpecifyKind(g.Key, DateTimeKind.Unspecified),
                PorcentajeReal   = Math.Clamp(g.Last().Porcentaje, 0m, 100m),
                Origen           = origen
            })
            .ToList();
        db.PuntosCurvaReal.AddRange(nuevos);
        await db.SaveChangesAsync();
        return nuevos.Count;
    }

    public Task<int> BorrarHistoricoCurvaAsync(int proyectoId, bool soloConstruccion) =>
        db.PuntosCurvaReal
            .Where(p => p.ProyectoId == proyectoId && p.SoloConstruccion == soloConstruccion)
            .ExecuteDeleteAsync();

    // Datos para la Curva S: planificado desde cronograma WBS vigente, real desde el histórico
    // cargado + informes diarios. `soloConstruccion` limita el alcance a las actividades
    // Civil / Mecánica / Eléctrica (equivalente a la curva de "Avance de construcción" del
    // informe interno).
    public async Task<CurvaSData> DatosCurvaSAsync(int proyectoId, bool soloConstruccion = false)
    {
        var versionVigente = await db.CronogramasVersion
            .FirstOrDefaultAsync(v => v.ProyectoId == proyectoId && v.EsVigente);

        var actividades = await db.ActividadesWBS
            .Where(a => a.ProyectoId == proyectoId && a.Activo
                     && (versionVigente == null || a.CronogramaVersionId == versionVigente.Id))
            .ToListAsync();

        if (actividades.Count == 0)
            return new CurvaSData();

        var hojas = SoloHojas(actividades);
        if (soloConstruccion)
            hojas = hojas.Where(a => a.Disciplina.HasValue && DisciplinasConstruccion.Contains(a.Disciplina.Value)).ToList();
        if (hojas.Count == 0)
            return new CurvaSData { SoloConstruccion = soloConstruccion, TotalActividades = 0 };

        var registros = await RegistrosCurvaAsync(proyectoId, hojas);
        var historico = await HistoricoCurvaAsync(proyectoId, soloConstruccion);

        return ConstruirCurvaS(hojas, registros, historico, soloConstruccion);
    }

    private CurvaSData ConstruirCurvaS(List<ActividadWBS> hojas, List<RegistroCurva> registros,
        List<PuntoCurvaReal> historicoPuntos, bool soloConstruccion)
    {
        var calc = new CalculadoraCurvaS(hojas, registros);
        var hist = historicoPuntos.Count > 0 ? new HistoricoCurva(historicoPuntos) : null;
        var hoy  = HoyColombia();

        var inicioProyecto = hojas.Min(a => a.FechaInicioPlaneada).Date;
        var finProyecto    = hojas.Max(a => a.FechaFinPlaneada).Date;
        if (finProyecto < inicioProyecto) finProyecto = inicioProyecto;
        if (hist is not null && hist.Desde < inicioProyecto) inicioProyecto = hist.Desde;

        // Eje X: puntos semanales desde el inicio, más el fin del proyecto, la fecha de hoy,
        // la fecha del primer informe y las fechas del histórico cargado.
        var fechas = new SortedSet<DateTime>();
        for (var d = inicioProyecto; d <= finProyecto; d = d.AddDays(7)) fechas.Add(d);
        fechas.Add(finProyecto);
        void Agregar(DateTime f) { if (f >= inicioProyecto && f <= finProyecto) fechas.Add(f); }
        Agregar(hoy);
        if (calc.PrimeraFechaReal is { } pf) Agregar(pf);
        if (hist is not null)
        {
            var paso = Math.Max(1, hist.Count / 250); // no saturar el eje si el histórico es diario
            var i = 0;
            foreach (var f in hist.Fechas) { if (i++ % paso == 0) Agregar(f); }
            Agregar(hist.Hasta);
        }

        // Tramos de la línea real (de más confiable a menos):
        //  1. Histórico cargado (informe interno): sólido, entre su primera y última fecha.
        //  2. Informes diarios de la app: sólido, desde el primer informe posterior al histórico.
        //  3. Estimación (punteada): entre el fin del histórico y el primer informe (o hoy),
        //     interpolando linealmente; sin histórico ni informes, avance del WBS repartido
        //     linealmente desde el inicio de cada actividad.
        var desdeReal = hist?.Desde ?? calc.PrimeraFechaConAvance(hoy) ?? hoy;
        if (desdeReal < inicioProyecto) desdeReal = inicioProyecto;
        Agregar(desdeReal);
        var hastaReal = hoy;

        // Primer informe posterior al histórico (si lo hay) y valor de anclaje en esa fecha
        DateTime? primerInformeUtil = calc.PrimeraFechaReal is { } p1 && (hist is null || p1 > hist.Hasta) ? p1 : null;
        var finHist    = hist?.Hasta ?? desdeReal;
        var valorHist  = hist?.Valor(finHist) ?? 0;
        var fechaAncla = primerInformeUtil ?? hoy;
        var valorAncla = calc.Real(fechaAncla, hoy);

        double RealEn(DateTime f, out bool esEstimado)
        {
            if (hist is not null && f <= hist.Hasta) { esEstimado = false; return hist.Valor(f); }
            if (primerInformeUtil is { } pi && f >= pi) { esEstimado = false; return calc.Real(f, hoy); }
            esEstimado = true;
            if (hist is null) return calc.Real(f, hoy);
            // Puente entre el fin del histórico y el ancla (primer informe u hoy)
            if (fechaAncla <= finHist) return valorHist;
            var r = (f - finHist).TotalDays / (fechaAncla - finHist).TotalDays;
            return Math.Round(valorHist + (valorAncla - valorHist) * Math.Clamp(r, 0, 1), 2);
        }

        var puntos   = new List<PuntoCurva>();
        var estimado = new List<bool>();
        foreach (var f in fechas)
        {
            var plan = calc.Planificado(f, CalcularAvanceEsperado);
            double? real = null;
            var est = false;
            if (f >= desdeReal && f <= hastaReal) real = RealEn(f, out est);
            puntos.Add(new PuntoCurva(f, plan, real));
            estimado.Add(real.HasValue && est);
        }

        var ejecutadoHoy = (hist is not null && hoy <= hist.Hasta) ? hist.Valor(hoy) : calc.Real(hoy, hoy);

        return new CurvaSData
        {
            Fechas            = puntos.Select(p => p.Fecha.ToString("dd-MMM-yy")).ToList(),
            AvancePlanificado = puntos.Select(p => p.Planificado).ToList(),
            AvanceReal        = puntos.Select(p => p.Real).ToList(),
            RealEstimado      = estimado,
            PlanificadoHoy    = calc.Planificado(hoy, CalcularAvanceEsperado),
            EjecutadoHoy      = ejecutadoHoy,
            FechaCorte        = hoy,
            PrimerInforme     = calc.PrimeraFechaReal,
            TotalActividades  = hojas.Count,
            Hitos             = hojas.Count(a => a.FechaFinPlaneada.Date <= a.FechaInicioPlaneada.Date),
            ActividadesConInforme = calc.ActividadesConInforme,
            HistoricoPuntos   = hist?.Count ?? 0,
            HistoricoDesde    = hist?.Desde,
            HistoricoHasta    = hist?.Hasta,
            TieneEstimado     = estimado.Any(e => e),
            SoloConstruccion  = soloConstruccion
        };
    }

    // Dashboard completo: KPIs, curva S, disciplinas, actividades críticas/atrasadas.
    public async Task<DashboardCompleto> ObtenerDashboardCompletoAsync(int proyectoId)
    {
        var hoy = HoyColombia();

        var versionVigente = await db.CronogramasVersion
            .FirstOrDefaultAsync(v => v.ProyectoId == proyectoId && v.EsVigente);

        var actividades = await db.ActividadesWBS
            .Where(a => a.ProyectoId == proyectoId && a.Activo
                     && (versionVigente == null || a.CronogramaVersionId == versionVigente.Id))
            .ToListAsync();

        if (actividades.Count == 0)
            return new DashboardCompleto { TieneDatos = false };

        // Solo hojas (sin hijos) para cálculos precisos
        var hojas = SoloHojas(actividades);

        // Último avance registrado por actividad (en memoria para evitar GroupBy problemático)
        // Registros de informes diarios resueltos a las hojas vigentes (cruce por CódigoWBS si hace falta)
        var registrosCurva = await RegistrosCurvaAsync(proyectoId, hojas);
        var historico      = await HistoricoCurvaAsync(proyectoId, soloConstruccion: false);

        var mapaAvances = registrosCurva
            .GroupBy(r => r.ActividadId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.Fecha).First().AvanceAcumulado);

        var actsDash = hojas.Select(a =>
        {
            var avanceReal = mapaAvances.TryGetValue(a.Id, out var ar) ? ar : a.AvanceReal;
            var avanceProg = CalcularAvanceEsperado(a.FechaInicioPlaneada, a.FechaFinPlaneada, hoy);
            var desv       = Math.Round(avanceReal - avanceProg, 1);
            return new ActividadDashboard
            {
                Id               = a.Id,
                Codigo           = a.CodigoWBS,
                Nombre           = a.Nombre,
                Disciplina       = a.Disciplina,
                AvanceProgramado = Math.Round(avanceProg, 1),
                AvanceReal       = Math.Round(avanceReal, 1),
                Desviacion       = desv,
                Estado           = ClasificarActividad(avanceReal, desv, avanceProg),
                EsCritica        = a.EsCritica,
                FechaFinPlaneada = a.FechaFinPlaneada
            };
        }).ToList();

        // Totales del proyecto: mismo cálculo ponderado por duración que la Curva S,
        // para que las tarjetas y la curva digan lo mismo.
        var curvaS    = ConstruirCurvaS(hojas, registrosCurva, historico, soloConstruccion: false);
        var avgProg   = Math.Round((decimal)curvaS.PlanificadoHoy, 1);
        var avgReal   = Math.Round((decimal)curvaS.EjecutadoHoy, 1);
        var desvTotal = Math.Round(avgReal - avgProg, 1);
        var spi       = avgProg > 0 ? Math.Round(avgReal / avgProg, 2) : 1m;
        var estadoGen = spi >= 1m ? "Al Día" : spi >= 0.9m ? "Leve Atraso" : spi >= 0.75m ? "Atrasado" : "Crítico";

        var porDisciplina = actsDash
            .Where(a => a.Disciplina.HasValue && DisciplinasDashboard.Contains(a.Disciplina.Value))
            .GroupBy(a => a.Disciplina!.Value)
            .Select(g => new ResumenDisciplinaDash
            {
                Disciplina       = g.Key,
                AvanceProgramado = Math.Round(g.Average(a => a.AvanceProgramado), 1),
                AvanceReal       = Math.Round(g.Average(a => a.AvanceReal), 1),
                Desviacion       = Math.Round(g.Average(a => a.Desviacion), 1),
                Total            = g.Count(),
                Atrasadas        = g.Count(a => a.Estado is EstadoDashboard.Atrasada or EstadoDashboard.Critica)
            })
            .OrderBy(d => Array.IndexOf(DisciplinasDashboard, d.Disciplina))
            .ToList();

        var counts = actsDash
            .GroupBy(a => a.Estado)
            .ToDictionary(g => g.Key, g => g.Count());

        return new DashboardCompleto
        {
            TieneDatos            = true,
            AvanceProgramadoTotal = avgProg,
            AvanceRealTotal       = avgReal,
            DesviacionTotal       = desvTotal,
            SPIGlobal             = spi,
            EstadoGeneral         = estadoGen,
            TotalActividades      = actsDash.Count,
            EnLinea               = counts.GetValueOrDefault(EstadoDashboard.EnLinea),
            Atrasadas             = counts.GetValueOrDefault(EstadoDashboard.Atrasada),
            Criticas              = counts.GetValueOrDefault(EstadoDashboard.Critica),
            Finalizadas           = counts.GetValueOrDefault(EstadoDashboard.Finalizada),
            NoIniciadas           = counts.GetValueOrDefault(EstadoDashboard.NoIniciada),
            CurvaS                = curvaS,
            PorDisciplina         = porDisciplina,
            TopAtrasadas          = actsDash
                                        .Where(a => a.Desviacion < 0)
                                        .OrderBy(a => a.Desviacion)
                                        .Take(10)
                                        .ToList(),
            ActividadesCriticas   = actsDash
                                        .Where(a => a.Estado is EstadoDashboard.Critica or EstadoDashboard.Atrasada)
                                        .OrderBy(a => a.Desviacion)
                                        .ToList()
        };
    }

    private static EstadoDashboard ClasificarActividad(decimal avanceReal, decimal desviacion, decimal avanceProg)
    {
        if (avanceReal >= 100) return EstadoDashboard.Finalizada;
        if (avanceReal == 0)   return EstadoDashboard.NoIniciada;
        if (desviacion < -15)  return EstadoDashboard.Critica;
        if (desviacion < -5)   return EstadoDashboard.Atrasada;
        return EstadoDashboard.EnLinea;
    }
}

public class ResumenDisciplina
{
    public Disciplina Disciplina { get; set; }
    public decimal AvancePromedio { get; set; }
    public decimal EsperadoPromedio { get; set; }
    public decimal SPI { get; set; }
    public int Total { get; set; }
    public int Atrasadas { get; set; }
}

public class KPIsProyecto
{
    public decimal AvanceGeneral { get; set; }
    public decimal EsperadoGeneral { get; set; }
    public decimal SPIGlobal { get; set; }
    public int ActividadesAtrasadas { get; set; }
    public int TotalActividades { get; set; }
    public int TotalInformes { get; set; }
    public decimal Desviacion => Math.Round(AvanceGeneral - EsperadoGeneral, 1);
}

public class CurvaSData
{
    public List<string> Fechas { get; set; } = [];
    public List<double?> AvanceReal { get; set; } = [];
    public List<double> AvancePlanificado { get; set; } = [];
    public List<bool> RealEstimado { get; set; } = [];
    public double PlanificadoHoy { get; set; }
    public double EjecutadoHoy { get; set; }
    public DateTime FechaCorte { get; set; }
    public DateTime? PrimerInforme { get; set; }
    public int TotalActividades { get; set; }
    public int Hitos { get; set; }
    public int ActividadesConInforme { get; set; }
    public bool SoloConstruccion { get; set; }
    public int HistoricoPuntos { get; set; }
    public DateTime? HistoricoDesde { get; set; }
    public DateTime? HistoricoHasta { get; set; }
    public bool TieneEstimado { get; set; }
}

public enum EstadoDashboard { EnLinea, Atrasada, Critica, Finalizada, NoIniciada }

public class DashboardCompleto
{
    public bool TieneDatos { get; set; }
    public decimal AvanceProgramadoTotal { get; set; }
    public decimal AvanceRealTotal { get; set; }
    public decimal DesviacionTotal { get; set; }
    public decimal SPIGlobal { get; set; }
    public string EstadoGeneral { get; set; } = "";
    public int TotalActividades { get; set; }
    public int EnLinea { get; set; }
    public int Atrasadas { get; set; }
    public int Criticas { get; set; }
    public int Finalizadas { get; set; }
    public int NoIniciadas { get; set; }
    public CurvaSData CurvaS { get; set; } = new();
    public List<ResumenDisciplinaDash> PorDisciplina { get; set; } = [];
    public List<ActividadDashboard> TopAtrasadas { get; set; } = [];
    public List<ActividadDashboard> ActividadesCriticas { get; set; } = [];
}

public class ActividadDashboard
{
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public Disciplina? Disciplina { get; set; }
    public decimal AvanceProgramado { get; set; }
    public decimal AvanceReal { get; set; }
    public decimal Desviacion { get; set; }
    public EstadoDashboard Estado { get; set; }
    public bool EsCritica { get; set; }
    public DateTime FechaFinPlaneada { get; set; }
    public string Recomendacion => Estado switch
    {
        EstadoDashboard.Critica    => "Intervención urgente: revisar recursos, restricciones y productividad. Priorizar en la planificación semanal.",
        EstadoDashboard.Atrasada   => "Seguimiento diario requerido. Validar restricciones y reasignar recursos si es necesario.",
        EstadoDashboard.NoIniciada => "Verificar pre-requisitos y confirmar inicio en la próxima semana de trabajo.",
        _                          => "Actividad dentro del rango esperado."
    };
}

public class ResumenDisciplinaDash
{
    public Disciplina Disciplina { get; set; }
    public decimal AvanceProgramado { get; set; }
    public decimal AvanceReal { get; set; }
    public decimal Desviacion { get; set; }
    public int Total { get; set; }
    public int Atrasadas { get; set; }
}
