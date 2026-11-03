using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Entities.Helvis;
using RenergeIA.Core.Enums;
using RenergeIA.Core.Helpers;

namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Revisión del informe diario por WhatsApp (documento §12) y novedades/bloqueos (§11).
/// Aprobar o devolver deja el MISMO estado que en la app (un solo estado por informe):
/// si ya lo decidió otra persona, no se vuelve a aplicar (P39).
/// </summary>
public partial class WhatsAppConversacion
{
    // ==================== Revisión del informe ====================

    private async Task EnviarInformeParaRevisarAsync(ContactoHelvis contacto, Proyecto proyecto, string numero,
        string saludo, CancellationToken ct)
    {
        var informe = await _db.InformesDiarios
            .Where(i => i.ProyectoId == proyecto.Id && i.Estado == EstadoInforme.EnRevision)
            .OrderByDescending(i => i.Fecha).FirstOrDefaultAsync(ct)
            ?? await _db.InformesDiarios
                .Where(i => i.ProyectoId == proyecto.Id && i.Estado == EstadoInforme.Borrador && i.RegistrosAvance.Any())
                .OrderByDescending(i => i.Fecha).FirstOrDefaultAsync(ct);

        if (informe is null)
        {
            await EnviarTextoAsync(contacto, numero,
                saludo + $"✅ No hay informes diarios pendientes de revisión en {proyecto.Codigo}.", ct);
            return;
        }

        await EnviarSolicitudRevisionAsync(contacto, informe, proyecto, saludo.TrimEnd(), ct);
    }

    /// <summary>Resumen del informe con los botones Aprobar / Devolver.</summary>
    private async Task EnviarSolicitudRevisionAsync(ContactoHelvis aprobador, InformeDiario informe, Proyecto proyecto,
        string encabezado, CancellationToken ct)
    {
        if (!PuedeRecibirAvisos(aprobador)) return;
        var resumen = await ResumenInformeAsync(informe, proyecto, ct);
        var cuerpo = (string.IsNullOrWhiteSpace(encabezado) ? "" : encabezado + "\n\n") + resumen +
                     (informe.Estado == EstadoInforme.Borrador ? "\n\n_Aún está en borrador: puede faltar el aporte de alguien._" : "");
        await EnviarBotonesYGuardarAsync(aprobador, aprobador.Telefono, Recortar(cuerpo, 1000),
            [($"apr_{informe.Id}", "✅ Aprobar"), ($"dev_{informe.Id}", "↩️ Devolver")], ct);
    }

    private async Task<string> ResumenInformeAsync(InformeDiario informe, Proyecto proyecto, CancellationToken ct)
    {
        var registros = await _db.RegistrosAvanceDiario.AsNoTracking()
            .Where(r => r.InformeDiarioId == informe.Id)
            .Select(r => new { r.ActividadWBS.CodigoWBS, r.ActividadWBS.Nombre, r.ActividadWBS.Unidad,
                               r.AvanceAcumulado, r.CantidadEjecutadaDia, r.ReportadoPor })
            .ToListAsync(ct);
        var fotos = await _db.FotosHelvis.CountAsync(f => f.InformeDiarioId == informe.Id, ct);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📄 *Informe diario N° {informe.NumeroCertificado} — {proyecto.Codigo}*");
        sb.AppendLine($"Fecha: {informe.Fecha:dd/MM/yyyy} · {registros.Count} actividad(es){(fotos > 0 ? $" · {fotos} foto(s)" : "")}");
        sb.AppendLine();
        foreach (var r in registros.OrderBy(r => r.CodigoWBS).Take(12))
        {
            var cant = r.CantidadEjecutadaDia > 0 ? $" · {r.CantidadEjecutadaDia:0.##} {r.Unidad ?? "und"} del día" : "";
            sb.AppendLine($"• {r.CodigoWBS} {Recortar(r.Nombre, 40)}: *{r.AvanceAcumulado:0.#}%*{cant}");
        }
        if (registros.Count > 12) sb.AppendLine($"… y {registros.Count - 12} más.");
        var quienes = registros.SelectMany(r => r.ReportadoPor.Split(';', StringSplitOptions.TrimEntries))
            .Where(s => s.Length > 0).Distinct().ToList();
        if (quienes.Count > 0) sb.AppendLine($"\nReportó: {string.Join(", ", quienes)}");
        sb.Append($"Ver en la app: {EnlaceInforme(informe)}");
        return sb.ToString();
    }

    private string EnlaceInforme(InformeDiario informe) =>
        $"{_opciones.UrlApp.TrimEnd('/')}/proyectos/{informe.ProyectoId}/informes/{informe.Id}";

    /// <summary>Botón Aprobar (apr_ID) o Devolver (dev_ID), venga de un aviso o del menú.</summary>
    private async Task ResolverRevisionAsync(ContactoHelvis contacto, List<Proyecto> proyectos, string id,
        string numero, CancellationToken ct)
    {
        if (!int.TryParse(id[4..], out var informeId)) return;
        var informe = await _db.InformesDiarios.FirstOrDefaultAsync(i => i.Id == informeId, ct);
        var proyecto = informe is null ? null : proyectos.FirstOrDefault(p => p.Id == informe.ProyectoId);

        if (informe is null || proyecto is null)
        {
            await EnviarTextoAsync(contacto, numero, "Ese informe no existe o no corresponde a los proyectos de este número.", ct);
            return;
        }
        if (!contacto.PuedeAprobar)
        {
            await EnviarTextoAsync(contacto, numero, "Este número no tiene habilitado el permiso para aprobar informes.", ct);
            return;
        }
        if (informe.Estado == EstadoInforme.Aprobado)
        {
            await EnviarTextoAsync(contacto, numero,
                $"Este informe ya fue *aprobado* por {informe.RevisadoPor ?? "otra persona"}" +
                $"{(informe.FechaRevision is DateTime f ? $" el {f.AddHours(-5):dd/MM HH:mm}" : "")}. No hace falta hacer nada más.", ct);
            return;
        }

        if (id.StartsWith("dev_"))
        {
            contacto.Flujo = "devolver";
            contacto.FlujoPaso = "motivo";
            contacto.FlujoDatos = informe.Id.ToString();
            contacto.FlujoFecha = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            await EnviarTextoAsync(contacto, numero,
                $"↩️ ¿Qué hay que corregir en el informe N° {informe.NumeroCertificado} de {proyecto.Codigo}? " +
                "Escríbelo en un mensaje; se lo envío a quienes reportaron.", ct);
            return;
        }

        informe.Estado = EstadoInforme.Aprobado;
        informe.RevisadoPor = $"{contacto.Nombre} (WhatsApp)";
        informe.FechaRevision = DateTime.UtcNow;
        informe.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        await EnviarTextoAsync(contacto, numero,
            $"✅ Aprobado el informe N° {informe.NumeroCertificado} de {proyecto.Codigo} ({informe.Fecha:dd/MM/yyyy}). " +
            "Se lo envío a los destinatarios configurados.", ct);

        // Entrega automática del informe aprobado (§12.5): destinatarios del panel.
        var resumen = await ResumenInformeAsync(informe, proyecto, ct);
        var destinatarios = await _db.ContactosHelvis
            .Where(c => c.RecibeInformes && c.Activo && c.Id != contacto.Id)
            .ToListAsync(ct);
        foreach (var d in destinatarios.Where(PuedeRecibirAvisos))
            await EnviarTextoAsync(d, d.Telefono,
                Recortar($"📄 *Informe diario aprobado* por {contacto.Nombre}\n\n{resumen}", 4000), ct);

        await AvisarReportantesAsync(informe, contacto,
            $"✅ El informe del {informe.Fecha:dd/MM/yyyy} de {proyecto.Codigo} quedó *aprobado* por {contacto.Nombre}. ¡Gracias!", ct);
    }

    private async Task DevolverInformeAsync(ContactoHelvis contacto, List<Proyecto> proyectos, MensajeWhatsApp mensaje,
        CancellationToken ct)
    {
        var motivo = (mensaje.Texto ?? "").Trim();
        if (motivo.Length < 4 || mensaje.OpcionId is not null)
        {
            await EnviarTextoAsync(contacto, mensaje.De, "Escribe en un mensaje qué hay que corregir (o *cancelar*).", ct);
            return;
        }

        var informe = int.TryParse(contacto.FlujoDatos, out var id)
            ? await _db.InformesDiarios.FirstOrDefaultAsync(i => i.Id == id, ct) : null;
        var proyecto = informe is null ? null : proyectos.FirstOrDefault(p => p.Id == informe.ProyectoId);
        contacto.Flujo = null; contacto.FlujoPaso = null; contacto.FlujoDatos = null; contacto.FlujoFecha = null;

        if (informe is null || proyecto is null || informe.Estado == EstadoInforme.Aprobado)
        {
            await _db.SaveChangesAsync(ct);
            await EnviarTextoAsync(contacto, mensaje.De,
                informe?.Estado == EstadoInforme.Aprobado
                    ? $"Mientras tanto el informe fue aprobado por {informe.RevisadoPor}; no se devolvió."
                    : "No encontré el informe; no se devolvió nada.", ct);
            return;
        }

        informe.Estado = EstadoInforme.Rechazado;
        informe.MotivoRechazo = Recortar(motivo, 2000);
        informe.RevisadoPor = $"{contacto.Nombre} (WhatsApp)";
        informe.FechaRevision = DateTime.UtcNow;
        informe.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        var avisados = await AvisarReportantesAsync(informe, contacto,
            $"↩️ El informe del {informe.Fecha:dd/MM/yyyy} de {proyecto.Codigo} fue *devuelto* por {contacto.Nombre} para corregir:\n\n" +
            $"«{Recortar(motivo, 600)}»\n\nPara corregir, escribe *reportar* y envía el dato correcto; el informe vuelve a revisión cuando toques *Terminé por hoy*.", ct);

        await EnviarTextoAsync(contacto, mensaje.De,
            $"↩️ Devuelto el informe N° {informe.NumeroCertificado}. " +
            (avisados > 0 ? $"Le avisé a {avisados} persona(s) que reportaron." : "Nadie lo reportó por WhatsApp; la corrección se hace en la app."), ct);
    }

    /// <summary>Avisa a quienes reportaron por WhatsApp en ese informe. Devuelve a cuántos.</summary>
    private async Task<int> AvisarReportantesAsync(InformeDiario informe, ContactoHelvis excepto, string texto, CancellationToken ct)
    {
        var ids = await _db.ReportesAvanceHelvis
            .Where(r => r.InformeDiarioId == informe.Id && r.Estado == EstadosReporteHelvis.Confirmado && r.ContactoHelvisId != excepto.Id)
            .Select(r => r.ContactoHelvisId).Distinct().ToListAsync(ct);
        var personas = await _db.ContactosHelvis.Where(c => ids.Contains(c.Id)).ToListAsync(ct);
        var enviados = 0;
        foreach (var p in personas.Where(PuedeRecibirAvisos))
        {
            await EnviarTextoAsync(p, p.Telefono, texto, ct);
            enviados++;
        }
        return enviados;
    }

    private async Task<List<ContactoHelvis>> AprobadoresAsync(int proyectoId, int exceptoId, CancellationToken ct)
    {
        var lista = await _db.ContactosHelvis
            .Where(c => c.Activo && c.PuedeAprobar && c.Id != exceptoId && c.Proyectos.Any(p => p.ProyectoId == proyectoId))
            .ToListAsync(ct);
        return lista.Where(PuedeRecibirAvisos).ToList();
    }

    // ==================== Conversaciones de varios pasos ====================

    /// <summary>Continúa un flujo en curso (devolución o bloqueo). Devuelve false si no había ninguno.</summary>
    private async Task<bool> ContinuarFlujoAsync(ContactoHelvis contacto, List<Proyecto> proyectos, MensajeWhatsApp mensaje,
        CancellationToken ct)
    {
        if (contacto.Flujo is null) return false;
        if (contacto.FlujoFecha is not DateTime fecha || DateTime.UtcNow - fecha > VigenciaBorrador)
        {
            contacto.Flujo = null; contacto.FlujoPaso = null; contacto.FlujoDatos = null; contacto.FlujoFecha = null;
            await _db.SaveChangesAsync(ct);
            return false;
        }

        contacto.FlujoFecha = DateTime.UtcNow;
        switch (contacto.Flujo)
        {
            case "devolver":
                await DevolverInformeAsync(contacto, proyectos, mensaje, ct);
                return true;
            case "bloqueo":
                await ContinuarBloqueoAsync(contacto, mensaje, ct);
                return true;
            default:
                contacto.Flujo = null;
                await _db.SaveChangesAsync(ct);
                return false;
        }
    }

    // ==================== Novedades y bloqueos ====================

    private sealed class DatosBloqueo
    {
        public int ProyectoId { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public string? Impacto { get; set; }
        public int? ActividadId { get; set; }
        public string? ActividadTexto { get; set; }
        public int? RestriccionId { get; set; }
    }

    private static readonly (string Id, string Titulo, string Pregunta)[] TiposBloqueo =
    [
        ("maquinaria", "Maquinaria varada",
            "¿Qué máquina se varó, desde qué hora, si es propia o subcontratada, qué atención técnica ha tenido y a quién se le informó?"),
        ("clima", "Clima",
            "¿Cuánto duró la lluvia, qué tan fuerte fue, cuántas horas afectó y si se hicieron trabajos alternativos o se reprogramó?"),
        ("suministros", "Suministros o herramientas",
            "¿Qué falta (y cuánto, si se sabe), para qué actividad y desde cuándo?"),
        ("pago", "Pago o anticipo pendiente",
            "¿Qué pago o anticipo está pendiente, de qué proveedor o compra, y qué está frenando?"),
        ("otro", "Otra novedad", "Cuéntame qué pasó y desde cuándo.")
    ];

    private async Task IniciarBloqueoAsync(ContactoHelvis contacto, Proyecto proyecto, string saludo, CancellationToken ct)
    {
        await CancelarConversacionesAsync(contacto, ct);
        var datos = new DatosBloqueo { ProyectoId = proyecto.Id };
        var abiertos = await _db.Restricciones.CountAsync(r => r.ProyectoId == proyecto.Id &&
            (r.Estado == EstadoRestriccion.Abierta || r.Estado == EstadoRestriccion.EnGestion), ct);

        if (abiertos > 0)
        {
            await GuardarFlujoAsync(contacto, "bloqueo", "inicio", datos, ct);
            await EnviarBotonesYGuardarAsync(contacto, contacto.Telefono,
                saludo + $"🚧 {proyecto.Codigo} tiene *{abiertos}* novedad(es) o bloqueo(s) abiertos. ¿Qué quieres hacer?",
                [("blq_nuevo", "🚧 Reportar nuevo"), ("blq_resuelto", "✅ Ya se resolvió"), ("menu", "🏠 Menú")], ct);
            return;
        }

        await GuardarFlujoAsync(contacto, "bloqueo", "tipo", datos, ct);
        await PreguntarTipoBloqueoAsync(contacto, saludo, ct);
    }

    private async Task PreguntarTipoBloqueoAsync(ContactoHelvis contacto, string saludo, CancellationToken ct)
    {
        var cuerpo = saludo + "🚧 ¿Qué tipo de novedad o bloqueo es?";
        var ok = await _wa.EnviarListaAsync(contacto.Telefono, cuerpo, "Elegir tipo",
            TiposBloqueo.Select(t => ($"blq_t_{t.Id}", t.Titulo, (string?)null)), ct);
        await GuardarSalienteAsync(contacto, contacto.Telefono,
            cuerpo + "\n" + string.Join("\n", TiposBloqueo.Select((t, i) => $"{i + 1}. {t.Titulo}")), "interactive", ok, ct);
    }

    private async Task ContinuarBloqueoAsync(ContactoHelvis contacto, MensajeWhatsApp mensaje, CancellationToken ct)
    {
        var datos = JsonSerializer.Deserialize<DatosBloqueo>(contacto.FlujoDatos ?? "{}") ?? new DatosBloqueo();
        var proyecto = await _db.Proyectos.FirstOrDefaultAsync(p => p.Id == datos.ProyectoId, ct);
        if (proyecto is null) { await CancelarConversacionesAsync(contacto, ct); return; }

        var op = mensaje.OpcionId;
        var texto = (mensaje.Texto ?? "").Trim();
        var t = ReporteTexto.Normalizar(texto);
        var numero = contacto.Telefono;

        switch (contacto.FlujoPaso)
        {
            case "inicio":
                if (op == "blq_resuelto" || t.Contains("resolv") || t.Contains("solucion"))
                {
                    await GuardarFlujoAsync(contacto, "bloqueo", "resuelto", datos, ct);
                    await ListarBloqueosAbiertosAsync(contacto, proyecto, ct);
                }
                else
                {
                    await GuardarFlujoAsync(contacto, "bloqueo", "tipo", datos, ct);
                    await PreguntarTipoBloqueoAsync(contacto, "", ct);
                }
                return;

            case "tipo":
            {
                var tipo = TipoDesdeRespuesta(op, t);
                if (tipo is null)
                {
                    await EnviarTextoAsync(contacto, numero, "Elige el tipo en la lista o responde con su número (1 a 5).", ct);
                    return;
                }
                datos.Tipo = tipo.Value.Titulo;
                await GuardarFlujoAsync(contacto, "bloqueo", "descripcion", datos, ct);
                await EnviarTextoAsync(contacto, numero, $"📝 *{tipo.Value.Titulo}*: {tipo.Value.Pregunta}\n\n_Puedes contarlo todo en un solo mensaje._", ct);
                return;
            }

            case "descripcion":
                if (t.Length < 4 || op is not null)
                {
                    await EnviarTextoAsync(contacto, numero, "Cuéntame en un mensaje qué pasó (o *cancelar*).", ct);
                    return;
                }
                datos.Descripcion = Recortar(texto, 1500);
                await GuardarFlujoAsync(contacto, "bloqueo", "impacto", datos, ct);
                await EnviarBotonesYGuardarAsync(contacto, numero, "¿Cómo afecta el trabajo?",
                    [("blq_i_detiene", "⛔ Detiene el avance"), ("blq_i_limita", "⚠️ Avanza con límites"), ("blq_i_no", "✅ No afecta")], ct);
                return;

            case "impacto":
                datos.Impacto = op switch
                {
                    "blq_i_detiene" => "Detiene el avance",
                    "blq_i_limita" => "Permite avanzar con limitaciones",
                    "blq_i_no" => "No afecta el avance",
                    _ => t.Contains("detien") || t.Contains("par") ? "Detiene el avance"
                       : t.Contains("limit") || t.Contains("lento") ? "Permite avanzar con limitaciones"
                       : t.Contains("no afecta") || t is "no" ? "No afecta el avance" : null
                };
                if (datos.Impacto is null)
                {
                    await EnviarTextoAsync(contacto, numero, "Toca una de las opciones: detiene, avanza con límites o no afecta.", ct);
                    return;
                }
                var (deLaPersona, sonAsignadas) = await ActividadesParaReportarAsync(contacto, datos.ProyectoId, ct);
                if (sonAsignadas && deLaPersona.Count > 0)
                {
                    await GuardarFlujoAsync(contacto, "bloqueo", "actividad", datos, ct);
                    var filas = deLaPersona.Take(9)
                        .Select(a => ($"act_{a.Id}", Recortar($"{a.CodigoWBS} {a.Nombre}", 24), (string?)Recortar(a.Nombre, 60)))
                        .Append(("act_0", "Ninguna en particular", (string?)null));
                    var cuerpo = "¿Qué actividad afecta?";
                    var ok = await _wa.EnviarListaAsync(numero, cuerpo, "Elegir actividad", filas, ct);
                    await GuardarSalienteAsync(contacto, numero, cuerpo, "interactive", ok, ct);
                    return;
                }
                await GuardarFlujoAsync(contacto, "bloqueo", "confirmar", datos, ct);
                await ConfirmarBloqueoAsync(contacto, proyecto, datos, ct);
                return;

            case "actividad":
            {
                var (asignadas, _) = await ActividadesParaReportarAsync(contacto, datos.ProyectoId, ct);
                if (op?.StartsWith("act_") == true && int.TryParse(op[4..], out var idAct))
                {
                    var a = asignadas.FirstOrDefault(x => x.Id == idAct);
                    datos.ActividadId = a?.Id;
                    datos.ActividadTexto = a is null ? null : $"{a.CodigoWBS} {a.Nombre}";
                }
                else if (int.TryParse(t, out var indice) && indice >= 1 && indice <= Math.Min(9, asignadas.Count))
                {
                    var a = asignadas[indice - 1];
                    datos.ActividadId = a.Id;
                    datos.ActividadTexto = $"{a.CodigoWBS} {a.Nombre}";
                }
                else
                {
                    var c = ReporteTexto.Coincidencias(texto, asignadas, a => a.Nombre);
                    if (c.Count == 1) { datos.ActividadId = c[0].Id; datos.ActividadTexto = $"{c[0].CodigoWBS} {c[0].Nombre}"; }
                }
                await GuardarFlujoAsync(contacto, "bloqueo", "confirmar", datos, ct);
                await ConfirmarBloqueoAsync(contacto, proyecto, datos, ct);
                return;
            }

            case "confirmar":
                if (op == "blq_ok" || t is "si" or "confirmo" or "ok" or "guardar")
                {
                    await GuardarBloqueoAsync(contacto, proyecto, datos, ct);
                    return;
                }
                if (op == "blq_cancelar" || t is "no" or "cancelar")
                {
                    await CancelarConversacionesAsync(contacto, ct);
                    await EnviarTextoAsync(contacto, numero, "Listo, no se registró la novedad.", ct);
                    return;
                }
                await ConfirmarBloqueoAsync(contacto, proyecto, datos, ct);
                return;

            case "resuelto":
            {
                var abiertos = await BloqueosAbiertosAsync(datos.ProyectoId, ct);
                Restriccion? elegido = null;
                if (op?.StartsWith("blq_r_") == true && int.TryParse(op[6..], out var idR))
                    elegido = abiertos.FirstOrDefault(r => r.Id == idR);
                else if (int.TryParse(t, out var indice) && indice >= 1 && indice <= Math.Min(10, abiertos.Count))
                    elegido = abiertos[indice - 1];
                if (elegido is null)
                {
                    await EnviarTextoAsync(contacto, numero, "Elige en la lista cuál se resolvió, o responde con su número.", ct);
                    return;
                }
                datos.RestriccionId = elegido.Id;
                await GuardarFlujoAsync(contacto, "bloqueo", "resuelto_ok", datos, ct);
                await EnviarBotonesYGuardarAsync(contacto, numero,
                    $"¿Confirmas que *{elegido.Numero}* quedó resuelto en obra (material recibido, máquina operando o trabajo reanudado)?\n\n" +
                    $"_{Recortar(elegido.Descripcion, 300)}_",
                    [("blq_rok", "✅ Sí, resuelto"), ("blq_cancelar", "❌ No todavía")], ct);
                return;
            }

            case "resuelto_ok":
                if (op == "blq_rok" || t is "si" or "confirmo")
                {
                    await CerrarBloqueoAsync(contacto, proyecto, datos, ct);
                    return;
                }
                await CancelarConversacionesAsync(contacto, ct);
                await EnviarTextoAsync(contacto, numero, "Listo, la novedad sigue abierta.", ct);
                return;
        }
    }

    private static (string Id, string Titulo, string Pregunta)? TipoDesdeRespuesta(string? op, string t)
    {
        if (op?.StartsWith("blq_t_") == true)
        {
            var id = op[6..];
            foreach (var tipo in TiposBloqueo) if (tipo.Id == id) return tipo;
        }
        if (int.TryParse(t, out var n) && n >= 1 && n <= TiposBloqueo.Length) return TiposBloqueo[n - 1];
        if (t.Contains("maquin") || t.Contains("varad") || t.Contains("equipo")) return TiposBloqueo[0];
        if (t.Contains("lluvi") || t.Contains("clima") || t.Contains("tormenta")) return TiposBloqueo[1];
        if (t.Contains("falta") || t.Contains("material") || t.Contains("suminis") || t.Contains("herramient")) return TiposBloqueo[2];
        if (t.Contains("pago") || t.Contains("anticip")) return TiposBloqueo[3];
        if (t.Contains("otr")) return TiposBloqueo[4];
        return null;
    }

    private async Task ConfirmarBloqueoAsync(ContactoHelvis contacto, Proyecto proyecto, DatosBloqueo d, CancellationToken ct)
    {
        var cuerpo = $"🚧 *Resumen de la novedad — {proyecto.Codigo}*\n\n" +
                     $"Tipo: {d.Tipo}\nQué pasó: {d.Descripcion}\nImpacto: {d.Impacto}\n" +
                     $"Actividad afectada: {d.ActividadTexto ?? "ninguna en particular"}\n\n¿La registro?";
        await EnviarBotonesYGuardarAsync(contacto, contacto.Telefono, Recortar(cuerpo, 1000),
            [("blq_ok", "✅ Registrar"), ("blq_cancelar", "❌ Cancelar")], ct);
    }

    private async Task GuardarBloqueoAsync(ContactoHelvis contacto, Proyecto proyecto, DatosBloqueo d, CancellationToken ct)
    {
        var numeros = await _db.Restricciones.Where(r => r.ProyectoId == proyecto.Id).Select(r => r.Numero).ToListAsync(ct);
        var siguiente = numeros
            .Select(n => int.TryParse(new string(n.Where(char.IsDigit).ToArray()), out var x) ? x : 0)
            .DefaultIfEmpty(0).Max() + 1;

        var restriccion = new Restriccion
        {
            ProyectoId = proyecto.Id,
            Numero = $"RES-{siguiente:D3}",
            Tipo = d.Tipo,
            Descripcion = Recortar((d.ActividadTexto is null ? "" : $"[{d.ActividadTexto}] ") + d.Descripcion, 2000),
            Impacto = d.Impacto,
            Estado = EstadoRestriccion.Abierta,
            FechaIdentificacion = SeguimientoDocumento.HoyColombia(),
            Plan = $"Reportado por {contacto.Nombre} por WhatsApp el {DateTime.UtcNow.AddHours(-5):dd/MM/yyyy HH:mm}."
        };
        _db.Restricciones.Add(restriccion);
        await _db.SaveChangesAsync(ct);
        await CancelarConversacionesAsync(contacto, ct);

        // Aviso automático a los responsables (Yesid, Javier) cuando detiene o limita el avance (§11.3).
        var avisados = 0;
        if (d.Impacto != "No afecta el avance")
        {
            var destinatarios = await _db.ContactosHelvis
                .Where(c => c.RecibeBloqueos && c.Activo && c.Id != contacto.Id)
                .ToListAsync(ct);
            foreach (var r in destinatarios.Where(PuedeRecibirAvisos))
            {
                await EnviarTextoAsync(r, r.Telefono,
                    Recortar($"🚧 *Novedad {restriccion.Numero} — {proyecto.Codigo}*\n\n" +
                             $"Tipo: {d.Tipo}\nImpacto: *{d.Impacto}*\nActividad: {d.ActividadTexto ?? "—"}\n" +
                             $"Qué pasó: {d.Descripcion}\n\nReportó: {contacto.Nombre}", 4000), ct);
                avisados++;
            }
        }

        await EnviarTextoAsync(contacto, contacto.Telefono,
            $"✅ Registré la novedad *{restriccion.Numero}* en {proyecto.Codigo}." +
            (avisados > 0 ? $" Avisé a {avisados} responsable(s)." : "") +
            "\n\nCuando se resuelva en obra, escribe *bloqueo* y toca *Ya se resolvió*.", ct);
    }

    private Task<List<Restriccion>> BloqueosAbiertosAsync(int proyectoId, CancellationToken ct) =>
        _db.Restricciones
            .Where(r => r.ProyectoId == proyectoId &&
                        (r.Estado == EstadoRestriccion.Abierta || r.Estado == EstadoRestriccion.EnGestion))
            .OrderBy(r => r.FechaIdentificacion).ThenBy(r => r.Id)
            .ToListAsync(ct);

    private async Task ListarBloqueosAbiertosAsync(ContactoHelvis contacto, Proyecto proyecto, CancellationToken ct)
    {
        var abiertos = await BloqueosAbiertosAsync(proyecto.Id, ct);
        var cuerpo = $"¿Cuál se resolvió en {proyecto.Codigo}?";
        var filas = abiertos.Take(10).Select(r => ($"blq_r_{r.Id}", Recortar($"{r.Numero} {r.Tipo}", 24), (string?)Recortar(r.Descripcion, 70)));
        var ok = await _wa.EnviarListaAsync(contacto.Telefono, cuerpo, "Elegir novedad", filas, ct);
        await GuardarSalienteAsync(contacto, contacto.Telefono,
            cuerpo + "\n" + string.Join("\n", abiertos.Take(10).Select((r, i) => $"{i + 1}. {r.Numero} {Recortar(r.Descripcion, 50)}")),
            "interactive", ok, ct);
    }

    /// <summary>Cierre por obra (§11.6): la confirmación final de solución la da quien está en obra.</summary>
    private async Task CerrarBloqueoAsync(ContactoHelvis contacto, Proyecto proyecto, DatosBloqueo d, CancellationToken ct)
    {
        var r = await _db.Restricciones.FirstOrDefaultAsync(x => x.Id == d.RestriccionId, ct);
        await CancelarConversacionesAsync(contacto, ct);
        if (r is null) return;

        r.Estado = EstadoRestriccion.Levantada;
        r.FechaLevantamiento = SeguimientoDocumento.HoyColombia();
        r.Plan = Recortar($"{r.Plan}\nResuelto según obra: {contacto.Nombre}, {DateTime.UtcNow.AddHours(-5):dd/MM/yyyy HH:mm} (WhatsApp).".Trim(), 2000);
        r.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        var destinatarios = await _db.ContactosHelvis.Where(c => c.RecibeBloqueos && c.Activo && c.Id != contacto.Id).ToListAsync(ct);
        foreach (var dest in destinatarios.Where(PuedeRecibirAvisos))
            await EnviarTextoAsync(dest, dest.Telefono,
                $"✅ *{r.Numero} — {proyecto.Codigo}* resuelto según obra ({contacto.Nombre}).\n_{Recortar(r.Descripcion, 300)}_", ct);

        await EnviarTextoAsync(contacto, contacto.Telefono, $"✅ Cerré *{r.Numero}*. ¡Gracias por confirmarlo!", ct);
    }

    private async Task GuardarFlujoAsync(ContactoHelvis contacto, string flujo, string paso, DatosBloqueo datos, CancellationToken ct)
    {
        contacto.Flujo = flujo;
        contacto.FlujoPaso = paso;
        contacto.FlujoDatos = JsonSerializer.Serialize(datos);
        contacto.FlujoFecha = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }
}
