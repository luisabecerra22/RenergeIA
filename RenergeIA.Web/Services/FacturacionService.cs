using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Helpers;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

/// <summary>
/// C3 · Avance de facturación: contratos, ítems ligados al cronograma, cierres mensuales con su acta
/// y la factura que queda como ingreso estimado en el flujo de caja (Consolidado).
/// El avance sugerido de cada ítem sale de <see cref="InformeDiarioService.AvanceActividadesAFechaAsync"/>
/// (misma lógica de la Curva S y el Dashboard), promediado por duración de las actividades hoja asociadas.
/// </summary>
public class FacturacionService(RenergeIADbContext db, InformeDiarioService informes)
{
    public record Sugerencia(decimal? Plan, decimal? Real, int Actividades);

    public record FilaMes(DateTime Corte, decimal PctPlan, decimal PctReal, bool Proyectado,
        decimal Facturable, decimal Total, CorteFacturacion? CorteGuardado);

    public record IngresoFacturado(DateTime FechaPago, DateTime? FechaFactura, string Moneda, decimal Total,
        string? NumeroFactura, string Contrato, string Concepto);

    // ==================== Contratos e ítems ====================

    public Task<List<ContratoFacturacion>> ContratosAsync(int proyectoId) =>
        db.ContratosFacturacion
            .Include(c => c.Items.OrderBy(i => i.Orden))
            .Include(c => c.Cortes).ThenInclude(k => k.Detalles)
            .Where(c => c.ProyectoId == proyectoId)
            .OrderBy(c => c.Orden).ThenBy(c => c.Id)
            .AsSplitQuery()
            .ToListAsync();

    public async Task<ContratoFacturacion> CrearContratoAsync(int proyectoId, string nombre, string moneda)
    {
        var orden = await db.ContratosFacturacion.CountAsync(c => c.ProyectoId == proyectoId);
        var c = new ContratoFacturacion { ProyectoId = proyectoId, Nombre = nombre, Moneda = moneda, Orden = orden };
        db.ContratosFacturacion.Add(c);
        await db.SaveChangesAsync();
        return c;
    }

    public async Task GuardarAsync() => await db.SaveChangesAsync();

    public async Task EliminarContratoAsync(ContratoFacturacion c)
    {
        c.Eliminado = true;
        c.FechaModificacion = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task<ItemFacturacion> AgregarItemAsync(ContratoFacturacion c)
    {
        var item = new ItemFacturacion
        {
            ContratoFacturacionId = c.Id,
            Codigo = (c.Items.Count + 1).ToString(),
            Nombre = "Nuevo ítem",
            Orden = c.Items.Count == 0 ? 0 : c.Items.Max(i => i.Orden) + 1
        };
        c.Items.Add(item);
        await db.SaveChangesAsync();
        return item;
    }

    /// <summary>Un ítem ya usado en un acta no se borra (quedaría el acta sin su detalle).</summary>
    public async Task<string?> EliminarItemAsync(ContratoFacturacion c, ItemFacturacion item)
    {
        var usado = await db.DetallesCorteFacturacion.AnyAsync(d => d.ItemFacturacionId == item.Id
            && d.Corte.Estado != EstadoCorteFacturacion.Borrador);
        if (usado) return "Este ítem ya está en un acta generada; no se puede eliminar.";
        var borradores = await db.DetallesCorteFacturacion.Where(d => d.ItemFacturacionId == item.Id).ToListAsync();
        db.DetallesCorteFacturacion.RemoveRange(borradores);
        c.Items.Remove(item);
        db.ItemsFacturacion.Remove(item);
        await db.SaveChangesAsync();
        return null;
    }

    /// <summary>Actividades del cronograma vigente (hojas y agrupaciones) para asociar a los ítems.</summary>
    public async Task<List<(string Codigo, string Nombre)>> ActividadesCronogramaAsync(int proyectoId)
    {
        var version = await db.CronogramasVersion.FirstOrDefaultAsync(v => v.ProyectoId == proyectoId && v.EsVigente);
        var acts = await db.ActividadesWBS.AsNoTracking()
            .Where(a => a.ProyectoId == proyectoId && a.Activo && (version == null || a.CronogramaVersionId == version.Id))
            .Select(a => new { a.CodigoWBS, a.Nombre })
            .ToListAsync();
        return acts
            .OrderBy(a => a.CodigoWBS, Comparer<string>.Create(CompararCodigo))
            .Select(a => (a.CodigoWBS, a.Nombre))
            .ToList();
    }

    // ==================== Avance sugerido desde el cronograma ====================

    /// <summary>
    /// % planeado y real de cada ítem a una fecha, promediando por duración las actividades hoja
    /// asociadas (un código de agrupación incluye todas sus hojas). Real = null en fechas futuras.
    /// </summary>
    public async Task<Dictionary<int, Sugerencia>> SugerenciasAsync(int proyectoId, IEnumerable<ItemFacturacion> items, DateTime fecha)
    {
        var hojas = await informes.AvanceActividadesAFechaAsync(proyectoId, fecha);
        var res = new Dictionary<int, Sugerencia>();
        foreach (var it in items)
        {
            var codigos = CalculoFacturacion.CodigosWbs(it.ActividadesWbs);
            var ligadas = hojas.Where(h => codigos.Any(c => h.Codigo == c || h.Codigo.StartsWith(c + "."))).ToList();
            if (ligadas.Count == 0) { res[it.Id] = new Sugerencia(null, null, 0); continue; }
            decimal pesoTotal = 0, plan = 0, real = 0;
            var hayFuturo = false;
            foreach (var h in ligadas)
            {
                var peso = Math.Max(1, (decimal)(h.Fin - h.Inicio).TotalDays);
                pesoTotal += peso;
                plan += peso * h.Programado;
                if (h.Real is decimal r) real += peso * r; else hayFuturo = true;
            }
            res[it.Id] = new Sugerencia(
                Math.Round(plan / pesoTotal, 1),
                hayFuturo ? null : Math.Round(real / pesoTotal, 1),
                ligadas.Count);
        }
        return res;
    }

    // ==================== Cierres mensuales ====================

    private static decimal PctAnterior(ContratoFacturacion c, int itemId, DateTime fechaCorte) =>
        c.Cortes
            .Where(k => k.Tipo == TipoCorteFacturacion.Mensual && k.Estado != EstadoCorteFacturacion.Borrador && k.FechaCorte < fechaCorte)
            .OrderByDescending(k => k.FechaCorte)
            .Select(k => k.Detalles.FirstOrDefault(d => d.ItemFacturacionId == itemId)?.PctReal)
            .FirstOrDefault(p => p is not null) ?? 0m;

    /// <summary>Hitos finales ya facturados en otro corte (no se vuelven a facturar).</summary>
    public static (bool Mec, bool Sus, bool Acep) HitosYaFacturados(ContratoFacturacion c, CorteFacturacion? excepto)
    {
        var otros = c.Cortes.Where(k => k.Tipo == TipoCorteFacturacion.Mensual && k.Estado != EstadoCorteFacturacion.Borrador && k != excepto).ToList();
        return (otros.Any(k => k.CulminacionMecanica), otros.Any(k => k.CulminacionSustancial), otros.Any(k => k.AceptacionProvisional));
    }

    /// <summary>
    /// Abre (o refresca) el cierre del mes. Mientras está en borrador el % real viene del cronograma,
    /// salvo los ítems ajustados a mano; un cierre con acta queda fijo.
    /// </summary>
    public async Task<CorteFacturacion> AbrirCorteAsync(ContratoFacturacion c, DateTime mes)
    {
        var fin = CalculoFacturacion.FinDeMes(mes);
        var corte = c.Cortes.FirstOrDefault(k => k.Tipo == TipoCorteFacturacion.Mensual && k.FechaCorte == fin);
        if (corte is null)
        {
            corte = new CorteFacturacion { ContratoFacturacionId = c.Id, FechaCorte = fin, Tipo = TipoCorteFacturacion.Mensual };
            c.Cortes.Add(corte);
        }
        if (corte.Estado != EstadoCorteFacturacion.Borrador) return corte;

        var sug = await SugerenciasAsync(c.ProyectoId, c.Items, fin);
        foreach (var it in c.Items)
        {
            var d = corte.Detalles.FirstOrDefault(x => x.ItemFacturacionId == it.Id);
            if (d is null) { d = new DetalleCorteFacturacion { ItemFacturacionId = it.Id }; corte.Detalles.Add(d); }
            var s = sug.GetValueOrDefault(it.Id);
            d.PctAnterior = PctAnterior(c, it.Id, fin);
            d.PctPlaneado = s?.Plan ?? 0;
            d.PctCronograma = s?.Real ?? s?.Plan;
            if (!d.AjustadoManual)
                d.PctReal = Math.Max(d.PctAnterior, d.PctCronograma ?? d.PctAnterior);
        }
        foreach (var huerfano in corte.Detalles.Where(d => c.Items.All(i => i.Id != d.ItemFacturacionId)).ToList())
            corte.Detalles.Remove(huerfano);
        Recalcular(c, corte);
        await db.SaveChangesAsync();
        return corte;
    }

    public static CalculoFacturacion.Resultado Calcular(ContratoFacturacion c, CorteFacturacion corte)
    {
        var lineas = c.Items.Select(it =>
        {
            var d = corte.Detalles.FirstOrDefault(x => x.ItemFacturacionId == it.Id);
            return new CalculoFacturacion.LineaEntrada(it.Id, it.Valor, it.Regimen, d?.PctAnterior ?? 0, d?.PctReal ?? 0);
        });
        return CalculoFacturacion.Calcular(c, lineas, corte.CulminacionMecanica, corte.CulminacionSustancial, corte.AceptacionProvisional);
    }

    public static void Recalcular(ContratoFacturacion c, CorteFacturacion corte)
    {
        if (corte.Tipo == TipoCorteFacturacion.Anticipo) return;
        var r = Calcular(c, corte);
        foreach (var l in r.Lineas)
        {
            var d = corte.Detalles.FirstOrDefault(x => x.ItemFacturacionId == l.ItemId);
            if (d is not null) d.Monto = l.Monto;
        }
        corte.BaseAvance = r.BaseAvance;
        corte.BaseHitos = r.BaseHitos;
        corte.ValorAiu = r.Aiu;
        corte.ValorIva = r.Iva;
        corte.Amortizacion = r.Amortizacion;
        corte.Total = r.Total;
        corte.FechaModificacion = DateTime.UtcNow;
    }

    public async Task AjustarPctAsync(ContratoFacturacion c, CorteFacturacion corte, DetalleCorteFacturacion d, decimal? pct, string usuario)
    {
        if (corte.Estado != EstadoCorteFacturacion.Borrador) return;
        if (pct is null)
        {
            d.AjustadoManual = false;
            d.AjustadoPor = null;
            d.PctReal = Math.Max(d.PctAnterior, d.PctCronograma ?? d.PctAnterior);
        }
        else
        {
            d.AjustadoManual = true;
            d.AjustadoPor = usuario;
            d.PctReal = Math.Clamp(pct.Value, 0, 100);
        }
        Recalcular(c, corte);
        await db.SaveChangesAsync();
    }

    public async Task MarcarHitoAsync(ContratoFacturacion c, CorteFacturacion corte, string hito, bool valor)
    {
        if (corte.Estado != EstadoCorteFacturacion.Borrador) return;
        switch (hito)
        {
            case "mec": corte.CulminacionMecanica = valor; break;
            case "sus": corte.CulminacionSustancial = valor; break;
            case "acep": corte.AceptacionProvisional = valor; break;
        }
        Recalcular(c, corte);
        await db.SaveChangesAsync();
    }

    public async Task GenerarActaAsync(ContratoFacturacion c, CorteFacturacion corte, string usuario)
    {
        if (corte.Estado != EstadoCorteFacturacion.Borrador) return;
        Recalcular(c, corte);
        corte.NumeroActa = (c.Cortes.Where(k => k.Tipo == TipoCorteFacturacion.Mensual).Max(k => k.NumeroActa) ?? 0) + 1;
        corte.FechaActa = DateTime.UtcNow;
        corte.ActaGeneradaPor = usuario;
        corte.Estado = EstadoCorteFacturacion.ActaGenerada;
        await db.SaveChangesAsync();
    }

    /// <summary>Devuelve un acta sin factura a borrador (para corregirla). Solo si es la última.</summary>
    public async Task<string?> ReabrirActaAsync(ContratoFacturacion c, CorteFacturacion corte)
    {
        if (corte.Estado != EstadoCorteFacturacion.ActaGenerada) return "Solo se puede reabrir un acta sin factura.";
        if (c.Cortes.Any(k => k.Tipo == TipoCorteFacturacion.Mensual && k.FechaCorte > corte.FechaCorte && k.Estado != EstadoCorteFacturacion.Borrador))
            return "Hay actas posteriores; reabra primero la más reciente.";
        corte.Estado = EstadoCorteFacturacion.Borrador;
        if (corte.Tipo == TipoCorteFacturacion.Mensual) corte.NumeroActa = null;
        await db.SaveChangesAsync();
        return null;
    }

    public async Task RegistrarFacturaAsync(ContratoFacturacion c, CorteFacturacion corte, string numero, DateTime fecha, int plazo, string usuario)
    {
        if (corte.Estado == EstadoCorteFacturacion.Borrador) return;
        corte.NumeroFactura = numero.Trim();
        corte.FechaFactura = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
        corte.PlazoPagoDias = plazo;
        corte.FechaPagoEstimada = DateTime.SpecifyKind(fecha.Date.AddDays(plazo), DateTimeKind.Utc);
        corte.FacturaRegistradaPor = usuario;
        corte.Estado = EstadoCorteFacturacion.Facturada;
        await db.SaveChangesAsync();
    }

    public async Task QuitarFacturaAsync(CorteFacturacion corte)
    {
        if (corte.Estado != EstadoCorteFacturacion.Facturada) return;
        corte.NumeroFactura = null;
        corte.FechaFactura = null;
        corte.FechaPagoEstimada = null;
        corte.Estado = EstadoCorteFacturacion.ActaGenerada;
        await db.SaveChangesAsync();
    }

    /// <summary>El acta del anticipo (valor de venta × % de anticipo).</summary>
    public async Task<CorteFacturacion> AnticipoAsync(ContratoFacturacion c, string usuario)
    {
        var a = c.Cortes.FirstOrDefault(k => k.Tipo == TipoCorteFacturacion.Anticipo);
        if (a is not null) return a;
        var fecha = (c.FechaContrato ?? DateTime.UtcNow).Date;
        a = new CorteFacturacion
        {
            ContratoFacturacionId = c.Id,
            Tipo = TipoCorteFacturacion.Anticipo,
            FechaCorte = DateTime.SpecifyKind(fecha, DateTimeKind.Utc),
            Estado = EstadoCorteFacturacion.ActaGenerada,
            NumeroActa = 0,
            FechaActa = DateTime.UtcNow,
            ActaGeneradaPor = usuario,
            Total = CalculoFacturacion.ValorAnticipo(c)
        };
        c.Cortes.Add(a);
        await db.SaveChangesAsync();
        return a;
    }

    // ==================== Resumen mes a mes ====================

    /// <summary>
    /// Mes a mes del contrato: los cierres con acta usan lo guardado; los demás se simulan con el
    /// cronograma (real hasta hoy, programado después = proyectado), encadenando lo ya facturado.
    /// </summary>
    public async Task<List<FilaMes>> ResumenMesAMesAsync(ContratoFacturacion c, DateTime desde, DateTime hasta)
    {
        var filas = new List<FilaMes>();
        var anterior = c.Items.ToDictionary(i => i.Id, _ => 0m);
        var hoy = SeguimientoDocumento.HoyColombia();
        var pesoTotal = c.Items.Sum(i => i.Valor);
        var hitosPrev = (Mec: false, Sus: false, Acep: false);
        var mes = new DateTime(desde.Year, desde.Month, 1);
        var limite = new DateTime(hasta.Year, hasta.Month, 1);
        for (var n = 0; mes <= limite && n < 48; n++, mes = mes.AddMonths(1))
        {
            var fin = CalculoFacturacion.FinDeMes(mes);
            var guardado = c.Cortes.FirstOrDefault(k => k.Tipo == TipoCorteFacturacion.Mensual && k.FechaCorte == fin);
            var sug = await SugerenciasAsync(c.ProyectoId, c.Items, fin);
            var plan = pesoTotal == 0 ? 0 : c.Items.Sum(i => i.Valor * (sug.GetValueOrDefault(i.Id)?.Plan ?? 0)) / pesoTotal;

            if (guardado is not null && guardado.Estado != EstadoCorteFacturacion.Borrador)
            {
                foreach (var d in guardado.Detalles) anterior[d.ItemFacturacionId] = d.PctReal;
                var realG = pesoTotal == 0 ? 0 : c.Items.Sum(i => i.Valor * anterior.GetValueOrDefault(i.Id)) / pesoTotal;
                hitosPrev = (hitosPrev.Mec || guardado.CulminacionMecanica, hitosPrev.Sus || guardado.CulminacionSustancial, hitosPrev.Acep || guardado.AceptacionProvisional);
                filas.Add(new FilaMes(fin, plan, realG, false, guardado.BaseAvance + guardado.BaseHitos, guardado.Total, guardado));
                continue;
            }

            var proyectado = fin > hoy;
            var lineas = new List<CalculoFacturacion.LineaEntrada>();
            var nuevos = new Dictionary<int, decimal>();
            foreach (var it in c.Items)
            {
                var s = sug.GetValueOrDefault(it.Id);
                var det = guardado?.Detalles.FirstOrDefault(d => d.ItemFacturacionId == it.Id);
                var real = det is { AjustadoManual: true } ? det.PctReal
                    : (proyectado ? s?.Plan : s?.Real ?? s?.Plan) ?? anterior.GetValueOrDefault(it.Id);
                real = Math.Max(real, anterior.GetValueOrDefault(it.Id));
                lineas.Add(new CalculoFacturacion.LineaEntrada(it.Id, it.Valor, it.Regimen, anterior.GetValueOrDefault(it.Id), real));
                nuevos[it.Id] = real;
            }
            var mec = guardado?.CulminacionMecanica == true && !hitosPrev.Mec;
            var sus = guardado?.CulminacionSustancial == true && !hitosPrev.Sus;
            var acep = guardado?.AceptacionProvisional == true && !hitosPrev.Acep;
            var r = CalculoFacturacion.Calcular(c, lineas, mec, sus, acep);
            foreach (var kv in nuevos) anterior[kv.Key] = kv.Value;
            var realPct = pesoTotal == 0 ? 0 : c.Items.Sum(i => i.Valor * anterior.GetValueOrDefault(i.Id)) / pesoTotal;
            filas.Add(new FilaMes(fin, plan, realPct, proyectado, r.BaseAvance + r.BaseHitos, r.Total, guardado));
        }
        return filas;
    }

    /// <summary>Rango de meses del C3: del inicio del contrato o del proyecto al fin del cronograma.</summary>
    public async Task<(DateTime Desde, DateTime Hasta)> RangoMesesAsync(ContratoFacturacion c)
    {
        var p = await db.Proyectos.AsNoTracking().FirstAsync(x => x.Id == c.ProyectoId);
        var version = await db.CronogramasVersion.AsNoTracking().FirstOrDefaultAsync(v => v.ProyectoId == c.ProyectoId && v.EsVigente);
        var finCrono = await db.ActividadesWBS.AsNoTracking()
            .Where(a => a.ProyectoId == c.ProyectoId && a.Activo && (version == null || a.CronogramaVersionId == version.Id))
            .MaxAsync(a => (DateTime?)a.FechaFinPlaneada);
        var desde = new[] { c.FechaContrato, p.FechaInicioPlaneada == default ? null : p.FechaInicioPlaneada }
            .Where(f => f is not null).Select(f => f!.Value).DefaultIfEmpty(DateTime.Today).Min();
        var primerCorte = c.Cortes.Where(k => k.Tipo == TipoCorteFacturacion.Mensual).Select(k => (DateTime?)k.FechaCorte).Min();
        if (primerCorte is DateTime pc && pc < desde) desde = pc;
        var hasta = new[] { finCrono, p.FechaFinPlaneada == default ? null : p.FechaFinPlaneada, DateTime.Today }
            .Where(f => f is not null).Select(f => f!.Value).Max();
        return (desde, hasta);
    }

    // ==================== Ingresos para el flujo de caja ====================

    /// <summary>Facturas registradas en el C3 del proyecto, con su fecha estimada de pago.</summary>
    public async Task<List<IngresoFacturado>> IngresosFacturadosAsync(int proyectoId)
    {
        var cortes = await db.CortesFacturacion.AsNoTracking()
            .Include(k => k.Contrato)
            .Where(k => k.Contrato.ProyectoId == proyectoId && !k.Contrato.Eliminado
                     && k.Estado == EstadoCorteFacturacion.Facturada && k.FechaPagoEstimada != null)
            .ToListAsync();
        return cortes.Select(k => new IngresoFacturado(
                k.FechaPagoEstimada!.Value, k.FechaFactura, k.Contrato.Moneda, k.Total, k.NumeroFactura, k.Contrato.Nombre,
                k.Tipo == TipoCorteFacturacion.Anticipo ? "Anticipo" : $"Acta {k.NumeroActa} · {k.FechaCorte:MMM yyyy}"))
            .OrderBy(i => i.FechaPago)
            .ToList();
    }

    private static int CompararCodigo(string? x, string? y)
    {
        var a = (x ?? "").Split('.');
        var b = (y ?? "").Split('.');
        for (var i = 0; i < Math.Min(a.Length, b.Length); i++)
        {
            var c = int.TryParse(a[i], out var na) && int.TryParse(b[i], out var nb)
                ? na.CompareTo(nb) : string.Compare(a[i], b[i], StringComparison.OrdinalIgnoreCase);
            if (c != 0) return c;
        }
        return a.Length.CompareTo(b.Length);
    }
}
