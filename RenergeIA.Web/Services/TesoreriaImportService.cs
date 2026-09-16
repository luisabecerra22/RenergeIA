using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

// Importación única del Forecast Control de tesorería: actualiza Flujo de Caja y Compromisos a la vez
public partial class TesoreriaImportService(RenergeIADbContext db, CostoService costoSvc)
{
    public sealed record VistaPrevia(
        string Archivo,
        DateTime? FechaDocumento,
        DateTime? FechaCargada,
        string? ArchivoCargado,
        TesoreriaParser.ResultadoTesoreria Flujo,
        List<TesoreriaCompromisosParser.Bloque> Bloques)
    {
        public bool EsAnterior => FechaDocumento.HasValue && FechaCargada.HasValue && FechaDocumento.Value < FechaCargada.Value;
        public bool SinFecha => FechaDocumento is null;
        public int Ocs => Bloques.Count(b => b.Grupo == "OC");
        public int OcsEnProceso => Bloques.Count(b => b.Grupo == "OC" && b.EnProceso);
        public int FacturasSinCodigo => Bloques.Where(b => b.Grupo == "OC").Sum(b => b.Hitos.Count(h => h.Codigo is null));
    }

    public sealed record Resultado(
        int ValoresFlujo,
        List<KeyValuePair<string, List<TesoreriaParser.MovimientoTesoreria>>> NoMapeados,
        int OcsNuevas,
        int OcsActualizadas,
        int OcsEnProcesoSinAsignar,
        int FacturasSinCodigo,
        int LineasSinRubro);

    [GeneratedRegex(@"(\d{2})\.(\d{2})\.(\d{4})")]
    private static partial Regex RegexFechaArchivo();

    // Fecha del corte tomada del nombre del archivo en formato MM.DD.AAAA
    public static DateTime? FechaDesdeNombre(string nombre)
    {
        var m = RegexFechaArchivo().Match(nombre);
        if (!m.Success) return null;
        return DateTime.TryParseExact($"{m.Groups[1].Value}/{m.Groups[2].Value}/{m.Groups[3].Value}", "MM/dd/yyyy",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var f) ? f : null;
    }

    public async Task<VistaPrevia> LeerAsync(int proyectoId, Stream archivo, string nombre)
    {
        using var ms = new MemoryStream();
        await archivo.CopyToAsync(ms);
        ms.Position = 0;
        using var wb = new XLWorkbook(ms);

        var proyecto = await db.Proyectos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == proyectoId);
        return new VistaPrevia(
            nombre,
            FechaDesdeNombre(nombre),
            proyecto?.FechaCorteTesoreria,
            proyecto?.ArchivoTesoreria,
            TesoreriaParser.Parse(wb),
            TesoreriaCompromisosParser.Parse(wb));
    }

    public async Task<Resultado> AplicarAsync(int proyectoId, VistaPrevia vista)
    {
        var (valores, noMapeados) = await AplicarFlujoAsync(proyectoId, vista);
        var (nuevas, actualizadas, enProceso, sinCodigo, sinRubro) = await AplicarCompromisosAsync(proyectoId, vista.Bloques);

        var proyecto = await db.Proyectos.FindAsync(proyectoId);
        if (proyecto is not null)
        {
            if (vista.FechaDocumento.HasValue) proyecto.FechaCorteTesoreria = vista.FechaDocumento;
            proyecto.ArchivoTesoreria = vista.Archivo.Length > 250 ? vista.Archivo[..250] : vista.Archivo;
            await db.SaveChangesAsync();
        }

        await costoSvc.SincronizarEjecutadoDesdeFlujoAsync(proyectoId);
        return new Resultado(valores, noMapeados, nuevas, actualizadas, enProceso, sinCodigo, sinRubro);
    }

    // ── Flujo de Caja ───────────────────────────────────────────────────────

    private async Task<(int Valores, List<KeyValuePair<string, List<TesoreriaParser.MovimientoTesoreria>>> NoMapeados)>
        AplicarFlujoAsync(int proyectoId, VistaPrevia vista)
    {
        var res = vista.Flujo;
        var subPartidas = await db.Partidas.AsNoTracking()
            .Where(p => p.ProyectoId == proyectoId && !p.EsPrincipal)
            .ToListAsync();

        // Destinos: los códigos del listado (sub-partidas sin consecutivo "-")
        var porCodigo = subPartidas
            .Where(p => !p.Codigo.Contains('-'))
            .GroupBy(p => p.Codigo.ToUpperInvariant())
            .ToDictionary(g => g.Key, g => g.First());

        var mapeosDb = (await db.MapeosCodigoTesoreria.AsNoTracking().ToListAsync())
            .GroupBy(m => m.CodigoTesoreria.ToUpperInvariant())
            .ToDictionary(g => g.Key, g => g.First().Rubro.ToUpperInvariant());

        // Prioridad: 1) clasificación guardada por la usuaria, 2) tabla de Codificación
        // (solo si ese código existe en el presupuesto), 3) el mismo código
        string RubroDe(string codigo)
        {
            var cod = codigo.ToUpperInvariant();
            if (mapeosDb.TryGetValue(cod, out var m)) return m;
            if (CodificacionSeeder.MapaCodigoARubro.TryGetValue(cod, out var r) && porCodigo.ContainsKey(r.ToUpperInvariant()))
                return r.ToUpperInvariant();
            return cod;
        }

        var celdas = new Dictionary<(int PartidaId, DateTime Fecha, string Moneda), decimal>();
        var noMapeados = new Dictionary<string, List<TesoreriaParser.MovimientoTesoreria>>(StringComparer.OrdinalIgnoreCase);
        foreach (var mov in res.Movimientos)
        {
            if (!porCodigo.TryGetValue(RubroDe(mov.Codigo), out var destino))
            {
                if (!noMapeados.TryGetValue(mov.Codigo, out var lista))
                    noMapeados[mov.Codigo] = lista = [];
                lista.Add(mov);
                continue;
            }
            var key = (destino.Id, mov.FechaCorte, mov.Moneda);
            celdas[key] = celdas.GetValueOrDefault(key) + mov.Monto;
        }

        // Reemplazo total: el corte más reciente trae todo el histórico actualizado
        var pagosPrevios = await db.PagosCorteSemanal.Where(p => p.ProyectoId == proyectoId).ToListAsync();
        db.PagosCorteSemanal.RemoveRange(pagosPrevios);
        foreach (var ((pid, fecha, mon), monto) in celdas)
        {
            if (monto == 0) continue;
            db.PagosCorteSemanal.Add(new PagoCorteSemanal
            {
                ProyectoId = proyectoId,
                PartidaId = pid,
                FechaCorte = fecha,
                Moneda = mon,
                Monto = monto,
                Descripcion = $"Tesorería {vista.Archivo}"
            });
        }

        var proyecto = await db.Proyectos.FindAsync(proyectoId);
        if (proyecto is not null && res.PrimerCorte.HasValue && res.UltimoCorte.HasValue)
        {
            if (proyecto.FechaInicioPagos is null || proyecto.FechaInicioPagos > res.PrimerCorte) proyecto.FechaInicioPagos = res.PrimerCorte;
            if (proyecto.FechaFinPagos is null || proyecto.FechaFinPagos < res.UltimoCorte) proyecto.FechaFinPagos = res.UltimoCorte;
        }

        await db.SaveChangesAsync();
        return (celdas.Count(c => c.Value != 0), noMapeados.OrderBy(k => k.Key).ToList());
    }

    // ── Compromisos ─────────────────────────────────────────────────────────

    public static string ClaveHito(string claveOc, string? periodo, string? documento, string descripcion) =>
        $"{claveOc}|{periodo}|{documento}|{descripcion}".ToUpperInvariant();

    private async Task<(int Nuevas, int Actualizadas, int EnProceso, int SinCodigo, int SinRubro)>
        AplicarCompromisosAsync(int proyectoId, List<TesoreriaCompromisosParser.Bloque> bloques)
    {
        var existentes = await db.CompromisoCostos
            .Include(c => c.Hitos)
            .Where(c => c.ProyectoId == proyectoId)
            .ToListAsync();
        var asignaciones = await db.AsignacionesTesoreria.AsNoTracking()
            .Where(a => a.ProyectoId == proyectoId)
            .ToListAsync();
        var numeroAsignado = asignaciones.Where(a => a.Tipo == "NumeroOC").ToDictionary(a => a.Clave, a => a.Valor);
        var codigoAsignado = asignaciones.Where(a => a.Tipo == "CodigoHito").ToDictionary(a => a.Clave, a => a.Valor);

        int nuevas = 0, actualizadas = 0, enProceso = 0, sinCodigo = 0, sinRubro = 0;
        var clavesVistas = new HashSet<string>();

        foreach (var b in bloques)
        {
            clavesVistas.Add(b.Clave);
            var numero = b.NumeroOC;
            if (b.EnProceso && numeroAsignado.TryGetValue(b.Clave, out var asignado)) numero = asignado;
            if (b.Grupo == "OC" && numero.Contains("XXX", StringComparison.OrdinalIgnoreCase)) enProceso++;

            var entidad = existentes.FirstOrDefault(c => c.ClaveTesoreria == b.Clave)
                ?? (b.Grupo == "OC" && !numero.Contains("XXX", StringComparison.OrdinalIgnoreCase)
                    ? existentes.FirstOrDefault(c => c.Origen == "Manual" && c.Grupo == "OC" &&
                        string.Equals(c.Codigo, numero, StringComparison.OrdinalIgnoreCase))
                    : null);

            if (entidad is null)
            {
                entidad = new CompromisoCosto { ProyectoId = proyectoId };
                db.CompromisoCostos.Add(entidad);
                existentes.Add(entidad);
                nuevas++;
            }
            else
            {
                db.HitosCompromiso.RemoveRange(entidad.Hitos);
                entidad.Hitos.Clear();
                actualizadas++;
            }

            var hitos = b.Hitos.Select((h, i) =>
            {
                var codigo = h.Codigo?.ToUpperInvariant();
                var manual = false;
                if (codigo is null && codigoAsignado.TryGetValue(ClaveHito(b.Clave, h.Periodo, h.Documento, h.Descripcion), out var cod))
                {
                    codigo = cod;
                    manual = true;
                }
                return new HitoCompromiso
                {
                    Orden = i,
                    Periodo = Corta(h.Periodo, 120),
                    Codigo = codigo,
                    CodigoManual = manual,
                    Descripcion = Corta(h.Descripcion, 400) ?? "",
                    Detalle = Corta(h.Detalle, 300),
                    Documento = Corta(h.Documento, 120),
                    Fecha = h.Fecha,
                    Subtotal = h.Subtotal,
                    Iva = h.Iva,
                    Importe = h.Importe,
                    RetFuente = h.RetFuente,
                    RetIca = h.RetIca,
                    TotalPagar = h.TotalPagar,
                    Pagado = h.Pagado
                };
            }).ToList();

            if (b.Grupo == "OC") sinCodigo += hitos.Count(h => h.Codigo is null);
            else sinRubro += hitos.Count(h => h.Codigo is null);

            var facturado = b.Grupo == "OC" ? b.Facturado : hitos.Sum(h => h.Importe);
            var pagado = hitos.Where(h => h.Pagado).Sum(h => h.Importe);
            var aprobado = b.Grupo == "OC" ? b.Aprobado : facturado;

            entidad.Origen = "Tesoreria";
            entidad.Grupo = b.Grupo;
            entidad.ClaveTesoreria = b.Clave;
            entidad.Codigo = Corta(numero, 50) ?? "";
            entidad.Proveedor = Corta(b.Proveedor, 200) ?? "";
            entidad.DescripcionServicio = Corta(b.Descripcion, 500);
            entidad.Moneda = b.Moneda;
            entidad.Valor = aprobado;
            entidad.ValorFactura = facturado;
            entidad.ValorPagado = pagado;
            entidad.SaldoPorPagar = Math.Max(facturado - pagado, 0) + Math.Max(aprobado - facturado, 0);
            entidad.Fecha = hitos.Where(h => h.Fecha.HasValue).Select(h => h.Fecha!.Value).DefaultIfEmpty(DateTime.Today).Min();
            entidad.NumeroFactura = null;
            entidad.FechaFactura = null;
            entidad.PartidaId = null;
            entidad.Estado = entidad.Codigo.Contains("XXX", StringComparison.OrdinalIgnoreCase) ? EstadoCompromiso.EnProceso
                : entidad.SaldoPorPagar <= 0 && facturado > 0 ? EstadoCompromiso.Pagado
                : EstadoCompromiso.Pendiente;
            foreach (var h in hitos) entidad.Hitos.Add(h);
        }

        // OCs de tesorería que ya no vienen en el archivo; las manuales se conservan
        var obsoletas = existentes.Where(c => c.Origen == "Tesoreria" && c.ClaveTesoreria is not null && !clavesVistas.Contains(c.ClaveTesoreria)).ToList();
        db.CompromisoCostos.RemoveRange(obsoletas);

        await db.SaveChangesAsync();
        return (nuevas, actualizadas, enProceso, sinCodigo, sinRubro);
    }

    private static string? Corta(string? s, int max) => s is null ? null : s.Length > max ? s[..max] : s;
}
