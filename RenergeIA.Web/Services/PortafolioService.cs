using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Core.Helpers;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

// Fuente única de los indicadores de cada proyecto para el Centro de control y el Dashboard del
// proyecto, para que ambas pantallas digan lo mismo con el mismo corte:
//  - Avance programado / real: los mismos de la Curva S (DatosCurvaSAsync), ponderados por duración
//    y por categorías con peso.
//  - Brecha = real − programado, en puntos porcentuales (p.p.).
//  - Índice de cumplimiento físico = real ÷ programado (no es el SPI de valor ganado).
//  - Fecha proyectada = fin del cronograma vigente (la última reprogramación aprobada); desviación de
//    plazo = fecha proyectada − fecha contractual (Proyecto.FechaFinPlaneada), en días calendario.
//  - Costos en COP (USD × TRM del proyecto): ejecutado y comprometido salen del seguimiento de pagos;
//    sobrecosto = suma, por código, de lo que ejecutado + comprometido supera su presupuesto.
//    Costo estimado al cierre = presupuesto + sobrecosto.
//  - Bloqueos = restricciones Abiertas / En gestión; vencidas si su fecha compromiso ya pasó.
// Umbrales de estado (propuestos, pendientes de aprobación de gerencia): ver Clasificar.
public class PortafolioService(RenergeIADbContext db, InformeDiarioService informeSvc, CostoService costoSvc)
{
    public const int DiasDatoActualizado = 7;
    public const int DiasDatoDesactualizado = 21;

    public async Task<PortafolioVM> ObtenerAsync()
    {
        var proyectos = await db.Proyectos.AsNoTracking()
            .Where(p => p.Estado != EstadoProyecto.Cancelado && p.Estado != EstadoProyecto.Completado)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        var filas = new List<IndicadoresProyecto>();
        foreach (var p in proyectos)
            filas.Add(await CalcularAsync(p));

        var hoy = SeguimientoDocumento.HoyColombia();
        return new PortafolioVM
        {
            FechaCorte = hoy,
            FechaActualizacion = DateTime.UtcNow.AddHours(-5),
            Proyectos = filas
        };
    }

    public async Task<IndicadoresProyecto?> ObtenerProyectoAsync(int proyectoId)
    {
        var p = await db.Proyectos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == proyectoId);
        return p is null ? null : await CalcularAsync(p);
    }

    private async Task<IndicadoresProyecto> CalcularAsync(Proyecto p)
    {
        var hoy = SeguimientoDocumento.HoyColombia();
        var ind = new IndicadoresProyecto
        {
            ProyectoId = p.Id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Cliente = p.Cliente,
            CapacidadKWp = p.CapacidadKWp,
            EstadoProyecto = p.Estado,
            FechaInicio = p.FechaInicioPlaneada.Year > 2000 ? p.FechaInicioPlaneada.Date : null,
            FechaContractual = p.FechaFinPlaneada.Year > 2000 ? p.FechaFinPlaneada.Date : null
        };

        // ── Avance (misma fuente que la Curva S y el Dashboard) ──
        var curva = await informeSvc.DatosCurvaSAsync(p.Id);
        ind.TieneCronograma = curva.TotalActividades > 0;
        if (ind.TieneCronograma)
        {
            ind.AvanceProgramado = Math.Round((decimal)curva.PlanificadoHoy, 1);
            ind.AvanceReal = Math.Round((decimal)curva.EjecutadoHoy, 1);
            ind.Brecha = Math.Round(ind.AvanceReal - ind.AvanceProgramado, 1);
            ind.Indice = ind.AvanceProgramado > 0 ? Math.Round(ind.AvanceReal / ind.AvanceProgramado, 2) : null;

            var version = await db.CronogramasVersion.AsNoTracking()
                .FirstOrDefaultAsync(v => v.ProyectoId == p.Id && v.EsVigente);
            ind.VersionCronograma = version is null ? null : $"{version.Nombre} (v{version.NumeroVersion})";
            var hojas = await informeSvc.ActividadesHojaVigentesAsync(p.Id);
            if (hojas.Count > 0)
            {
                ind.FechaProyectada = hojas.Max(a => a.FechaFinPlaneada).Date;
                ind.FechaInicio ??= hojas.Min(a => a.FechaInicioPlaneada).Date;
                ind.UltimoDatoAvance = hojas.Max(a => a.FechaModificacion ?? a.FechaCreacion).Date;
            }
            if (ind.FechaContractual is { } fc && ind.FechaProyectada is { } fp)
                ind.RetrasoDias = (int)(fp - fc).TotalDays;
        }

        var ultimoInforme = await db.InformesDiarios.AsNoTracking()
            .Where(i => i.ProyectoId == p.Id)
            .MaxAsync(i => (DateTime?)i.Fecha);
        ind.UltimoInforme = ultimoInforme?.Date;
        if (ultimoInforme is { } ui && (ind.UltimoDatoAvance is null || ui.Date > ind.UltimoDatoAvance))
            ind.UltimoDatoAvance = ui.Date;

        // ── Costos (seguimiento de pagos → presupuesto) ──
        await costoSvc.SincronizarEjecutadoDesdeFlujoAsync(p.Id);
        var trm = p.TasaCambioCOPUSD > 0 ? p.TasaCambioCOPUSD : 4000m;
        // Categoría = partida principal (mismo agrupamiento del Consolidado)
        var principales = await db.Partidas.AsNoTracking()
            .Where(x => x.ProyectoId == p.Id && x.EsPrincipal)
            .Select(x => new { x.Id, Nombre = string.IsNullOrWhiteSpace(x.Categoria) ? x.Descripcion : x.Categoria })
            .ToDictionaryAsync(x => x.Id, x => x.Nombre);
        var partidas = (await db.Partidas.AsNoTracking()
                .Where(x => x.ProyectoId == p.Id && !x.EsPrincipal && !x.Codigo.Contains("-"))
                .Select(x => new { x.PadreId, x.MonedaOriginal, x.CantidadPresupuestada, x.PrecioUnitario, x.ValorEjecutado, x.MontoComprometido })
                .ToListAsync())
            .Select(x => new
            {
                Categoria = x.PadreId is int pid && principales.TryGetValue(pid, out var n) ? n : null,
                x.MonedaOriginal, x.CantidadPresupuestada, x.PrecioUnitario, x.ValorEjecutado, x.MontoComprometido
            })
            .ToList();
        decimal Cop(decimal v, string moneda) => moneda == "USD" ? v * trm : v;
        var porCategoria = new Dictionary<string, CostoCategoria>();
        foreach (var x in partidas)
        {
            var ppto = Cop(x.CantidadPresupuestada * x.PrecioUnitario, x.MonedaOriginal);
            var ej = Cop(x.ValorEjecutado, x.MonedaOriginal);
            var co = Cop(x.MontoComprometido, x.MonedaOriginal);
            var sobre = Math.Max(0m, ej + co - ppto);
            ind.Presupuesto += ppto; ind.Ejecutado += ej; ind.Comprometido += co; ind.Sobrecosto += sobre;
            var cat = string.IsNullOrWhiteSpace(x.Categoria) ? "Sin categoría" : x.Categoria.Trim();
            if (!porCategoria.TryGetValue(cat, out var c)) porCategoria[cat] = c = new CostoCategoria { Categoria = cat };
            c.Presupuesto += ppto; c.Ejecutado += ej; c.Comprometido += co; c.Sobrecosto += sobre;
        }
        ind.TieneCostos = ind.Presupuesto > 0;
        ind.CostosPorCategoria = porCategoria.Values.Where(c => c.Presupuesto > 0 || c.Ejecutado + c.Comprometido > 0)
            .OrderByDescending(c => c.Presupuesto).ToList();
        ind.FechaCorteCostos = p.FechaCorteTesoreria?.Date;

        // ── Bloqueos y decisiones (restricciones abiertas) ──
        var abiertas = await db.Restricciones.AsNoTracking()
            .Where(r => r.ProyectoId == p.Id && (r.Estado == EstadoRestriccion.Abierta || r.Estado == EstadoRestriccion.EnGestion))
            .ToListAsync();
        ind.RestriccionesAbiertas = abiertas.Count;
        ind.RestriccionesVencidas = abiertas.Count(r => r.FechaCompromiso is { } f && f.Date < hoy);
        var principal = abiertas
            .OrderBy(r => r.FechaCompromiso is { } f && f.Date < hoy ? 0 : 1)
            .ThenBy(r => r.FechaCompromiso ?? DateTime.MaxValue)
            .ThenBy(r => r.FechaIdentificacion)
            .FirstOrDefault();
        if (principal is not null)
        {
            ind.PrincipalBloqueo = principal.Descripcion;
            ind.BloqueoNumero = principal.Numero;
            ind.ResponsableBloqueo = principal.Responsable;
            ind.PlanBloqueo = principal.Plan;
            ind.ImpactoBloqueo = principal.Impacto;
            ind.BloqueoDiasVencido = principal.FechaCompromiso is { } fcomp && fcomp.Date < hoy ? (int)(hoy - fcomp.Date).TotalDays : null;
        }

        Clasificar(ind, hoy);
        return ind;
    }

    // Calidad del dato y estado de desempeño se muestran por separado: un proyecto sin datos recientes
    // no se presenta como "saludable" ni como "crítico".
    private static void Clasificar(IndicadoresProyecto ind, DateTime hoy)
    {
        if (!ind.TieneCronograma)
            ind.CalidadDato = "Sin información";
        else if (ind.UltimoDatoAvance is not { } u || (hoy - u).TotalDays > DiasDatoDesactualizado)
            ind.CalidadDato = "Sin datos recientes";
        else if ((hoy - u).TotalDays > DiasDatoActualizado)
            ind.CalidadDato = "Desactualizado";
        else
            ind.CalidadDato = "Actualizado";

        var porIniciar = ind.EstadoProyecto == EstadoProyecto.Planificacion && ind.AvanceReal == 0
                         || ind.FechaInicio is { } fi && fi > hoy;
        if (!ind.TieneCronograma)
        {
            ind.Estado = "Sin información";
            ind.Motivo = "no tiene cronograma cargado";
        }
        else if (porIniciar)
        {
            ind.Estado = "Por iniciar";
            ind.Motivo = ind.FechaInicio is { } f ? $"inicia el {f:dd/MM/yyyy}" : "aún no inicia";
        }
        else if (ind.RetrasoDias > 30 || ind.Brecha <= -10 || ind.BloqueoDiasVencido > 0 && ind.Brecha <= -3)
        {
            ind.Estado = "Crítico";
            ind.Motivo = ind.RetrasoDias > 30 ? $"el cronograma vigente termina {ind.RetrasoDias} días después de la fecha contractual"
                : ind.Brecha <= -10 ? $"brecha de avance de {ind.Brecha:0.0} p.p."
                : $"bloqueo vencido hace {ind.BloqueoDiasVencido} días con brecha de {ind.Brecha:0.0} p.p.";
        }
        else if (ind.RetrasoDias > 0 || ind.Brecha <= -3)
        {
            ind.Estado = "En riesgo";
            ind.Motivo = ind.RetrasoDias > 0 ? $"el cronograma vigente termina {ind.RetrasoDias} días después de la fecha contractual"
                : $"brecha de avance de {ind.Brecha:0.0} p.p.";
        }
        else
        {
            ind.Estado = "Saludable";
            ind.Motivo = "avance y plazo dentro de las tolerancias";
        }
    }
}

public class PortafolioVM
{
    public DateTime FechaCorte { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public List<IndicadoresProyecto> Proyectos { get; set; } = [];
}

public class IndicadoresProyecto
{
    public int ProyectoId { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Cliente { get; set; } = "";
    public decimal CapacidadKWp { get; set; }
    public EstadoProyecto EstadoProyecto { get; set; }

    public bool TieneCronograma { get; set; }
    public decimal AvanceProgramado { get; set; }
    public decimal AvanceReal { get; set; }
    public decimal Brecha { get; set; }
    public decimal? Indice { get; set; }
    public string? VersionCronograma { get; set; }

    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaContractual { get; set; }
    public DateTime? FechaProyectada { get; set; }
    public int? RetrasoDias { get; set; }

    public bool TieneCostos { get; set; }
    public decimal Presupuesto { get; set; }
    public decimal Ejecutado { get; set; }
    public decimal Comprometido { get; set; }
    public decimal Sobrecosto { get; set; }
    public decimal EstimadoCierre => Presupuesto + Sobrecosto;
    public DateTime? FechaCorteCostos { get; set; }
    public List<CostoCategoria> CostosPorCategoria { get; set; } = [];

    public int RestriccionesAbiertas { get; set; }
    public int RestriccionesVencidas { get; set; }
    public string? PrincipalBloqueo { get; set; }
    public string? BloqueoNumero { get; set; }
    public string? ResponsableBloqueo { get; set; }
    public string? PlanBloqueo { get; set; }
    public string? ImpactoBloqueo { get; set; }
    public int? BloqueoDiasVencido { get; set; }

    public DateTime? UltimoInforme { get; set; }
    public DateTime? UltimoDatoAvance { get; set; }
    public string CalidadDato { get; set; } = "Sin información";
    public string Estado { get; set; } = "Sin información";
    public string Motivo { get; set; } = "";

    public bool EnEjecucion => Estado is "Saludable" or "En riesgo" or "Crítico";
    public string UrlDashboard => $"/proyectos/{ProyectoId}/dashboard";
}

public class CostoCategoria
{
    public string Categoria { get; set; } = "";
    public decimal Presupuesto { get; set; }
    public decimal Ejecutado { get; set; }
    public decimal Comprometido { get; set; }
    public decimal Sobrecosto { get; set; }
    public decimal Consumo => Presupuesto > 0 ? Math.Round((Ejecutado + Comprometido) / Presupuesto * 100, 0) : 0;
}
