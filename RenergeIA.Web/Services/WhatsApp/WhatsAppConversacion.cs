using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Entities.Helvis;
using RenergeIA.Core.Helpers;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Lógica de Helvis: es deliberadamente determinista —el código decide, no un modelo—
/// para que las respuestas sean predecibles y auditables.
/// Flujo (pedido de la usuaria 2026-10-23): primero se identifica a la persona por su
/// celular, luego se pregunta/recuerda SOBRE QUÉ PROYECTO se trabaja, y después el menú
/// del proyecto: Ver avance (Curva S), Alertas (planificación documental) e Informe
/// diario (actividades asignadas; el registro conversacional llega en la Etapa 2).
/// Todo mensaje entrante y saliente queda guardado en MensajesHelvis.
/// Las decisiones A–I del panel de revisión (docs/helvis/panel-conversacion.html) están
/// aplicadas aquí: si cambia este flujo, el panel se actualiza en el mismo cambio.
/// </summary>
public partial class WhatsAppConversacion
{
    /// <summary>Cuánto tiempo recuerda Helvis el proyecto elegido antes de volver a preguntar.</summary>
    private static readonly TimeSpan VigenciaProyectoActivo = TimeSpan.FromHours(12);

    private readonly WhatsAppClient _wa;
    private readonly RenergeIADbContext _db;
    private readonly InformeDiarioService _informes;
    private readonly ILogger<WhatsAppConversacion> _log;
    private readonly WhatsAppOptions _opciones;

    public WhatsAppConversacion(WhatsAppClient wa, RenergeIADbContext db,
        InformeDiarioService informes, ILogger<WhatsAppConversacion> log,
        Microsoft.Extensions.Options.IOptions<WhatsAppOptions> opciones)
    {
        _opciones = opciones.Value;
        _wa = wa;
        _db = db;
        _informes = informes;
        _log = log;
    }

    public async Task AtenderAsync(MensajeWhatsApp mensaje, CancellationToken ct = default)
    {
        _log.LogInformation("WhatsApp: mensaje de {Numero} ({Nombre}), tipo {Tipo}, opción {Opcion}: {Texto}",
            mensaje.De, mensaje.Nombre, mensaje.Tipo, mensaje.OpcionId ?? "-", mensaje.Texto);

        var contacto = await _db.ContactosHelvis
            .Include(c => c.Proyectos).ThenInclude(p => p.Proyecto)
            .FirstOrDefaultAsync(c => c.Telefono == mensaje.De, ct);

        // Deduplicación persistente: el índice único de WaMessageId rechaza el duplicado
        // aunque el proceso se haya reiniciado entre el envío original y el reintento de Meta.
        if (!await GuardarEntranteAsync(mensaje, contacto, ct)) return;

        await _wa.MarcarLeidoAsync(mensaje.Id, ct);

        if (contacto is null || !contacto.Activo)
        {
            await AtenderDesconocidoAsync(mensaje, esDesconocido: contacto is null, ct);
            return;
        }

        // Decisión A: saludar por el nombre la primera vez del día; después, directo al grano.
        var saludo = await PrimeraVezHoyAsync(mensaje.De, ct) ? Saludo(contacto, mensaje) + "\n\n" : "";

        // Decisión E: sin el permiso Consultar no se entrega información.
        if (!contacto.PuedeConsultar)
        {
            await EnviarTextoAsync(contacto, mensaje.De,
                saludo + "Este número está registrado, pero aún no tiene habilitada la consulta de información. " +
                "El administrador puede activarla en el panel de Helvis. 🙌", ct);
            return;
        }

        // Fotos: quien puede reportar las deja guardadas (se ligan a su reporte de avance).
        // Videos, documentos y stickers aún no se guardan; quien no reporta recibe el aviso.
        if (mensaje.Tipo is "image" && contacto.PuedeReportar)
        {
            await RecibirFotoAsync(contacto, mensaje, saludo, ct);
            return;
        }
        if (mensaje.Tipo is "image" or "video" or "document" or "sticker")
        {
            var motivo = mensaje.Tipo == "image"
                ? "este número no tiene habilitado el permiso para reportar"
                : "por ahora solo se guardan fotos";
            await EnviarTextoAsync(contacto, mensaje.De,
                saludo + $"📷 Recibí el archivo, pero *no queda guardado*: {motivo}. " +
                "Mientras tanto, va en el Informe diario de la app.\n\n" +
                "Escribe *menú* para ver las opciones.", ct);
            return;
        }
        if (mensaje.Tipo is "audio" or "voice")
        {
            await EnviarTextoAsync(contacto, mensaje.De,
                saludo + "🎙️ Recibí la nota de voz, pero aún no puedo escucharlas. " +
                "¿Me lo escribes en un mensaje de texto?", ct);
            return;
        }

        // El filtro de soft-delete deja el Proyecto en null si está en la papelera.
        var proyectos = contacto.Proyectos
            .Where(p => p.Proyecto is not null)
            .Select(p => p.Proyecto)
            .OrderBy(p => p.Nombre)
            .ToList();

        // Botones que llegan desde un aviso (aprobar/devolver un informe) valen en
        // cualquier momento, sin importar el proyecto activo: se validan permisos adentro.
        if (mensaje.OpcionId is { } idAviso && (idAviso.StartsWith("apr_") || idAviso.StartsWith("dev_")))
        {
            await ResolverRevisionAsync(contacto, proyectos, idAviso, mensaje.De, ct);
            return;
        }

        // "menú" / "cancelar" interrumpen cualquier conversación de varios pasos.
        var palabra = ReporteTexto.Normalizar(mensaje.Texto);
        var esEscape = mensaje.OpcionId is "menu" ||
                       palabra is "menu" or "cancelar" or "salir" or "inicio" or "volver";
        if (esEscape && await CancelarConversacionesAsync(contacto, ct))
            await EnviarTextoAsync(contacto, mensaje.De, "Listo, cancelé lo que estaba en curso; no se guardó nada.", ct);

        // Conversaciones de varios pasos en curso: reporte de avance, bloqueo, devolución.
        if (!esEscape)
        {
            if (await ContinuarFlujoAsync(contacto, proyectos, mensaje, ct)) return;
            if (await ContinuarReporteAsync(contacto, mensaje, ct)) return;
        }

        var opcion = esEscape ? null : mensaje.OpcionId ?? Interpretar(mensaje.Texto);

        // Decisión I: "costos" ya no abre el avance físico; se avisa que llega después.
        if (opcion == "costos")
        {
            await EnviarTextoAsync(contacto, mensaje.De,
                "💰 La consulta de costos por WhatsApp aún no está disponible; llega en una etapa " +
                "posterior. Por ahora puedo mostrar el avance físico, las alertas documentales y el informe diario.", ct);
            opcion = null; // y a continuación va el menú
        }

        // Cambio explícito de proyecto
        if (opcion == "cambiar")
        {
            await EnviarSelectorProyectoAsync(contacto, proyectos, mensaje, saludo, ct);
            return;
        }

        // Botón de proyecto pulsado. Decisión H: si había una opción pedida antes de
        // elegir el proyecto, se responde de una vez en lugar de perderla.
        if (opcion is not null && opcion.StartsWith("proy_") &&
            int.TryParse(opcion[5..], out var elegidoId) &&
            proyectos.Any(p => p.Id == elegidoId))
        {
            var pendiente = contacto.OpcionPendiente;
            await ActivarProyectoAsync(contacto, elegidoId, ct);
            await EjecutarOpcionAsync(contacto, proyectos.First(p => p.Id == elegidoId),
                pendiente, proyectos.Count > 1, mensaje.De, saludo: "", ct);
            return;
        }

        var activo = ProyectoActivo(contacto, proyectos);

        // Respuesta numérica mientras se espera elegir proyecto (lista numerada con 4+)
        if (activo is null && proyectos.Count > 1 && mensaje.OpcionId is null &&
            int.TryParse(mensaje.Texto.Trim(), out var indice) &&
            indice >= 1 && indice <= proyectos.Count)
        {
            var elegido = proyectos[indice - 1];
            var pendiente = contacto.OpcionPendiente;
            await ActivarProyectoAsync(contacto, elegido.Id, ct);
            await EjecutarOpcionAsync(contacto, elegido, pendiente, proyectos.Count > 1,
                mensaje.De, saludo: "", ct);
            return;
        }

        if (proyectos.Count == 0)
        {
            // Decisión B: redacción neutra, sin "tienes"/"pídele".
            await EnviarTextoAsync(contacto, mensaje.De,
                $"{saludo}Este número aún no tiene proyectos asignados. " +
                "El administrador puede asignarlos en el panel de Helvis y seguimos hablando. 🙌", ct);
            return;
        }

        if (activo is null)
        {
            if (proyectos.Count == 1)
            {
                activo = proyectos[0];
                await ActivarProyectoAsync(contacto, activo.Id, ct);
                // continúa al menú/opción con el único proyecto
            }
            else
            {
                // Decisión H: recordar qué pidió para responderlo tras elegir proyecto.
                contacto.OpcionPendiente = opcion is "avance" or "alertas" or "informe" or "reportar" or "bloqueo" or "revisar"
                    ? opcion
                    : mensaje.OpcionId is null && contacto.PuedeReportar && EsPosibleReporte(mensaje.Texto) ? "reportar" : null;
                await _db.SaveChangesAsync(ct);
                await EnviarSelectorProyectoAsync(contacto, proyectos, mensaje, saludo, ct);
                return;
            }
        }

        // Un texto que parece reporte de obra ("hoy hincamos 80 pilotes"): quien puede
        // reportar arranca el reporte con ese mismo texto (no se le vuelve a preguntar lo
        // que ya dijo); quien no, recibe el aviso de que no quedó guardado (decisión D).
        if (opcion is null && mensaje.OpcionId is null && !esEscape && EsPosibleReporte(mensaje.Texto))
        {
            if (contacto.PuedeReportar)
            {
                await IniciarReporteAsync(contacto, activo, mensaje.Texto, saludo, ct);
                return;
            }
            await EnviarTextoAsync(contacto, mensaje.De,
                saludo + "📝 Este número no tiene habilitado el permiso para reportar avance, así que el " +
                "mensaje *no quedó guardado* como avance. El administrador puede activarlo en el panel de Helvis.", ct);
            return;
        }

        await EjecutarOpcionAsync(contacto, activo, opcion, proyectos.Count > 1, mensaje.De, saludo, ct);
    }

    /// <summary>Despacha la opción elegida; cualquier otra cosa muestra el menú del proyecto.</summary>
    private async Task EjecutarOpcionAsync(ContactoHelvis contacto, Proyecto proyecto, string? opcion,
        bool variosProyectos, string numero, string saludo, CancellationToken ct)
    {
        switch (opcion)
        {
            case "avance":
                await EnviarAvanceAsync(contacto, proyecto, numero, ct);
                break;

            case "alertas":
                await EnviarAlertasAsync(contacto, proyecto, numero, ct);
                break;

            case "informe":
                await EnviarActividadesAsignadasAsync(contacto, proyecto, numero, ct);
                break;

            case "reportar" when contacto.PuedeReportar:
                await IniciarReporteAsync(contacto, proyecto, null, saludo, ct);
                break;

            case "bloqueo" when contacto.PuedeReportar:
                await IniciarBloqueoAsync(contacto, proyecto, saludo, ct);
                break;

            case "revisar" when contacto.PuedeAprobar:
                await EnviarInformeParaRevisarAsync(contacto, proyecto, numero, saludo, ct);
                break;

            case "reportar" or "bloqueo" or "revisar":
                await EnviarTextoAsync(contacto, numero,
                    $"{saludo}Este número no tiene habilitado ese permiso. El administrador puede activarlo en el panel de Helvis.", ct);
                await EnviarMenuProyectoAsync(contacto, proyecto, variosProyectos, numero, "", ct);
                break;

            default:
                await EnviarMenuProyectoAsync(contacto, proyecto, variosProyectos, numero, saludo, ct);
                break;
        }
    }

    // ==================== Proyecto activo ====================

    private static Proyecto? ProyectoActivo(ContactoHelvis contacto, List<Proyecto> proyectos)
    {
        if (contacto.ProyectoActivoId is not int id) return null;
        if (contacto.ProyectoActivoFecha is not DateTime fecha ||
            DateTime.UtcNow - fecha > VigenciaProyectoActivo) return null;
        return proyectos.FirstOrDefault(p => p.Id == id);
    }

    private async Task ActivarProyectoAsync(ContactoHelvis contacto, int proyectoId, CancellationToken ct)
    {
        contacto.ProyectoActivoId = proyectoId;
        contacto.ProyectoActivoFecha = DateTime.UtcNow;
        contacto.OpcionPendiente = null; // la opción recordada se consume al elegir
        await _db.SaveChangesAsync(ct);
    }

    private async Task EnviarSelectorProyectoAsync(ContactoHelvis contacto, List<Proyecto> proyectos,
        MensajeWhatsApp mensaje, string saludo, CancellationToken ct)
    {
        if (proyectos.Count == 0)
        {
            await EnviarTextoAsync(contacto, mensaje.De,
                saludo + "Este número aún no tiene proyectos asignados. El administrador puede asignarlos en el panel de Helvis.", ct);
            return;
        }

        if (proyectos.Count <= 3)
        {
            var cuerpo = $"{saludo}¿Sobre qué proyecto trabajamos hoy?";
            var botones = proyectos
                .Select(p => ($"proy_{p.Id}", string.IsNullOrWhiteSpace(p.Codigo) ? p.Nombre : p.Codigo))
                .ToArray();
            var ok = await _wa.EnviarBotonesAsync(mensaje.De, cuerpo, botones, ct);
            await GuardarSalienteAsync(contacto, mensaje.De, cuerpo, "interactive", ok, ct);
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(saludo).AppendLine("¿Sobre qué proyecto trabajamos hoy? Responde con el número:");
            for (var i = 0; i < proyectos.Count; i++)
                sb.AppendLine($"*{i + 1}.* {proyectos[i].Codigo} — {proyectos[i].Nombre}");
            await EnviarTextoAsync(contacto, mensaje.De, sb.ToString().TrimEnd(), ct);
        }
    }

    // ==================== Menú del proyecto ====================

    private async Task EnviarMenuProyectoAsync(ContactoHelvis contacto, Proyecto proyecto,
        bool variosProyectos, string numero, string saludo, CancellationToken ct)
    {
        var cuerpo = $"{saludo}📍 *{proyecto.Codigo} — {proyecto.Nombre}*\n\n¿Qué hacemos?";
        if (variosProyectos) cuerpo += "\n\n_Escribe *proyecto* para cambiar de proyecto._";

        var opciones = OpcionesMenu(contacto);

        // Decisión J: con más de 3 opciones (botones máximo 3) el menú pasa a lista.
        bool ok;
        if (opciones.Count <= 3)
            ok = await _wa.EnviarBotonesAsync(numero, cuerpo, opciones.Select(o => (o.Id, o.Titulo)), ct);
        else
            ok = await _wa.EnviarListaAsync(numero, cuerpo, "Ver opciones", opciones, ct);

        await GuardarSalienteAsync(contacto, numero, cuerpo, "interactive", ok, ct);
    }

    /// <summary>Opciones del menú según los permisos de la persona (documento §3).</summary>
    internal static List<(string Id, string Titulo, string? Descripcion)> OpcionesMenu(ContactoHelvis contacto)
    {
        var opciones = new List<(string Id, string Titulo, string? Descripcion)>();
        if (contacto.PuedeReportar)
        {
            opciones.Add(("reportar", "✍️ Reportar avance", "Registrar el avance de una actividad"));
            opciones.Add(("bloqueo", "🚧 Novedad o bloqueo", "Maquinaria, clima, suministros, pagos"));
        }
        if (contacto.PuedeAprobar)
            opciones.Add(("revisar", "✅ Revisar informe", "Aprobar o devolver el informe diario"));
        opciones.Add(("avance", "📊 Ver avance", "Curva S y actividades atrasadas"));
        opciones.Add(("alertas", "📋 Alertas", "Documentos pendientes de atención"));
        opciones.Add(("informe", "📝 Informe diario", "Actividades asignadas y lo reportado hoy"));
        return opciones;
    }

    // ==================== Ver avance (Curva S) ====================

    private async Task EnviarAvanceAsync(ContactoHelvis contacto, Proyecto proyecto,
        string numero, CancellationToken ct)
    {
        var curva = await _informes.DatosCurvaSAsync(proyecto.Id);
        var prog = curva.PlanificadoHoy;
        var real = curva.EjecutadoHoy;
        var brecha = real - prog;
        var icono = brecha >= -1 ? "✅" : brecha >= -5 ? "⚠️" : "🔴";

        // Actividades más atrasadas: primero las asignadas al contacto; si no tiene, las del proyecto.
        var asignadas = await _db.AsignacionesHelvis.AsNoTracking()
            .Where(a => a.ContactoHelvisId == contacto.Id && a.Activa &&
                        a.Actividad.ProyectoId == proyecto.Id &&
                        a.Actividad.CronogramaVersion != null && a.Actividad.CronogramaVersion.EsVigente)
            .Select(a => new { a.Actividad.CodigoWBS, a.Actividad.Nombre, a.Actividad.AvanceReal, a.Actividad.AvancePlanificado })
            .ToListAsync(ct);

        var titulo = "Actividades asignadas más atrasadas";
        var fuente = asignadas
            .Where(a => a.AvanceReal < a.AvancePlanificado)
            .OrderByDescending(a => a.AvancePlanificado - a.AvanceReal)
            .Take(3)
            .Select(a => (a.CodigoWBS, a.Nombre, a.AvanceReal, a.AvancePlanificado))
            .ToList();

        if (fuente.Count == 0)
        {
            titulo = "Actividades más atrasadas del proyecto";
            var actividades = await _db.ActividadesWBS.AsNoTracking()
                .Where(a => a.ProyectoId == proyecto.Id && a.CronogramaVersion != null && a.CronogramaVersion.EsVigente)
                .Select(a => new { a.Id, a.ActividadPadreId, a.CodigoWBS, a.Nombre, a.AvanceReal, a.AvancePlanificado })
                .ToListAsync(ct);
            var padres = actividades.Where(a => a.ActividadPadreId != null).Select(a => a.ActividadPadreId!.Value).ToHashSet();
            fuente = actividades
                .Where(a => !padres.Contains(a.Id) && a.AvanceReal < a.AvancePlanificado)
                .OrderByDescending(a => a.AvancePlanificado - a.AvanceReal)
                .Take(3)
                .Select(a => (a.CodigoWBS, a.Nombre, a.AvanceReal, a.AvancePlanificado))
                .ToList();
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📊 *{proyecto.Codigo} — avance al {SeguimientoDocumento.HoyColombia():dd/MM/yyyy}*");
        sb.AppendLine();
        sb.AppendLine($"Programado: *{prog:N0}%* · Real: *{real:N0}%*");
        sb.AppendLine($"Brecha: *{brecha:+0.#;-0.#;0} p.p.* {icono}");

        if (fuente.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"*{titulo}:*");
            foreach (var a in fuente)
                sb.AppendLine($"• {a.CodigoWBS} {Recortar(a.Nombre, 48)} — {a.AvanceReal:N0}% (prog. {a.AvancePlanificado:N0}%)");
        }

        sb.AppendLine();
        sb.AppendLine("Escribe *menú* para volver.");

        await EnviarTextoAsync(contacto, numero, sb.ToString().TrimEnd(), ct);
    }

    // ==================== Alertas (planificación documental) ====================

    private async Task EnviarAlertasAsync(ContactoHelvis contacto, Proyecto proyecto,
        string numero, CancellationToken ct)
    {
        var documentos = await _db.Documentos.AsNoTracking()
            .Where(d => d.ProyectoId == proyecto.Id)
            .ToListAsync(ct);

        var pendientes = documentos.Where(SeguimientoDocumento.RequiereAtencion).ToList();
        var rojos = pendientes.Where(d => SeguimientoDocumento.Semaforo(d) == SemaforoDocumento.Rojo)
            .OrderByDescending(d => SeguimientoDocumento.DiasSinAtender(d) ?? 0).ToList();
        var amarillos = pendientes.Where(d => SeguimientoDocumento.Semaforo(d) == SemaforoDocumento.Amarillo)
            .OrderByDescending(d => SeguimientoDocumento.DiasSinAtender(d) ?? 0).ToList();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📋 *{proyecto.Codigo} — alertas documentales*");
        sb.AppendLine();

        if (pendientes.Count == 0)
        {
            sb.AppendLine("✅ Sin documentos pendientes de atención. ¡Todo al día!");
        }
        else
        {
            sb.AppendLine($"🔴 Críticos ({SeguimientoDocumento.DiasRojo}+ días): *{rojos.Count}*");
            sb.AppendLine($"🟡 En alerta ({SeguimientoDocumento.DiasAmarillo}-{SeguimientoDocumento.DiasRojo - 1} días): *{amarillos.Count}*");
            sb.AppendLine($"📄 Pendientes en total: *{pendientes.Count}*");

            var destacados = rojos.Concat(amarillos).Take(5).ToList();
            if (destacados.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("*Los más urgentes:*");
                foreach (var d in destacados)
                {
                    var dias = SeguimientoDocumento.DiasSinAtender(d);
                    var marca = SeguimientoDocumento.Semaforo(d) == SemaforoDocumento.Rojo ? "🔴" : "🟡";
                    sb.AppendLine($"{marca} {d.Codigo} {Recortar(d.Titulo, 40)} — {(dias is null ? "sin fecha" : $"{dias} días")}, en cancha de {SeguimientoDocumento.EnCanchaDe(d)}");
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine("Escribe *menú* para volver.");

        await EnviarTextoAsync(contacto, numero, sb.ToString().TrimEnd(), ct);
    }

    // ==================== Informe diario (actividades asignadas) ====================

    /// <summary>
    /// Responde al botón "Informe diario" con las actividades asignadas al contacto
    /// EN EL PROYECTO ACTIVO. El registro del avance se hace con la opción Reportar avance.
    /// </summary>
    private async Task EnviarActividadesAsignadasAsync(ContactoHelvis contacto, Proyecto proyecto,
        string numero, CancellationToken ct)
    {
        var asignadas = await _db.AsignacionesHelvis.AsNoTracking()
            .Where(a => a.ContactoHelvisId == contacto.Id && a.Activa &&
                        a.Actividad.ProyectoId == proyecto.Id &&
                        a.Actividad.CronogramaVersion != null && a.Actividad.CronogramaVersion.EsVigente)
            .Select(a => new { a.Actividad.CodigoWBS, a.Actividad.Nombre, a.Actividad.AvanceReal, a.Frente })
            .OrderBy(a => a.CodigoWBS)
            .ToListAsync(ct);

        string texto;
        if (asignadas.Count == 0)
        {
            texto = $"📝 *Informe diario — {proyecto.Codigo}*\n\n" +
                    "Aún no hay actividades asignadas a este número en este proyecto. " +
                    "Se asignan desde el panel de Helvis en RenergeIA.\n\n" +
                    "Escribe *menú* para volver.";
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"📝 *Informe diario — {proyecto.Codigo}*");
            sb.AppendLine();
            sb.AppendLine("Actividades asignadas:");
            foreach (var a in asignadas.Take(20))
            {
                var frente = string.IsNullOrWhiteSpace(a.Frente) ? "" : $" ({a.Frente})";
                sb.AppendLine($"• {a.CodigoWBS} {Recortar(a.Nombre, 48)}{frente} — {a.AvanceReal:N0}%");
            }
            if (asignadas.Count > 20) sb.AppendLine($"… y {asignadas.Count - 20} más.");
            sb.AppendLine();
            sb.AppendLine(contacto.PuedeReportar
                ? "Para registrar el avance escribe *reportar* (o cuéntalo directo, ej. _hoy hincamos 80 pilotes_)."
                : "Escribe *menú* para volver.");
            texto = sb.ToString().TrimEnd();
        }

        await EnviarTextoAsync(contacto, numero, texto, ct);
    }

    // ==================== Registro del historial ====================

    /// <summary>Guarda el mensaje entrante. Devuelve false si ya estaba registrado (reintento de Meta).</summary>
    private async Task<bool> GuardarEntranteAsync(MensajeWhatsApp mensaje, ContactoHelvis? contacto, CancellationToken ct)
    {
        if (await _db.MensajesHelvis.AnyAsync(x => x.WaMessageId == mensaje.Id, ct))
        {
            _log.LogInformation("WhatsApp: mensaje {Id} ya procesado antes; se ignora el reintento.", mensaje.Id);
            return false;
        }

        _db.MensajesHelvis.Add(new MensajeHelvis
        {
            ContactoHelvisId = contacto?.Id,
            Telefono = mensaje.De,
            Entrante = true,
            WaMessageId = mensaje.Id,
            Texto = mensaje.Texto,
            Tipo = mensaje.Tipo,
            OpcionId = mensaje.OpcionId
        });

        try
        {
            await _db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException)
        {
            // Dos instancias recibieron el mismo reintento a la vez: el índice único decide.
            _log.LogInformation("WhatsApp: mensaje {Id} insertado por otra instancia; se ignora.", mensaje.Id);
            return false;
        }
    }

    private async Task EnviarTextoAsync(ContactoHelvis? contacto, string numero, string texto, CancellationToken ct)
    {
        var ok = await _wa.EnviarTextoAsync(numero, texto, ct);
        await GuardarSalienteAsync(contacto, numero, texto, "text", ok, ct);
    }

    private async Task GuardarSalienteAsync(ContactoHelvis? contacto, string numero, string texto,
        string tipo, bool ok, CancellationToken ct)
    {
        _db.MensajesHelvis.Add(new MensajeHelvis
        {
            ContactoHelvisId = contacto?.Id,
            Telefono = numero,
            Entrante = false,
            Texto = texto,
            Tipo = tipo,
            Estado = ok ? "enviado" : "fallo"
        });
        await _db.SaveChangesAsync(ct);
    }

    // ==================== Desconocidos ====================

    /// <summary>
    /// Número no registrado (o contacto desactivado): pregunta cordialmente qué necesita,
    /// conserva todo lo que escriba (queda visible en el panel de Helvis) y nunca entrega
    /// información ni datos del equipo. Decisión F: a los administradores les llega el aviso.
    /// </summary>
    private async Task AtenderDesconocidoAsync(MensajeWhatsApp mensaje, bool esDesconocido, CancellationToken ct)
    {
        // Si ya se le dio la bienvenida hace poco, no repetirla con cada mensaje.
        var limite = DateTime.UtcNow.AddHours(-24);
        var yaSaludado = await _db.MensajesHelvis.AnyAsync(x =>
            x.Telefono == mensaje.De && !x.Entrante && x.FechaCreacion >= limite, ct);

        var texto = yaSaludado
            ? "Gracias, el mensaje quedó registrado y el equipo de Renergeia lo revisará. 🙏"
            : "Hola 👋 Soy *Helvis*, el asistente de *Renergeia*.\n\n" +
              "Este número aún no está registrado conmigo. ¿Me cuentas el motivo del mensaje y con quién " +
              "quieres hablar? Dejo el aviso al equipo de Renergeia para que te contacten.";

        await EnviarTextoAsync(null, mensaje.De, texto, ct);

        // Decisión F: avisar a los administradores (solo si es un número realmente desconocido;
        // un contacto desactivado ya es conocido y su mensaje queda en el panel).
        if (esDesconocido)
            await AvisarAdministradoresAsync(mensaje, ct);
    }

    /// <summary>
    /// Reenvía a los administradores activos el número y el mensaje de un desconocido,
    /// sin compartirles al desconocido ningún dato del equipo. Si el administrador no ha
    /// escrito en 24 h, Meta puede rechazar el envío (queda en "fallo" en el panel).
    /// </summary>
    private async Task AvisarAdministradoresAsync(MensajeWhatsApp mensaje, CancellationToken ct)
    {
        var admins = await _db.ContactosHelvis
            .Where(c => c.Activo && c.EsAdministrador && c.Telefono != mensaje.De)
            .ToListAsync(ct);
        if (admins.Count == 0) return;

        var detalle = string.IsNullOrWhiteSpace(mensaje.Texto)
            ? "(envió un archivo o una nota de voz)"
            : $"«{Recortar(mensaje.Texto, 300)}»";
        var aviso = $"📨 *Aviso de Helvis:* el número +{mensaje.De} escribió sin estar registrado:\n\n" +
                    $"{detalle}\n\nLa conversación completa está en el panel de Helvis.";

        foreach (var admin in admins)
            await EnviarTextoAsync(admin, admin.Telefono, aviso, ct);
    }

    // ==================== Utilidades ====================

    private static string Saludo(ContactoHelvis contacto, MensajeWhatsApp mensaje)
    {
        var nombre = PrimerNombre(string.IsNullOrWhiteSpace(contacto.Nombre) ? mensaje.Nombre : contacto.Nombre);
        var saludo = string.IsNullOrWhiteSpace(nombre) ? "Hola 👋" : $"Hola, {nombre} 👋";
        return $"{saludo} Soy *Helvis*, el asistente de Renergeia.";
    }

    /// <summary>¿Es el primer contacto del día (hora Colombia)? Entonces se saluda por el nombre.</summary>
    private async Task<bool> PrimeraVezHoyAsync(string numero, CancellationToken ct)
    {
        var inicioDiaUtc = DateTime.SpecifyKind(SeguimientoDocumento.HoyColombia().AddHours(5), DateTimeKind.Utc);
        return !await _db.MensajesHelvis.AnyAsync(x =>
            x.Telefono == numero && !x.Entrante && x.FechaCreacion >= inicioDiaUtc, ct);
    }

    /// <summary>Permite responder con número o con palabras, no solo con los botones.</summary>
    private static string? Interpretar(string texto)
    {
        var t = Normalizar(texto);
        if (t.Length == 0) return null;

        if (t.StartsWith("report") || t.StartsWith("registr")) return "reportar";
        if (t.Contains("bloque") || t.Contains("novedad") || t.Contains("varad")) return "bloqueo";
        if (t.Contains("aprob") || t.Contains("revisar")) return "revisar";
        if (t is "1" || t.Contains("avance") || t.Contains("consulta")) return "avance";
        if (t is "2" || t.Contains("alerta") || t.Contains("vencim") || t.Contains("pendiente")) return "alertas";
        if (t is "3" || t.Contains("informe") || t.Contains("reporte") || t.Contains("diario")) return "informe";
        if (t.Contains("costo") || t.Contains("presupuesto")) return "costos";
        if (t.Contains("proyecto") || t is "cambiar") return "cambiar";

        return null; // cualquier otra cosa devuelve el menú
    }

    /// <summary>
    /// Heurística de la decisión D: ¿el texto parece un reporte de obra?
    /// (números con varias palabras, o vocabulario típico de avance). Sirve solo para
    /// avisar que el reporte aún no se guarda; no intenta entender el contenido.
    /// </summary>
    private static bool EsPosibleReporte(string texto)
    {
        var t = Normalizar(texto);
        if (t.Length < 8) return false;

        string[] claves =
        [
            "hincad", "pilot", "instal", "excav", "vaciad", "fundid", "montaj", "zanja",
            "panel", "modulo", "mesa", "cable", "avanzamos", "terminamos", "hicimos",
            "ejecutamos", "metros", " ml", " m2", " m3", " und"
        ];
        var tieneClave = claves.Any(t.Contains);
        var tieneNumero = t.Any(char.IsDigit);
        var palabras = t.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        return tieneClave || (tieneNumero && palabras >= 4);
    }

    private static string Normalizar(string texto)
    {
        var sinTildes = new string(texto.Trim().ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD)
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                        != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());
        return sinTildes.Normalize(System.Text.NormalizationForm.FormC);
    }

    private static string PrimerNombre(string nombre) =>
        nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? nombre;

    private static string Recortar(string texto, int largo) =>
        texto.Length <= largo ? texto : texto[..(largo - 1)] + "…";
}
