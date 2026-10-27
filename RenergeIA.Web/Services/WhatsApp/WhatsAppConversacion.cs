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
/// </summary>
public class WhatsAppConversacion
{
    /// <summary>Cuánto tiempo recuerda Helvis el proyecto elegido antes de volver a preguntar.</summary>
    private static readonly TimeSpan VigenciaProyectoActivo = TimeSpan.FromHours(12);

    private readonly WhatsAppClient _wa;
    private readonly RenergeIADbContext _db;
    private readonly InformeDiarioService _informes;
    private readonly ILogger<WhatsAppConversacion> _log;

    public WhatsAppConversacion(WhatsAppClient wa, RenergeIADbContext db,
        InformeDiarioService informes, ILogger<WhatsAppConversacion> log)
    {
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
            await AtenderDesconocidoAsync(mensaje, ct);
            return;
        }

        // El filtro de soft-delete deja el Proyecto en null si está en la papelera.
        var proyectos = contacto.Proyectos
            .Where(p => p.Proyecto is not null)
            .Select(p => p.Proyecto)
            .OrderBy(p => p.Nombre)
            .ToList();

        var opcion = mensaje.OpcionId ?? Interpretar(mensaje.Texto);

        // Cambio explícito de proyecto
        if (opcion == "cambiar")
        {
            await EnviarSelectorProyectoAsync(contacto, proyectos, mensaje, saludar: false, ct);
            return;
        }

        // Botón de proyecto pulsado
        if (opcion is not null && opcion.StartsWith("proy_") &&
            int.TryParse(opcion[5..], out var elegidoId) &&
            proyectos.Any(p => p.Id == elegidoId))
        {
            await ActivarProyectoAsync(contacto, elegidoId, ct);
            await EnviarMenuProyectoAsync(contacto, proyectos.First(p => p.Id == elegidoId),
                proyectos.Count > 1, mensaje.De, ct);
            return;
        }

        var activo = ProyectoActivo(contacto, proyectos);

        // Respuesta numérica mientras se espera elegir proyecto (lista numerada con 4+)
        if (activo is null && proyectos.Count > 1 && mensaje.OpcionId is null &&
            int.TryParse(mensaje.Texto.Trim(), out var indice) &&
            indice >= 1 && indice <= proyectos.Count)
        {
            var elegido = proyectos[indice - 1];
            await ActivarProyectoAsync(contacto, elegido.Id, ct);
            await EnviarMenuProyectoAsync(contacto, elegido, proyectos.Count > 1, mensaje.De, ct);
            return;
        }

        if (proyectos.Count == 0)
        {
            await EnviarTextoAsync(contacto, mensaje.De,
                $"{Saludo(contacto, mensaje)}\n\n" +
                "Aún no tienes proyectos asignados. Pídele al administrador que te los asigne " +
                "en el panel de Helvis y volvemos a hablar. 🙌", ct);
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
                await EnviarSelectorProyectoAsync(contacto, proyectos, mensaje, saludar: true, ct);
                return;
            }
        }

        switch (opcion)
        {
            case "avance":
                await EnviarAvanceAsync(contacto, activo, mensaje.De, ct);
                break;

            case "alertas":
                await EnviarAlertasAsync(contacto, activo, mensaje.De, ct);
                break;

            case "informe":
                await EnviarActividadesAsignadasAsync(contacto, activo, mensaje.De, ct);
                break;

            default:
                await EnviarMenuProyectoAsync(contacto, activo, proyectos.Count > 1, mensaje.De, ct);
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
        await _db.SaveChangesAsync(ct);
    }

    private async Task EnviarSelectorProyectoAsync(ContactoHelvis contacto, List<Proyecto> proyectos,
        MensajeWhatsApp mensaje, bool saludar, CancellationToken ct)
    {
        if (proyectos.Count == 0)
        {
            await EnviarTextoAsync(contacto, mensaje.De,
                "Aún no tienes proyectos asignados. Pídele al administrador que te los asigne en el panel de Helvis.", ct);
            return;
        }

        var encabezado = saludar ? $"{Saludo(contacto, mensaje)}\n\n" : "";

        if (proyectos.Count <= 3)
        {
            var cuerpo = $"{encabezado}¿Sobre qué proyecto trabajamos hoy?";
            var botones = proyectos
                .Select(p => ($"proy_{p.Id}", string.IsNullOrWhiteSpace(p.Codigo) ? p.Nombre : p.Codigo))
                .ToArray();
            var ok = await _wa.EnviarBotonesAsync(mensaje.De, cuerpo, botones, ct);
            await GuardarSalienteAsync(contacto, mensaje.De, cuerpo, "interactive", ok, ct);
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(encabezado).AppendLine("¿Sobre qué proyecto trabajamos hoy? Responde con el número:");
            for (var i = 0; i < proyectos.Count; i++)
                sb.AppendLine($"*{i + 1}.* {proyectos[i].Codigo} — {proyectos[i].Nombre}");
            await EnviarTextoAsync(contacto, mensaje.De, sb.ToString().TrimEnd(), ct);
        }
    }

    // ==================== Menú del proyecto ====================

    private async Task EnviarMenuProyectoAsync(ContactoHelvis contacto, Proyecto proyecto,
        bool variosProyectos, string numero, CancellationToken ct)
    {
        var cuerpo = $"📍 *{proyecto.Codigo} — {proyecto.Nombre}*\n\n¿Qué necesitas?";
        if (variosProyectos) cuerpo += "\n\n_Escribe *proyecto* para cambiar de proyecto._";

        var ok = await _wa.EnviarBotonesAsync(numero, cuerpo,
            new[]
            {
                ("avance", "📊 Ver avance"),
                ("alertas", "📋 Alertas"),
                ("informe", "📝 Informe diario")
            }, ct);

        await GuardarSalienteAsync(contacto, numero, cuerpo, "interactive", ok, ct);
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

        var titulo = "Tus actividades más atrasadas";
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
    /// EN EL PROYECTO ACTIVO. El registro conversacional de avance llega en la Etapa 2.
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
                    "Aún no tienes actividades asignadas en este proyecto. " +
                    "Se asignan desde el panel de Helvis en RenergeIA.\n\n" +
                    "Escribe *menú* para volver.";
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"📝 *Informe diario — {proyecto.Codigo}*");
            sb.AppendLine();
            sb.AppendLine("Tus actividades asignadas:");
            foreach (var a in asignadas.Take(20))
            {
                var frente = string.IsNullOrWhiteSpace(a.Frente) ? "" : $" ({a.Frente})";
                sb.AppendLine($"• {a.CodigoWBS} {Recortar(a.Nombre, 48)}{frente} — {a.AvanceReal:N0}%");
            }
            if (asignadas.Count > 20) sb.AppendLine($"… y {asignadas.Count - 20} más.");
            sb.AppendLine();
            sb.AppendLine("El registro de avance por aquí se conecta en la Etapa 2. Escribe *menú* para volver.");
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
    /// Número no registrado: pregunta cordialmente qué necesita, conserva todo lo que escriba
    /// (queda visible en el panel de Helvis) y nunca entrega información ni datos del equipo.
    /// </summary>
    private async Task AtenderDesconocidoAsync(MensajeWhatsApp mensaje, CancellationToken ct)
    {
        // Si ya se le dio la bienvenida hace poco, no repetirla con cada mensaje.
        var limite = DateTime.UtcNow.AddHours(-24);
        var yaSaludado = await _db.MensajesHelvis.AnyAsync(x =>
            x.Telefono == mensaje.De && !x.Entrante && x.FechaCreacion >= limite, ct);

        var texto = yaSaludado
            ? "Gracias, quedó registrado tu mensaje y el equipo de Renergeia lo revisará. 🙏"
            : "Hola 👋 Soy *Helvis*, el asistente de *Renergeia*.\n\n" +
              "Este número aún no está registrado conmigo. ¿Me cuentas qué necesitas y con quién " +
              "te gustaría comunicarte? Dejaré el aviso al equipo para que te contacten.";

        await EnviarTextoAsync(null, mensaje.De, texto, ct);
    }

    // ==================== Utilidades ====================

    private static string Saludo(ContactoHelvis contacto, MensajeWhatsApp mensaje)
    {
        var nombre = PrimerNombre(string.IsNullOrWhiteSpace(contacto.Nombre) ? mensaje.Nombre : contacto.Nombre);
        var saludo = string.IsNullOrWhiteSpace(nombre) ? "Hola 👋" : $"Hola, {nombre} 👋";
        return $"{saludo} Soy *Helvis*, el asistente de Renergeia _(prueba)_.";
    }

    /// <summary>Permite responder con número o con palabras, no solo con los botones.</summary>
    private static string? Interpretar(string texto)
    {
        var t = Normalizar(texto);
        if (t.Length == 0) return null;

        if (t is "1" || t.Contains("avance") || t.Contains("consulta") || t.Contains("costo")) return "avance";
        if (t is "2" || t.Contains("alerta") || t.Contains("vencim") || t.Contains("pendiente")) return "alertas";
        if (t is "3" || t.Contains("informe") || t.Contains("reporte") || t.Contains("diario")) return "informe";
        if (t.Contains("proyecto") || t is "cambiar") return "cambiar";

        return null; // cualquier otra cosa devuelve el menú
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
