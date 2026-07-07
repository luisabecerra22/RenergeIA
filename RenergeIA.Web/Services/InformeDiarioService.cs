using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public class InformeDiarioService(RenergeIADbContext db)
{
    public decimal CalcularAvanceEsperado(DateTime fechaInicio, DateTime fechaFin, DateTime fechaInforme)
    {
        if (fechaFin <= fechaInicio) return 100m;
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

    // Datos para la Curva S: planificado desde cronograma WBS, real desde informes diarios.
    public async Task<CurvaSData> DatosCurvaSAsync(int proyectoId)
    {
        var versionVigente = await db.CronogramasVersion
            .FirstOrDefaultAsync(v => v.ProyectoId == proyectoId && v.EsVigente);

        var actividades = await db.ActividadesWBS
            .Where(a => a.ProyectoId == proyectoId && a.Activo
                     && (versionVigente == null || a.CronogramaVersionId == versionVigente.Id))
            .ToListAsync();

        if (actividades.Count == 0)
            return new CurvaSData();

        var codigosSet = actividades.Select(a => a.CodigoWBS).ToHashSet();
        var hojas = actividades
            .Where(a => !codigosSet.Any(c => c != a.CodigoWBS && c.StartsWith(a.CodigoWBS + ".")))
            .ToList();

        if (hojas.Count == 0) hojas = actividades;

        var inicioProyecto = hojas.Min(a => a.FechaInicioPlaneada).Date;
        var finProyecto    = hojas.Max(a => a.FechaFinPlaneada).Date;
        var hoy            = DateTime.UtcNow.Date;

        // Generar puntos semanales para la línea planificada (todo el proyecto)
        var puntosPlan = new List<(DateTime Fecha, double Avance)>();
        for (var d = inicioProyecto; d <= finProyecto; d = d.AddDays(7))
        {
            var avg = hojas.Average(a => (double)CalcularAvanceEsperado(
                a.FechaInicioPlaneada, a.FechaFinPlaneada, d));
            puntosPlan.Add((d, Math.Round(avg, 2)));
        }
        if (puntosPlan.Last().Fecha < finProyecto)
            puntosPlan.Add((finProyecto, Math.Round(
                hojas.Average(a => (double)CalcularAvanceEsperado(
                    a.FechaInicioPlaneada, a.FechaFinPlaneada, finProyecto)), 2)));

        // Línea real: desde informes diarios, promedio acumulado por fecha
        var todosRegistros = await db.RegistrosAvanceDiario
            .Where(r => r.ProyectoId == proyectoId)
            .ToListAsync();

        var realPorFecha = todosRegistros
            .GroupBy(r => r.Fecha.Date)
            .Select(g => (Fecha: g.Key, Avance: Math.Round(g.Average(r => (double)r.AvanceAcumulado), 2)))
            .OrderBy(x => x.Fecha)
            .ToList();

        // Usar solo las fechas semanales del planificado como eje X
        var fechasStr    = new List<string>();
        var planificado  = new List<double>();
        var real         = new List<double?>();

        // Avance real actual del WBS (promedio hojas) para el punto de "hoy"
        var avanceRealHoy = Math.Round(hojas.Average(a => (double)a.AvanceReal), 2);

        foreach (var punto in puntosPlan)
        {
            var fecha = punto.Fecha;
            fechasStr.Add(fecha.ToString("dd-MMM-yy"));
            planificado.Add(punto.Avance);

            if (fecha > hoy)
            {
                // Futuro: no hay dato real
                real.Add(null);
            }
            else if (!realPorFecha.Any())
            {
                // No hay informes: usar el avance real del WBS proporcional
                // Antes del inicio = 0, desde inicio hasta hoy = proporcional al avance actual
                if (fecha < inicioProyecto)
                    real.Add(0);
                else
                {
                    var diasDesdeInicio = (fecha - inicioProyecto).TotalDays;
                    var diasHastaHoy = (hoy - inicioProyecto).TotalDays;
                    if (diasHastaHoy <= 0)
                        real.Add(avanceRealHoy);
                    else
                        real.Add(Math.Round(avanceRealHoy * diasDesdeInicio / diasHastaHoy, 2));
                }
            }
            else
            {
                // Hay informes: buscar el valor real más cercano <= fecha
                var registroPrevio = realPorFecha.LastOrDefault(r => r.Fecha <= fecha);
                if (registroPrevio != default)
                    real.Add(registroPrevio.Avance);
                else if (fecha >= inicioProyecto)
                    real.Add(0); // Antes del primer informe pero después del inicio
                else
                    real.Add(0);
            }
        }

        // Asegurar que "hoy" esté como punto si no coincide con un punto semanal
        if (hoy > inicioProyecto && hoy <= finProyecto && !puntosPlan.Any(p => p.Fecha == hoy))
        {
            // Encontrar posición de inserción
            var idx = fechasStr.Count;
            for (int i = 0; i < puntosPlan.Count; i++)
            {
                if (puntosPlan[i].Fecha > hoy) { idx = i; break; }
            }

            // Interpolar planificado para hoy
            var antes = puntosPlan.LastOrDefault(p => p.Fecha <= hoy);
            var desp  = puntosPlan.FirstOrDefault(p => p.Fecha >= hoy);
            double planHoy = 0;
            if (antes != default && desp != default && desp.Fecha != antes.Fecha)
            {
                var ratio = (hoy - antes.Fecha).TotalDays / (desp.Fecha - antes.Fecha).TotalDays;
                planHoy = Math.Round(antes.Avance + (desp.Avance - antes.Avance) * ratio, 2);
            }
            else if (antes != default) planHoy = antes.Avance;

            fechasStr.Insert(idx, hoy.ToString("dd-MMM-yy"));
            planificado.Insert(idx, planHoy);
            real.Insert(idx, avanceRealHoy);
        }

        // Avance planificado a corte de hoy
        double planificadoHoy = 0;
        {
            var antes = puntosPlan.LastOrDefault(p => p.Fecha <= hoy);
            var desp  = puntosPlan.FirstOrDefault(p => p.Fecha >= hoy);
            if (antes != default && desp != default && desp.Fecha != antes.Fecha)
            {
                var ratio = (hoy - antes.Fecha).TotalDays / (desp.Fecha - antes.Fecha).TotalDays;
                planificadoHoy = Math.Round(antes.Avance + (desp.Avance - antes.Avance) * ratio, 2);
            }
            else if (antes != default) planificadoHoy = antes.Avance;
        }

        return new CurvaSData
        {
            Fechas            = fechasStr,
            AvancePlanificado = planificado,
            AvanceReal        = real,
            PlanificadoHoy    = planificadoHoy,
            EjecutadoHoy      = avanceRealHoy
        };
    }

    // Dashboard completo: KPIs, curva S, disciplinas, actividades críticas/atrasadas.
    public async Task<DashboardCompleto> ObtenerDashboardCompletoAsync(int proyectoId)
    {
        var hoy = DateTime.UtcNow.Date;

        var versionVigente = await db.CronogramasVersion
            .FirstOrDefaultAsync(v => v.ProyectoId == proyectoId && v.EsVigente);

        var actividades = await db.ActividadesWBS
            .Where(a => a.ProyectoId == proyectoId && a.Activo
                     && (versionVigente == null || a.CronogramaVersionId == versionVigente.Id))
            .ToListAsync();

        if (actividades.Count == 0)
            return new DashboardCompleto { TieneDatos = false };

        // Filtrar solo hojas (sin hijos) para cálculos precisos
        var codigosSet = actividades.Select(a => a.CodigoWBS).ToHashSet();
        var hojas = actividades
            .Where(a => !codigosSet.Any(c => c != a.CodigoWBS && c.StartsWith(a.CodigoWBS + ".")))
            .ToList();
        if (hojas.Count == 0) hojas = actividades;

        // Último avance registrado por actividad (en memoria para evitar GroupBy problemático)
        var todosRegistros = await db.RegistrosAvanceDiario
            .Where(r => r.ProyectoId == proyectoId)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();

        var mapaAvances = todosRegistros
            .GroupBy(r => r.ActividadWBSId)
            .ToDictionary(g => g.Key, g => g.First().AvanceAcumulado);

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

        var avgProg   = Math.Round(actsDash.Average(a => a.AvanceProgramado), 1);
        var avgReal   = Math.Round(actsDash.Average(a => a.AvanceReal), 1);
        var desvTotal = Math.Round(avgReal - avgProg, 1);
        var spi       = avgProg > 0 ? Math.Round(avgReal / avgProg, 2) : 1m;
        var estadoGen = spi >= 1m ? "Al Día" : spi >= 0.9m ? "Leve Atraso" : spi >= 0.75m ? "Atrasado" : "Crítico";

        var porDisciplina = actsDash
            .Where(a => a.Disciplina.HasValue)
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
            .OrderBy(d => d.Disciplina)
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
            CurvaS                = await DatosCurvaSAsync(proyectoId),
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
    public double PlanificadoHoy { get; set; }
    public double EjecutadoHoy { get; set; }
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
