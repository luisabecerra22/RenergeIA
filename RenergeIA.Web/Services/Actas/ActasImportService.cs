using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;
using static RenergeIA.Web.Services.Actas.ActasZipParser;

namespace RenergeIA.Web.Services.Actas;

// Importación desde Actas IA. Reglas (definidas con la usuaria):
// - Repetible sin duplicar: actas por Codigo, compromisos por IdOrigen, archivos por (acta, tipo).
// - El documento histórico no se reescribe: si un archivo ya migrado difiere, se reporta como conflicto.
// - El estado del acta ya existente en la app SE CONSERVA (igual que subir_acta.js con el estado remoto).
// - Un compromiso con gestión hecha EN LA APP (actualizaciones propias o edición) no se pisa con el export.
// - Un compromiso cuyo continuidadDe resuelve a otro compromiso NO crea tarjeta nueva: queda como
//   actualización del compromiso original (identidad desde la reunión de origen hasta el cierre).
// - Conflictos y referencias ambiguas se LISTAN, no se deciden en silencio.
// - Cada aplicación guarda un LoteImportacionActas con respaldo para revertir.
public class ActasImportService(RenergeIADbContext db)
{
    public sealed record Diferencia(string Codigo, string Campo, string? Actual, string? Nuevo);

    public sealed class Reporte
    {
        public int ActasEnPaquete, CompromisosEnPaquete, ArchivosEnPaquete;
        public List<string> ActasNuevas = [];
        public List<string> ActasSinCambio = [];
        public List<Diferencia> ActasConDiferencias = [];
        public List<string> CompromisosNuevos = [];
        public List<string> CompromisosActualizarTablero = [];
        public List<string> CompromisosProtegidos = [];      // con gestión en la app: no se pisan
        public List<string> ContinuidadesResueltas = [];     // idNuevo -> idOriginal
        public List<string> ArchivosNuevos = [];
        public List<string> ArchivosDistintos = [];          // ya migrados y el paquete trae otro contenido
        public List<string> ProyectosNuevos = [];
        public List<string> ProyectosSinMapear = [];         // proyecto del acta sin catálogo / sin proyecto RenergeIA
        public List<string> FechasDescriptivas = [];         // se conservan como texto, sin vencimiento
        public List<string> Conflictos = [];
        public List<string> Advertencias = [];
        // Carga de Word sueltos
        public List<string> ActasComplementadas = [];        // ya existían: solo se agregan documentos/compromisos que falten
        public List<string> CompromisosExistentes = [];      // ya cargados: no se tocan
        public List<string> SeguimientosNuevos = [];         // filas de "Seguimiento de compromisos anteriores" que quedan como actualización
        public List<string> SeguimientosSinOrigen = [];      // el compromiso de origen aún no está en la app
        public int Personas, Alias, Externos;
        public bool TieneExportTablero;
    }

    private static readonly JsonSerializerOptions _json = new() { WriteIndented = false };

    // ── SIMULACIÓN (no escribe nada) ─────────────────────────────────────────
    public async Task<Reporte> SimularAsync(PaqueteActas p)
    {
        var r = new Reporte
        {
            ActasEnPaquete = p.Actas.Count,
            CompromisosEnPaquete = p.Actas.Sum(a => a.Compromisos.Count),
            ArchivosEnPaquete = p.Actas.Sum(a => a.Archivos.Count),
            Personas = p.Personas, Alias = p.Alias, Externos = p.Externos,
            TieneExportTablero = p.Tablero.Count > 0,
        };
        r.Advertencias.AddRange(p.Advertencias);

        // Duplicados dentro del propio paquete
        foreach (var g in p.Actas.GroupBy(a => a.Codigo).Where(g => g.Count() > 1))
            r.Conflictos.Add($"El paquete trae {g.Count()} carpetas con el mismo código {g.Key}: {string.Join(", ", g.Select(x => x.Carpeta))}. Se tomará la primera.");

        var actasDb = await db.ActasReunion.AsNoTracking().IgnoreQueryFilters()
            .Select(a => new { a.Codigo, a.DatosOrigenJson, a.Estado, a.Origen }).ToListAsync();
        var actasPorCodigo = actasDb.GroupBy(a => a.Codigo).ToDictionary(g => g.Key, g => g.First());
        var compsDb = await db.CompromisosReunion.AsNoTracking().IgnoreQueryFilters()
            .Select(c => new { c.IdOrigen, c.EditadoPor, Gestiones = c.Actualizaciones.Count(x => x.Origen != "Migracion") })
            .ToListAsync();
        var compsPorId = compsDb.GroupBy(c => c.IdOrigen).ToDictionary(g => g.Key, g => g.First());
        var archivosDb = await db.ArchivosActaReunion.AsNoTracking()
            .Select(x => new { x.ActaReunion.Codigo, x.Tipo, x.Tamano, x.LoteImportacionId }).ToListAsync();
        var proyectosDb = await db.ProyectosActa.AsNoTracking().ToListAsync();
        var proyectosRen = await db.Proyectos.AsNoTracking().Select(x => new { x.Id, x.Codigo, x.Nombre }).ToListAsync();

        // Word sueltos: el proyecto con código reconocible que aún no está en el catálogo se agrega
        // (solo si no existe: un ProyectoLeido sobre uno existente le borraría alias y áreas)
        if (p.DesdeWord)
            foreach (var a in p.Actas)
            {
                var cod = CodigoProyectoDe(a.ProyectoTexto);
                if (cod is null || proyectosDb.Any(x => Igual(x.Codigo, cod)) || p.Proyectos.Any(x => Igual(x.Codigo, cod))) continue;
                var txt = a.ProyectoTexto!.Trim();
                var guion = txt.IndexOf(" - ", StringComparison.Ordinal);
                var nombre = guion > 0 ? txt[(guion + 3)..].Trim() : txt;
                if (proyectosDb.Any(x => Igual(x.Nombre, nombre))) continue;
                p.Proyectos.Add(new ProyectoLeido(nombre.Length > 0 ? nombre : cod, cod, null, null, null, null));
            }

        var seguimientosDb = p.DesdeWord
            ? await db.ActualizacionesCompromisoReunion.AsNoTracking()
                .Where(x => x.TipoActualizacion == "seguimiento" && x.ActaReferencia != null)
                .Select(x => x.CompromisoReunion.IdOrigen + "|" + x.ActaReferencia).ToListAsync()
            : [];

        foreach (var pr in p.Proyectos)
            if (!proyectosDb.Any(x => Igual(x.Codigo, pr.Codigo) || Igual(x.Nombre, pr.Nombre)))
            {
                r.ProyectosNuevos.Add($"{pr.Nombre}{(pr.Codigo is null ? "" : $" ({pr.Codigo})")}");
                if (pr.Codigo is null || !proyectosRen.Any(x => Igual(x.Codigo, pr.Codigo)))
                    r.ProyectosSinMapear.Add($"{pr.Nombre}{(pr.Codigo is null ? "" : $" ({pr.Codigo})")} — sin proyecto equivalente en RenergeIA (queda sin vincular, se asigna manualmente)");
            }

        var vistos = new HashSet<string>();
        foreach (var a in p.Actas)
        {
            if (!vistos.Add(a.Codigo)) continue;

            var codProy = CodigoProyectoDe(a.ProyectoTexto);
            if (a.ProyectoTexto is not null && codProy is null && !p.Proyectos.Any(x => Igual(x.Nombre, a.ProyectoTexto))
                && !proyectosDb.Any(x => Igual(x.Nombre, a.ProyectoTexto)))
                r.ProyectosSinMapear.Add($"Acta {a.Codigo}: proyecto \"{a.ProyectoTexto}\" sin código reconocible ni entrada en el catálogo.");

            if (!actasPorCodigo.TryGetValue(a.Codigo, out var ex))
                r.ActasNuevas.Add(a.Codigo);
            else if (p.DesdeWord)
                r.ActasComplementadas.Add(a.Codigo);
            else if (JsonIgual(ex.DatosOrigenJson, a.Json))
                r.ActasSinCambio.Add(a.Codigo);
            else
                r.ActasConDiferencias.Add(new Diferencia(a.Codigo, "acta.json", "versión migrada", "el paquete trae otra versión — se actualizará el contenido de origen (el estado en la app se conserva)"));

            foreach (var c in a.Compromisos)
            {
                if (c.ContinuidadDe is not null) r.ContinuidadesResueltas.Add($"{c.IdOrigen} → continúa {c.ContinuidadDe}");
                if (!string.IsNullOrWhiteSpace(c.FechaTexto) && FechaDesdeTexto(c.FechaTexto) is null)
                    r.FechasDescriptivas.Add($"{c.IdOrigen}: \"{c.FechaTexto}\"");

                if (!compsPorId.TryGetValue(c.IdOrigen, out var cex))
                {
                    if (c.ContinuidadDe is null) r.CompromisosNuevos.Add(c.IdOrigen);
                }
                else if (p.DesdeWord)
                    r.CompromisosExistentes.Add(c.IdOrigen);
                else if (cex.Gestiones > 0 || cex.EditadoPor is not null)
                    r.CompromisosProtegidos.Add(c.IdOrigen);
                else if (p.Tablero.ContainsKey(c.IdOrigen))
                    r.CompromisosActualizarTablero.Add(c.IdOrigen);
            }

            foreach (var f in a.Archivos)
            {
                var exF = archivosDb.FirstOrDefault(x => x.Codigo == a.Codigo && x.Tipo == f.Tipo);
                if (exF is null) r.ArchivosNuevos.Add($"{a.Codigo} · {f.Filename}");
                else if (exF.Tamano != f.Contenido.Length)
                    r.ArchivosDistintos.Add($"{a.Codigo} · {f.Filename}: el ya migrado ({exF.Tamano:N0} B) difiere del paquete ({f.Contenido.Length:N0} B) — NO se reemplaza (documento histórico); revisar manualmente");
            }
            if (a.ArchivosFaltantes.Count > 0 && a.ArchivosFaltantes.Count < 4)
                r.Advertencias.Add($"{a.Codigo}: sin {string.Join(", ", a.ArchivosFaltantes)} en el paquete.");

            // Seguimiento de compromisos anteriores: se registra como actualización del compromiso
            // de origen (solo agrega historia; el estado del compromiso NO cambia)
            foreach (var s in a.SeguimientoAnterior)
            {
                if (s.Candidatos.Count == 0)
                {
                    r.SeguimientosSinOrigen.Add($"{a.Codigo}: el Word no indica el número del compromiso (origen \"{s.Origen}\") — queda visible en el acta, sin pasar al historial");
                    continue;
                }
                var id = s.Candidatos.FirstOrDefault(c => compsPorId.ContainsKey(c) || p.Actas.Any(x => x.Compromisos.Any(k => k.IdOrigen == c)));
                if (id is null)
                    r.SeguimientosSinOrigen.Add($"{a.Codigo} → {string.Join(" o ", s.Candidatos)}: el compromiso de origen aún no está cargado (queda visible en el acta; sube primero el acta de origen si quieres verlo en su historial)");
                else if (!seguimientosDb.Contains(id + "|" + a.Codigo))
                    r.SeguimientosNuevos.Add($"{id}: {s.Estado ?? "sin estado"} — {s.Observacion}");
            }
        }

        // Compromisos del export del tablero sin acta en el paquete
        foreach (var id in p.Tablero.Keys)
        {
            var sep = id.LastIndexOf('-');
            var codigoActa = sep > 0 ? id[..sep] : id;
            if (!p.Actas.Any(a => a.Codigo == codigoActa) && !compsPorId.ContainsKey(id))
                r.Conflictos.Add($"El export del tablero trae el compromiso {id} pero el paquete no incluye su acta {codigoActa}: se omitirá.");
        }
        return r;
    }

    // ── APLICAR (idempotente, con respaldo y reversión) ──────────────────────
    public async Task<(LoteImportacionActas Lote, Reporte Reporte)> AplicarAsync(PaqueteActas p, string archivo, string usuario)
    {
        var reporte = await SimularAsync(p);
        var respaldo = new RespaldoLote();

        await using var tx = await db.Database.BeginTransactionAsync();

        var lote = new LoteImportacionActas { Archivo = Corta(archivo, 300), Usuario = Corta(usuario, 200) };
        db.LotesImportacionActas.Add(lote);
        await db.SaveChangesAsync();

        // 1. Catálogo de proyectos (+ vínculo automático SOLO por código exacto con RenergeIA)
        var proyectosRen = await db.Proyectos.AsNoTracking().Select(x => new { x.Id, x.Codigo }).ToListAsync();
        var proyectosDb = await db.ProyectosActa.ToListAsync();
        foreach (var pr in p.Proyectos)
        {
            var ex = proyectosDb.FirstOrDefault(x => Igual(x.Codigo, pr.Codigo)) ?? proyectosDb.FirstOrDefault(x => Igual(x.Nombre, pr.Nombre));
            if (ex is null)
            {
                ex = new ProyectoActa { LoteImportacionId = lote.Id };
                db.ProyectosActa.Add(ex);
                proyectosDb.Add(ex);
            }
            else respaldo.ProyectosModificados.Add(Snapshot(ex));
            ex.Nombre = pr.Nombre; ex.Codigo = pr.Codigo; ex.AreasJson = pr.AreasJson;
            ex.AliasJson = pr.AliasJson; ex.CodigosAnterioresJson = pr.CodigosAnterioresJson; ex.IdOrigen = pr.IdOrigen;
            ex.ProyectoId ??= proyectosRen.FirstOrDefault(x => pr.Codigo is not null && Igual(x.Codigo, pr.Codigo))?.Id;
        }
        await db.SaveChangesAsync();
        respaldo.ProyectosCreados = proyectosDb.Where(x => x.LoteImportacionId == lote.Id).Select(x => x.Id).ToList();

        // 2. Actas
        var vistos = new HashSet<string>();
        var actasMap = new Dictionary<string, ActaReunion>();
        foreach (var a in p.Actas)
        {
            if (!vistos.Add(a.Codigo)) continue;
            var ex = await db.ActasReunion.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Codigo == a.Codigo);
            if (ex is null)
            {
                ex = new ActaReunion { Codigo = a.Codigo, LoteImportacionId = lote.Id };
                db.ActasReunion.Add(ex);
                // Estado inicial: el del export del tablero si existe; si no, el del acta.json; si no, borrador
                ex.Estado = MapEstadoActa(p.EstadosActaTablero.GetValueOrDefault(a.Codigo) ?? a.EstadoTexto);
            }
            else if (p.DesdeWord)
            {
                // Word suelto sobre un acta ya cargada: el acta no se reescribe; solo recibe
                // los documentos y compromisos que le falten (pasos siguientes)
                actasMap[a.Codigo] = ex;
                continue;
            }
            else
            {
                respaldo.ActasModificadas.Add(Snapshot(ex));
                // El estado existente en la app SE CONSERVA (regla de subir_acta.js)
            }
            ex.Fecha = a.Fecha; ex.FechaTexto = Corta(a.FechaTexto, 200);
            ex.ProyectoTexto = Corta(a.ProyectoTexto, 300); ex.Area = Corta(a.Area, 50);
            ex.Asunto = Corta(a.Asunto, 500); ex.Modalidad = Corta(a.Modalidad, 150); ex.Hora = Corta(a.Hora, 150);
            ex.Resumen = a.Resumen; ex.AsistentesJson = a.AsistentesJson; ex.PuntosJson = a.PuntosJson;
            ex.ActaAnteriorCodigo = Corta(a.ActaAnterior, 150); ex.TrazabilidadJson = a.TrazabilidadJson;
            ex.SeguimientoPendienteConfirmacion = a.SeguimientoPendienteConfirmacion;
            ex.FuenteUrl = Corta(a.FuenteUrl, 600); ex.DatosOrigenJson = a.Json; ex.Origen = p.DesdeWord ? "Word" : "ActasIA";
            var codProy = CodigoProyectoDe(a.ProyectoTexto);
            ex.ProyectoActaId = (proyectosDb.FirstOrDefault(x => codProy is not null && Igual(x.Codigo, codProy))
                ?? proyectosDb.FirstOrDefault(x => Igual(x.Nombre, a.ProyectoTexto)))?.Id;
            actasMap[a.Codigo] = ex;
        }
        await db.SaveChangesAsync();
        respaldo.ActasCreadas = actasMap.Values.Where(x => x.LoteImportacionId == lote.Id).Select(x => x.Id).ToList();

        // 3. Compromisos (dos pasadas: crear/actualizar y luego resolver continuidades)
        var compsDb = await db.CompromisosReunion.IgnoreQueryFilters().Include(c => c.Actualizaciones).ToListAsync();
        var porIdOrigen = compsDb.GroupBy(c => c.IdOrigen).ToDictionary(g => g.Key, g => g.First());
        vistos.Clear();

        // Mapa del paquete y resolución TRANSITIVA de continuidades (A→B→C llega hasta C):
        // un compromiso solo "materializa" tarjeta si su raíz no existe ni en BD ni en el paquete.
        var paqueteComp = new Dictionary<string, CompromisoLeido>();
        foreach (var a in p.Actas)
        {
            if (!vistos.Add(a.Codigo)) continue;
            foreach (var c in a.Compromisos) paqueteComp.TryAdd(c.IdOrigen, c);
        }
        vistos.Clear();
        string RaizDe(string id)
        {
            var recorridos = new HashSet<string>();
            while (recorridos.Add(id) && paqueteComp.TryGetValue(id, out var k) && k.ContinuidadDe is not null)
                id = k.ContinuidadDe;
            return id;
        }
        foreach (var a in p.Actas)
        {
            if (!vistos.Add(a.Codigo) || !actasMap.TryGetValue(a.Codigo, out var acta)) continue;
            foreach (var c in a.Compromisos)
            {
                // Continuidad hacia un compromiso existente: NO se crea tarjeta nueva;
                // queda como actualización del compromiso RAÍZ (segunda pasada). La raíz se
                // resuelve transitivamente y debe materializarse (estar en BD, o ser un
                // compromiso del paquete sin continuidadDe, que sí se crea).
                if (c.ContinuidadDe is not null)
                {
                    var raiz = RaizDe(c.ContinuidadDe);
                    var materializa = porIdOrigen.ContainsKey(raiz) ||
                        (paqueteComp.TryGetValue(raiz, out var k) && k.ContinuidadDe is null);
                    if (materializa) continue;
                }

                porIdOrigen.TryGetValue(c.IdOrigen, out var ex);
                if (p.DesdeWord && ex is not null) continue; // Word suelto: lo ya cargado no se toca
                var protegido = ex is not null && (ex.EditadoPor is not null || ex.Actualizaciones.Any(x => x.Origen != "Migracion"));
                if (ex is null)
                {
                    ex = new CompromisoReunion { IdOrigen = c.IdOrigen, LoteImportacionId = lote.Id };
                    db.CompromisosReunion.Add(ex);
                    porIdOrigen[c.IdOrigen] = ex;
                    compsDb.Add(ex);
                }
                else if (!protegido) respaldo.CompromisosModificados.Add(Snapshot(ex));

                if (protegido) continue; // gestionado en la app: el paquete no lo pisa

                ex.ActaReunionId = acta.Id;
                ex.Orden = c.Orden;
                ex.Texto = Corta(c.Texto, 3000) ?? "";
                ex.Responsable = Corta(c.Responsable, 400) ?? "Por confirmar";
                ex.Empresa = Corta(c.Empresa, 200);
                ex.FechaTexto = Corta(c.FechaTexto, 400);
                ex.FechaLimite = FechaDesdeTexto(c.FechaTexto);
                ex.Categoria = Corta(c.Categoria, 100) ?? Corta(a.Area, 100) ?? "Por clasificar";
                ex.Tipo = c.TipoTexto == "recordatorio" || (c.TipoTexto is null && EsEmpresaCliente(c.Empresa))
                    ? TipoCompromisoReunion.Recordatorio : TipoCompromisoReunion.Propio;
                ex.ContinuidadDeOrigen = c.ContinuidadDe;
                ex.ProyectoTexto = Corta(a.ProyectoTexto, 300);
                ex.Area = Corta(a.Area, 50);
                ex.FechaActa = a.Fecha;

                // Estado y gestión desde el export del tablero (la fuente operativa)
                if (p.Tablero.TryGetValue(c.IdOrigen, out var t))
                {
                    ex.Estado = MapEstadoCompromiso(t.Estado);
                    ex.NotaCierre = Corta(t.NotaCierre, 2000);
                    ex.CerradoPor = Corta(t.CerradoPor, 200);
                    ex.CerradoEl = t.CerradoEl;
                    ex.FechaLimite = t.FechaLimite ?? ex.FechaLimite;
                    ex.FechaTexto = Corta(t.FechaTexto, 400) ?? ex.FechaTexto;
                    ex.Categoria = Corta(t.Categoria, 100) ?? ex.Categoria;
                    if (t.TipoTexto == "recordatorio") ex.Tipo = TipoCompromisoReunion.Recordatorio;
                    else if (t.TipoTexto == "propio") ex.Tipo = TipoCompromisoReunion.Propio;
                    ex.Responsable = Corta(t.Responsable, 400) ?? ex.Responsable;
                    ex.Empresa = Corta(t.Empresa, 200) ?? ex.Empresa;
                    ex.RequiereConfirmacion = t.RequiereConfirmacion;
                    ex.ObservacionSeguimiento = Corta(t.ObservacionSeguimiento, 2000);
                    ex.HistorialJson = t.HistorialJson;
                    ex.OrigenOriginalJson = t.OrigenOriginalJson ?? t.Json;
                    ex.CreadoOrigen = t.Creado;
                    ex.EditadoPor = null; // la autoría histórica queda en HistorialJson; EditadoPor es solo para gestión en la app
                }
                else
                {
                    ex.OrigenOriginalJson ??= JsonSerializer.Serialize(c, _json);
                }
            }
        }
        await db.SaveChangesAsync();
        respaldo.CompromisosCreados = compsDb.Where(x => x.LoteImportacionId == lote.Id).Select(x => x.Id).ToList();

        // 3b. Continuidades: actualización sobre el compromiso ORIGINAL (misma identidad)
        vistos.Clear();
        foreach (var a in p.Actas)
        {
            if (!vistos.Add(a.Codigo)) continue;
            foreach (var c in a.Compromisos.Where(x => x.ContinuidadDe is not null))
            {
                var raiz = RaizDe(c.ContinuidadDe!);
                if (!porIdOrigen.TryGetValue(raiz, out var original))
                {
                    reporte.Conflictos.Add($"{c.IdOrigen}: continuidadDe={c.ContinuidadDe} (raíz {raiz}) no se encontró — se creó como compromiso independiente para no perderlo.");
                    continue;
                }
                var yaRegistrada = original.Actualizaciones.Any(x => x.ActaReferencia == a.Codigo && x.Origen == "Migracion")
                    || await db.ActualizacionesCompromisoReunion.AnyAsync(x => x.CompromisoReunionId == original.Id && x.ActaReferencia == a.Codigo && x.Origen == "Migracion");
                if (yaRegistrada) continue;
                var act = new ActualizacionCompromisoReunion
                {
                    CompromisoReunionId = original.Id,
                    Fecha = a.Fecha ?? DateTime.UtcNow,
                    Autor = "Migración Actas IA",
                    TipoActualizacion = "continuidad",
                    Observacion = Corta($"El compromiso continúa en el acta {a.Codigo} (ítem {c.Orden}). {c.Texto}", 3000),
                    ActaReferencia = a.Codigo,
                    Origen = "Migracion"
                };
                db.ActualizacionesCompromisoReunion.Add(act);
                respaldo.ActualizacionesCreadas.Add(act);
            }
        }
        await db.SaveChangesAsync();

        // 3c. Word suelto: cada fila de "Seguimiento de compromisos anteriores" queda como
        // actualización del compromiso de origen. Solo agrega historia: el estado NO cambia
        // (un cierre propuesto en una reunión nunca retira un pendiente).
        if (p.DesdeWord)
        {
            vistos.Clear();
            foreach (var a in p.Actas)
            {
                if (!vistos.Add(a.Codigo)) continue;
                foreach (var s in a.SeguimientoAnterior)
                {
                    var original = s.Candidatos.Select(c => porIdOrigen.GetValueOrDefault(c)).FirstOrDefault(c => c is not null);
                    if (original is null || original.Id == 0) continue;
                    var ya = await db.ActualizacionesCompromisoReunion.AnyAsync(x => x.CompromisoReunionId == original.Id
                        && x.ActaReferencia == a.Codigo && x.TipoActualizacion == "seguimiento");
                    if (ya) continue;
                    var obs = string.Join(" ", new[] { s.Estado is null ? null : $"[{s.Estado}]", s.Observacion }.Where(x => x is not null));
                    var act = new ActualizacionCompromisoReunion
                    {
                        CompromisoReunionId = original.Id,
                        Fecha = a.Fecha ?? DateTime.UtcNow,
                        Autor = "Carga de acta Word",
                        TipoActualizacion = "seguimiento",
                        Observacion = Corta($"Seguimiento en el acta {a.Codigo}: {obs}", 3000),
                        ActaReferencia = a.Codigo,
                        Origen = "Migracion"
                    };
                    db.ActualizacionesCompromisoReunion.Add(act);
                    respaldo.ActualizacionesCreadas.Add(act);
                }
            }
            await db.SaveChangesAsync();
        }

        // 4. Archivos (los históricos NO se reemplazan si difieren)
        vistos.Clear();
        foreach (var a in p.Actas)
        {
            if (!vistos.Add(a.Codigo) || !actasMap.TryGetValue(a.Codigo, out var acta)) continue;
            var existentes = await db.ArchivosActaReunion.Where(x => x.ActaReunionId == acta.Id).ToListAsync();
            foreach (var f in a.Archivos)
            {
                var ex = existentes.FirstOrDefault(x => x.Tipo == f.Tipo);
                if (ex is null)
                {
                    var nuevo = new ArchivoActaReunion
                    {
                        ActaReunionId = acta.Id, Tipo = f.Tipo, Filename = Corta(f.Filename, 300)!,
                        Tamano = f.Contenido.Length, Contenido = f.Contenido, LoteImportacionId = lote.Id
                    };
                    db.ArchivosActaReunion.Add(nuevo);
                    respaldo.ArchivosCreadosTmp.Add(nuevo);
                }
                // si existe y difiere: se conserva el histórico; el reporte de simulación ya lo listó
            }
        }
        await db.SaveChangesAsync();
        respaldo.ArchivosCreados = respaldo.ArchivosCreadosTmp.Select(x => x.Id).ToList();

        // 5. Seguimientos (si el paquete los trae); dedupe por id dentro del propio paquete
        var segVistos = new HashSet<string>();
        foreach (var (id, json) in p.Seguimientos)
        {
            if (!segVistos.Add(id)) continue;
            var ex = await db.RegistrosSeguimientoActa.FirstOrDefaultAsync(x => x.IdOrigen == id);
            if (ex is null)
            {
                ex = new RegistroSeguimientoActa { IdOrigen = id, LoteImportacionId = lote.Id };
                db.RegistrosSeguimientoActa.Add(ex);
                respaldo.SeguimientosCreadosTmp.Add(ex);
            }
            else respaldo.SeguimientosModificados.Add(Snapshot(ex));
            using var d = JsonDocument.Parse(json);
            var e = d.RootElement;
            ex.ProyectoTexto = Corta(Prop(e, "proyecto"), 300);
            ex.Tema = Corta(Prop(e, "tema"), 600);
            ex.Accion = Corta(Prop(e, "accion"), 2000);
            ex.EstadoRegistro = Corta(Prop(e, "estadoRegistro"), 100);
            ex.Evidencia = Corta(Prop(e, "evidencia"), 2000);
            ex.ReferenciasJson = PropCrudo(e, "referencias");
            ex.HistorialJson = PropCrudo(e, "historial");
            ex.Confirmada = e.TryGetProperty("confirmada", out var cf) && cf.ValueKind == JsonValueKind.True;
            ex.ConfirmadoPor = Corta(Prop(e, "confirmadoPor"), 200);
            ex.DatosOrigenJson = json;
        }
        await db.SaveChangesAsync();
        respaldo.SeguimientosCreados = respaldo.SeguimientosCreadosTmp.Select(x => x.Id).ToList();

        lote.ResumenJson = JsonSerializer.Serialize(reporte, _json);
        lote.RespaldoJson = JsonSerializer.Serialize(respaldo.ParaGuardar(), _json);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return (lote, reporte);
    }

    // ── REVERTIR un lote: borra lo creado y restaura lo modificado ───────────
    public async Task<string> RevertirAsync(int loteId)
    {
        var lote = await db.LotesImportacionActas.FindAsync(loteId)
            ?? throw new InvalidOperationException("Lote no encontrado.");
        if (lote.Revertido) return "Este lote ya fue revertido.";
        if (string.IsNullOrEmpty(lote.RespaldoJson)) throw new InvalidOperationException("El lote no tiene respaldo.");

        var r = JsonSerializer.Deserialize<RespaldoGuardado>(lote.RespaldoJson!) ?? new();

        // Protección: si hubo gestión EN LA APP sobre registros creados por este lote,
        // la reversión se bloquea (no se decide en silencio perder trabajo del usuario).
        var gestiones = await db.ActualizacionesCompromisoReunion
            .CountAsync(x => r.CompromisosCreados.Contains(x.CompromisoReunionId) && x.Origen != "Migracion");
        var comentarios = await db.ComentariosActaReunion
            .CountAsync(x => r.ActasCreadas.Contains(x.ActaReunionId) && x.IdOrigen == null);
        if (gestiones > 0 || comentarios > 0)
            throw new InvalidOperationException(
                $"No se revirtió: sobre los registros de este lote ya hay {gestiones} gestión(es) y {comentarios} comentario(s) hechos en la app, que se perderían. Revisa antes de revertir.");

        await using var tx = await db.Database.BeginTransactionAsync();

        await db.ActualizacionesCompromisoReunion.Where(x => r.ActualizacionesCreadas.Contains(x.Id)).ExecuteDeleteAsync();
        await db.ArchivosActaReunion.Where(x => r.ArchivosCreados.Contains(x.Id)).ExecuteDeleteAsync();
        await db.CompromisosReunion.IgnoreQueryFilters().Where(x => r.CompromisosCreados.Contains(x.Id)).ExecuteDeleteAsync();
        await db.ActasReunion.IgnoreQueryFilters().Where(x => r.ActasCreadas.Contains(x.Id)).ExecuteDeleteAsync();
        await db.RegistrosSeguimientoActa.Where(x => r.SeguimientosCreados.Contains(x.Id)).ExecuteDeleteAsync();
        await db.ProyectosActa.Where(x => r.ProyectosCreados.Contains(x.Id)).ExecuteDeleteAsync();

        int restaurados = 0;
        restaurados += await RestaurarAsync<ActaReunion>(r.ActasModificadas);
        restaurados += await RestaurarAsync<CompromisoReunion>(r.CompromisosModificados);
        restaurados += await RestaurarAsync<RegistroSeguimientoActa>(r.SeguimientosModificados);
        restaurados += await RestaurarAsync<ProyectoActa>(r.ProyectosModificados);

        lote.Revertido = true;
        lote.RevertidoEl = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return $"Lote revertido: {r.ActasCreadas.Count} actas, {r.CompromisosCreados.Count} compromisos, {r.ArchivosCreados.Count} archivos y {r.ActualizacionesCreadas.Count} actualizaciones eliminados; {restaurados} registros restaurados.";
    }

    private async Task<int> RestaurarAsync<T>(List<JsonElement> snapshots) where T : EntidadBase
    {
        int n = 0;
        foreach (var s in snapshots)
        {
            var entidad = s.Deserialize<T>();
            if (entidad is null) continue;
            var actual = await db.Set<T>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == entidad.Id);
            if (actual is null) continue;
            db.Entry(actual).CurrentValues.SetValues(entidad);
            n++;
        }
        await db.SaveChangesAsync();
        return n;
    }

    // ── helpers ──────────────────────────────────────────────────────────────
    private static EstadoActaReunion MapEstadoActa(string? estado) => estado?.Trim().ToLowerInvariant() switch
    {
        "aprobada" => EstadoActaReunion.Aprobada,
        "comentarios" => EstadoActaReunion.ConComentarios,
        _ => EstadoActaReunion.Borrador
    };

    private static EstadoCompromisoReunion MapEstadoCompromiso(string? estado) => estado?.Trim().ToLowerInvariant() switch
    {
        "cerrado" => EstadoCompromisoReunion.Cerrado,
        "en_seguimiento" => EstadoCompromisoReunion.EnSeguimiento,
        _ => EstadoCompromisoReunion.Pendiente
    };

    private static bool Igual(string? a, string? b) =>
        !string.IsNullOrWhiteSpace(a) && !string.IsNullOrWhiteSpace(b) &&
        string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool JsonIgual(string? a, string? b)
    {
        if (a is null || b is null) return a == b;
        try
        {
            using var da = JsonDocument.Parse(a);
            using var dbj = JsonDocument.Parse(b);
            return da.RootElement.GetRawText() == dbj.RootElement.GetRawText();
        }
        catch { return a == b; }
    }

    private static string? Prop(JsonElement e, string nombre) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(nombre, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static string? PropCrudo(JsonElement e, string nombre) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(nombre, out var v) && v.ValueKind is JsonValueKind.Array or JsonValueKind.Object ? v.GetRawText() : null;

    private static string? Corta(string? s, int max) => s is null ? null : s.Length > max ? s[..max] : s;

    // Snapshot SOLO de columnas escalares (CurrentValues): jamás serializa navegaciones
    // ni binarios de archivos — un respaldo con el grafo completo crecería a cientos de MB.
    private JsonElement Snapshot<T>(T entidad) where T : EntidadBase
    {
        var clon = (T)db.Entry(entidad).CurrentValues.ToObject();
        return JsonSerializer.SerializeToElement(clon, _json);
    }

    private sealed class RespaldoLote
    {
        public List<int> ActasCreadas = [], CompromisosCreados = [], ArchivosCreados = [], SeguimientosCreados = [], ProyectosCreados = [];
        public List<ActualizacionCompromisoReunion> ActualizacionesCreadas = [];
        public List<ArchivoActaReunion> ArchivosCreadosTmp = [];
        public List<RegistroSeguimientoActa> SeguimientosCreadosTmp = [];
        public List<JsonElement> ActasModificadas = [], CompromisosModificados = [], SeguimientosModificados = [], ProyectosModificados = [];

        public RespaldoGuardado ParaGuardar() => new()
        {
            ActasCreadas = ActasCreadas, CompromisosCreados = CompromisosCreados, ArchivosCreados = ArchivosCreados,
            SeguimientosCreados = SeguimientosCreados, ProyectosCreados = ProyectosCreados,
            ActualizacionesCreadas = ActualizacionesCreadas.Select(x => x.Id).ToList(),
            ActasModificadas = ActasModificadas, CompromisosModificados = CompromisosModificados,
            SeguimientosModificados = SeguimientosModificados, ProyectosModificados = ProyectosModificados
        };
    }

    public sealed class RespaldoGuardado
    {
        public List<int> ActasCreadas { get; set; } = [];
        public List<int> CompromisosCreados { get; set; } = [];
        public List<int> ArchivosCreados { get; set; } = [];
        public List<int> SeguimientosCreados { get; set; } = [];
        public List<int> ProyectosCreados { get; set; } = [];
        public List<int> ActualizacionesCreadas { get; set; } = [];
        public List<JsonElement> ActasModificadas { get; set; } = [];
        public List<JsonElement> CompromisosModificados { get; set; } = [];
        public List<JsonElement> SeguimientosModificados { get; set; } = [];
        public List<JsonElement> ProyectosModificados { get; set; } = [];
    }
}
