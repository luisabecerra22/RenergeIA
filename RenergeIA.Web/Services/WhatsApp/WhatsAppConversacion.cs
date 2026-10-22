using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities.Helvis;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Lógica de Helvis: es deliberadamente determinista —el código decide, no un modelo—
/// para que las respuestas sean predecibles y auditables.
/// Etapa 1: identifica a la persona por su celular (ContactosHelvis), guarda TODO el
/// historial en MensajesHelvis (que además deduplica reintentos de Meta aunque el
/// servidor se reinicie) y atiende cordialmente a los números no registrados sin
/// entregarles información (ESPECIFICACION_HELVIS §3).
/// </summary>
public class WhatsAppConversacion
{
    private readonly WhatsAppClient _wa;
    private readonly RenergeIADbContext _db;
    private readonly ILogger<WhatsAppConversacion> _log;

    public WhatsAppConversacion(WhatsAppClient wa, RenergeIADbContext db, ILogger<WhatsAppConversacion> log)
    {
        _wa = wa;
        _db = db;
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

        var opcion = mensaje.OpcionId ?? Interpretar(mensaje.Texto);

        switch (opcion)
        {
            case "alertas":
                await EnviarTextoAsync(contacto, mensaje.De,
                    "📋 *Alertas y vencimientos*\n\n" +
                    "Aquí verás los documentos por vencer y tus pendientes de planificación documental.\n\n" +
                    "Esta opción se conecta en la fase 1. Escribe *menú* para volver.", ct);
                break;

            case "consultas":
                await EnviarTextoAsync(contacto, mensaje.De,
                    "📊 *Consultas del proyecto*\n\n" +
                    "Aquí podrás preguntar por avance, documentos y costos.\n\n" +
                    "Esta opción se conecta en la fase 2. Escribe *menú* para volver.", ct);
                break;

            case "informe":
                await EnviarActividadesAsignadasAsync(contacto, mensaje.De, ct);
                break;

            default:
                await EnviarMenuAsync(contacto, mensaje, ct);
                break;
        }
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

    // ==================== Conversación ====================

    /// <summary>
    /// Responde al botón "Informe diario" con las actividades asignadas al contacto
    /// (solo las de cronogramas VIGENTES). El registro de avance llega en la Etapa 2.
    /// </summary>
    private async Task EnviarActividadesAsignadasAsync(ContactoHelvis contacto, string numero, CancellationToken ct)
    {
        var asignadas = await _db.AsignacionesHelvis.AsNoTracking()
            .Where(a => a.ContactoHelvisId == contacto.Id && a.Activa &&
                        a.Actividad.CronogramaVersion != null && a.Actividad.CronogramaVersion.EsVigente)
            .Select(a => new
            {
                Proyecto = a.Actividad.Proyecto.Codigo,
                a.Actividad.CodigoWBS,
                a.Actividad.Nombre,
                a.Actividad.AvanceReal,
                a.Frente
            })
            .OrderBy(a => a.Proyecto).ThenBy(a => a.CodigoWBS)
            .ToListAsync(ct);

        string texto;
        if (asignadas.Count == 0)
        {
            texto = "📝 *Informe diario*\n\n" +
                    "Aún no tienes actividades asignadas. Se asignan desde el panel de Helvis en RenergeIA.\n\n" +
                    "Escribe *menú* para volver.";
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("📝 *Informe diario*");
            sb.AppendLine();
            sb.AppendLine("Estas son tus actividades asignadas:");
            string? proyectoActual = null;
            foreach (var a in asignadas.Take(20))
            {
                if (a.Proyecto != proyectoActual)
                {
                    proyectoActual = a.Proyecto;
                    sb.AppendLine();
                    sb.AppendLine($"*{a.Proyecto}*");
                }
                var frente = string.IsNullOrWhiteSpace(a.Frente) ? "" : $" ({a.Frente})";
                sb.AppendLine($"• {a.CodigoWBS} {a.Nombre}{frente} — {a.AvanceReal:N0}%");
            }
            if (asignadas.Count > 20) sb.AppendLine($"… y {asignadas.Count - 20} más.");
            sb.AppendLine();
            sb.AppendLine("El registro de avance por aquí se conecta en la Etapa 2. Escribe *menú* para volver.");
            texto = sb.ToString();
        }

        await EnviarTextoAsync(contacto, numero, texto, ct);
    }

    private async Task EnviarMenuAsync(ContactoHelvis contacto, MensajeWhatsApp mensaje, CancellationToken ct)
    {
        var nombre = PrimerNombre(string.IsNullOrWhiteSpace(contacto.Nombre) ? mensaje.Nombre : contacto.Nombre);
        var saludo = string.IsNullOrWhiteSpace(nombre) ? "Hola 👋" : $"Hola, {nombre} 👋";

        var cuerpo = $"{saludo}\n\nSoy *Helvis*, el asistente de Renergeia _(prueba)_. ¿Con qué te ayudo?";

        var ok = await _wa.EnviarBotonesAsync(mensaje.De, cuerpo,
            new[]
            {
                ("alertas", "📋 Alertas"),
                ("consultas", "📊 Consultas"),
                ("informe", "📝 Informe diario")
            }, ct);

        await GuardarSalienteAsync(contacto, mensaje.De, cuerpo, "interactive", ok, ct);
    }

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

    /// <summary>Permite responder con número o con palabras, no solo con los botones.</summary>
    private static string? Interpretar(string texto)
    {
        var t = Normalizar(texto);
        if (t.Length == 0) return null;

        if (t is "1" || t.Contains("alerta") || t.Contains("vencim") || t.Contains("pendiente")) return "alertas";
        if (t is "2" || t.Contains("consulta") || t.Contains("avance") || t.Contains("costo") || t.Contains("documento")) return "consultas";
        if (t is "3" || t.Contains("informe") || t.Contains("reporte") || t.Contains("diario")) return "informe";

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
}
