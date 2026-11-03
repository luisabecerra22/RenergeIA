using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Entities.Helvis;
using RenergeIA.Core.Enums;
using RenergeIA.Core.Helpers;

namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Reporte de avance por WhatsApp (documento §4, §7 y §8).
/// Reglas: se pregunta UNA cosa a la vez y solo lo que falta; nada se guarda hasta que
/// la persona confirma el resumen; al confirmar se escribe una sola vez en el Informe
/// diario del día (mismo cálculo que la app) y el avance baja solo con motivo.
/// Sin cantidad total en el cronograma, la cantidad se conserva y el % queda pendiente:
/// nunca se inventa un total ni se convierte lo desconocido en 0 %.
/// </summary>
public partial class WhatsAppConversacion
{
    /// <summary>Un borrador sin tocar en este tiempo se considera abandonado.</summary>
    private static readonly TimeSpan VigenciaBorrador = TimeSpan.FromHours(12);

    /// <summary>Hasta esta hora (Colombia) se puede reportar el día anterior (§7.3).</summary>
    private const int HoraLimiteDiaAnterior = 10;

    private Task<ReporteAvanceHelvis?> BorradorActualAsync(ContactoHelvis contacto, CancellationToken ct)
    {
        var limite = DateTime.UtcNow - VigenciaBorrador;
        return _db.ReportesAvanceHelvis
            .Where(r => r.ContactoHelvisId == contacto.Id && r.Estado == EstadosReporteHelvis.Borrador &&
                        (r.FechaModificacion ?? r.FechaCreacion) >= limite)
            .OrderByDescending(r => r.Id)
            .FirstOrDefaultAsync(ct);
    }

    // ==================== Inicio ====================

    private async Task IniciarReporteAsync(ContactoHelvis contacto, Proyecto proyecto, string? texto,
        string saludo, CancellationToken ct)
    {
        await CancelarConversacionesAsync(contacto, ct);

        var lectura = texto is null ? null : ReporteTexto.Leer(texto);
        var hoy = SeguimientoDocumento.HoyColombia();
        var fecha = hoy;
        if (lectura?.EsDeAyer == true)
        {
            if (DateTime.UtcNow.AddHours(-5).Hour >= HoraLimiteDiaAnterior)
            {
                await EnviarTextoAsync(contacto, contacto.Telefono,
                    saludo + $"⏰ El reporte del día anterior se puede completar hasta las {HoraLimiteDiaAnterior}:00 a. m. " +
                    "Después de esa hora lo registra Yesid o Luisa en la app.\n\n" +
                    "Si el avance es de hoy, escríbelo sin la palabra «ayer».", ct);
                return;
            }
            fecha = hoy.AddDays(-1);
        }

        var informe = await InformeDelDiaAsync(proyecto.Id, fecha, ct);
        if (informe?.Estado == EstadoInforme.Aprobado)
        {
            await EnviarTextoAsync(contacto, contacto.Telefono,
                saludo + $"🔒 El informe diario del {fecha:dd/MM/yyyy} de {proyecto.Codigo} ya está *aprobado*. " +
                "Un cambio sobre un informe aprobado lo hace el aprobador en la app (queda como nueva revisión).", ct);
            return;
        }

        var borrador = new ReporteAvanceHelvis
        {
            ContactoHelvisId = contacto.Id,
            ProyectoId = proyecto.Id,
            FechaTrabajo = fecha,
            TextoOriginal = texto is null ? null : Recortar(texto, 2000),
            Cantidad = lectura?.Cantidad,
            Porcentaje = lectura?.Porcentaje,
            CantidadEsAcumulada = lectura?.EsAcumulado
        };

        // Si el mensaje ya nombra la actividad ("hincamos…"), se reconoce sin preguntar.
        var (disponibles, _) = await ActividadesParaReportarAsync(contacto, proyecto.Id, ct);
        if (texto is not null)
        {
            var coincidencias = ReporteTexto.Coincidencias(texto, disponibles, a => a.Nombre);
            if (coincidencias.Count == 1) borrador.ActividadWBSId = coincidencias[0].Id;
            else if (coincidencias.Count > 1) borrador.OpcionesActividad = string.Join(",", coincidencias.Take(10).Select(a => a.Id));
        }
        if (borrador.ActividadWBSId is null && disponibles.Count == 1)
            borrador.ActividadWBSId = disponibles[0].Id;

        _db.ReportesAvanceHelvis.Add(borrador);
        await _db.SaveChangesAsync(ct);

        // Fotos enviadas justo antes de arrancar el reporte se le adjuntan.
        var desde = DateTime.UtcNow.AddHours(-2);
        var sueltas = await _db.FotosHelvis
            .Where(f => f.ContactoHelvisId == contacto.Id && f.ReporteAvanceHelvisId == null &&
                        f.InformeDiarioId == null && f.FechaCreacion >= desde)
            .ToListAsync(ct);
        foreach (var f in sueltas) { f.ReporteAvanceHelvisId = borrador.Id; f.ProyectoId = proyecto.Id; }
        if (sueltas.Count > 0) await _db.SaveChangesAsync(ct);

        await AvanzarReporteAsync(contacto, borrador, saludo, ct);
    }

    // ==================== Continuación ====================

    /// <summary>Atiende la respuesta mientras hay un reporte en curso. Devuelve false si no había ninguno.</summary>
    private async Task<bool> ContinuarReporteAsync(ContactoHelvis contacto, MensajeWhatsApp mensaje, CancellationToken ct)
    {
        var opcionId = mensaje.OpcionId;
        var t = ReporteTexto.Normalizar(mensaje.Texto);

        // Botones que se ofrecen DESPUÉS de guardar (ya no hay borrador abierto).
        if (opcionId is "rep_otra" or "rep_fin" || t is "termine" or "eso es todo" or "termine por hoy")
        {
            var ultimo = await _db.ReportesAvanceHelvis
                .Where(r => r.ContactoHelvisId == contacto.Id && r.Estado == EstadosReporteHelvis.Confirmado)
                .OrderByDescending(r => r.FechaConfirmacion).FirstOrDefaultAsync(ct);
            var proyectoUltimo = ultimo is null ? null : await _db.Proyectos.FirstOrDefaultAsync(p => p.Id == ultimo.ProyectoId, ct);
            if (ultimo is null || proyectoUltimo is null) return false;

            if (opcionId == "rep_otra")
                await IniciarReporteAsync(contacto, proyectoUltimo, null, "", ct);
            else
                await TerminarAporteAsync(contacto, proyectoUltimo, ultimo.FechaTrabajo, ct);
            return true;
        }

        var b = await BorradorActualAsync(contacto, ct);
        if (b is null) return false;

        b.FechaModificacion = DateTime.UtcNow;
        var texto = mensaje.Texto ?? "";

        switch (b.Paso)
        {
            case "actividad":
            {
                var (disponibles, _) = await ActividadesParaReportarAsync(contacto, b.ProyectoId, ct);
                var ofrecidas = IdsOfrecidos(b, disponibles);
                int? elegida = null;

                if (opcionId?.StartsWith("act_") == true && int.TryParse(opcionId[4..], out var idBoton) &&
                    disponibles.Any(a => a.Id == idBoton))
                    elegida = idBoton;
                else if (int.TryParse(t, out var indice) && indice >= 1 && indice <= ofrecidas.Count)
                    elegida = ofrecidas[indice - 1];
                else
                {
                    var coincidencias = ReporteTexto.Coincidencias(texto, disponibles, a => a.Nombre);
                    if (coincidencias.Count == 1) elegida = coincidencias[0].Id;
                    else if (coincidencias.Count > 1)
                        b.OpcionesActividad = string.Join(",", coincidencias.Take(10).Select(a => a.Id));
                    else
                        await EnviarTextoAsync(contacto, contacto.Telefono,
                            "No encontré esa actividad entre las disponibles. Responde con el *número* de la lista " +
                            "o con una palabra del nombre (ej. _hincado_).", ct);
                }

                if (elegida is int id)
                {
                    b.ActividadWBSId = id;
                    b.OpcionesActividad = null;
                    // Si en el mismo mensaje vino la cantidad ("hincado 80 hoy"), se aprovecha.
                    if (b.Cantidad is null && b.Porcentaje is null && opcionId is null)
                    {
                        var l = ReporteTexto.Leer(texto);
                        if (!int.TryParse(t, out _)) { b.Cantidad = l.Cantidad; b.Porcentaje = l.Porcentaje; b.CantidadEsAcumulada = l.EsAcumulado; }
                    }
                }
                break;
            }

            case "valor":
            {
                var l = ReporteTexto.Leer(texto);
                if (l.Cantidad is null && l.Porcentaje is null)
                {
                    await EnviarTextoAsync(contacto, contacto.Telefono,
                        "No encontré un número en el mensaje. Escribe la cantidad o el porcentaje, " +
                        "por ejemplo _hoy 80_, _llevamos 400_ o _35%_.", ct);
                    await _db.SaveChangesAsync(ct);
                    return true;
                }
                b.Cantidad = l.Cantidad;
                b.Porcentaje = l.Porcentaje;
                b.CantidadEsAcumulada = l.EsAcumulado;
                b.Motivo = null;
                b.SumarAlDeHoy = null;
                break;
            }

            case "tipo":
                if (opcionId == "rep_dia" || t.Contains("dia") || t.Contains("hoy")) b.CantidadEsAcumulada = false;
                else if (opcionId == "rep_acum" || t.Contains("acum") || t.Contains("total") || t.Contains("llevamos"))
                    b.CantidadEsAcumulada = true;
                break;

            case "duplicado":
                if (opcionId == "rep_sumar" || t.Contains("adicional") || t.Contains("sumar")) b.SumarAlDeHoy = true;
                else if (opcionId == "rep_reemplazar" || t.Contains("reemplaz") || t.Contains("correg")) b.SumarAlDeHoy = false;
                break;

            case "motivo":
                if (t.Length >= 4 && opcionId is null) b.Motivo = Recortar(texto.Trim(), 1000);
                else
                {
                    await EnviarTextoAsync(contacto, contacto.Telefono,
                        "Para bajar el avance necesito el motivo en un mensaje (ej. _se había reportado de más_).", ct);
                    await _db.SaveChangesAsync(ct);
                    return true;
                }
                break;

            case "recursos":
                if (opcionId == "rep_sinrec" || t is "no" or "omitir" or "ninguno" or "no aplica")
                    b.RecursosRespondidos = true;
                else
                {
                    var (personas, horas) = ReporteTexto.LeerRecursos(texto);
                    if (personas is null && horas is null)
                    {
                        await EnviarTextoAsync(contacto, contacto.Telefono,
                            "No entendí los recursos. Escribe por ejemplo _8 personas 9 horas_, o toca *Omitir*.", ct);
                        await _db.SaveChangesAsync(ct);
                        return true;
                    }
                    b.Personas = personas;
                    b.Horas = horas;
                    b.RecursosRespondidos = true;
                }
                break;

            case "fotos":
                if (opcionId == "rep_seguir" || t is "continuar" or "no" or "listo" or "sin fotos" or "ninguna" or "seguir")
                    b.FotosRespondidas = true;
                else if (t.Length > 0)
                {
                    // Un texto en este paso se toma como observación del reporte.
                    b.Observaciones = Recortar(texto.Trim(), 2000);
                    b.FotosRespondidas = true;
                }
                break;

            case "confirmar":
                if (opcionId == "rep_ok" || t is "si" or "confirmo" or "ok" or "guardar" or "confirmar" or "si, guardar")
                {
                    await _db.SaveChangesAsync(ct);
                    await GuardarReporteAsync(contacto, b, ct);
                    return true;
                }
                if (opcionId == "rep_cambiar" || t.StartsWith("cambi") || t.StartsWith("correg"))
                {
                    b.Cantidad = null; b.Porcentaje = null; b.CantidadEsAcumulada = null;
                    b.Motivo = null; b.SumarAlDeHoy = null;
                    break;
                }
                if (opcionId == "rep_cancelar" || t is "no" or "cancelar")
                {
                    b.Estado = EstadosReporteHelvis.Cancelado;
                    await _db.SaveChangesAsync(ct);
                    await EnviarTextoAsync(contacto, contacto.Telefono,
                        "Reporte cancelado; no se guardó nada. Escribe *menú* para ver las opciones.", ct);
                    return true;
                }
                break;
        }

        await _db.SaveChangesAsync(ct);
        await AvanzarReporteAsync(contacto, b, "", ct);
        return true;
    }

    // ==================== Siguiente pregunta ====================

    /// <summary>Pregunta el siguiente dato que falta, o muestra el resumen para confirmar.</summary>
    private async Task AvanzarReporteAsync(ContactoHelvis contacto, ReporteAvanceHelvis b, string saludo, CancellationToken ct)
    {
        var numero = contacto.Telefono;
        var act = b.ActividadWBSId is int idAct
            ? await _db.ActividadesWBS.FirstOrDefaultAsync(a => a.Id == idAct, ct)
            : null;

        if (act is null)
        {
            await PreguntarActividadAsync(contacto, b, saludo, ct);
            return;
        }

        var unidad = string.IsNullOrWhiteSpace(act.Unidad) ? "unidades" : act.Unidad;
        var nombreAct = $"*{act.CodigoWBS} {Recortar(act.Nombre, 60)}*";

        if (b.Cantidad is null && b.Porcentaje is null)
        {
            await CambiarPasoAsync(b, "valor", ct);
            var total = act.CantidadTotal > 0 ? $" (total del cronograma: {act.CantidadTotal:N0} {unidad})" : "";
            await EnviarTextoAsync(contacto, numero,
                saludo + $"✍️ Reporte de {nombreAct}{total}\nVa en *{act.AvanceReal:0.#}%*.\n\n" +
                "¿Cuánto avanzó? Puede ser una cantidad o un porcentaje, por ejemplo _hoy 80_, " +
                "_llevamos 400_ o _35%_.", ct);
            return;
        }

        if (b.Porcentaje is null && b.CantidadEsAcumulada is null)
        {
            await CambiarPasoAsync(b, "tipo", ct);
            await EnviarBotonesYGuardarAsync(contacto, numero,
                saludo + $"Para {nombreAct}: los *{b.Cantidad:0.##} {unidad}*, ¿son de hoy o el acumulado total?",
                [("rep_dia", "📅 Son de hoy"), ("rep_acum", "📈 Acumulado total")], ct);
            return;
        }

        var registroHoy = await RegistroDelDiaAsync(b.ProyectoId, act.Id, b.FechaTrabajo, ct);
        if (b.Porcentaje is null && b.CantidadEsAcumulada == false && registroHoy is { CantidadEjecutadaDia: > 0 } &&
            b.SumarAlDeHoy is null)
        {
            await CambiarPasoAsync(b, "duplicado", ct);
            await EnviarBotonesYGuardarAsync(contacto, numero,
                $"Ya hay un reporte del {b.FechaTrabajo:dd/MM} para {nombreAct}: " +
                $"*{registroHoy.CantidadEjecutadaDia:0.##} {unidad}* ({registroHoy.ReportadoPor}).\n\n" +
                $"Los {b.Cantidad:0.##} {unidad} que reportas, ¿son trabajo adicional o corrigen ese dato?",
                [("rep_sumar", "➕ Es adicional"), ("rep_reemplazar", "🔁 Corrige el dato")], ct);
            return;
        }

        var calc = Calcular(b, act, registroHoy);
        if (calc.Error is not null)
        {
            b.Cantidad = null; b.Porcentaje = null; b.CantidadEsAcumulada = null; b.SumarAlDeHoy = null;
            await CambiarPasoAsync(b, "valor", ct);
            await EnviarTextoAsync(contacto, numero, $"⚠️ {calc.Error}\n\nEscribe de nuevo el avance de {nombreAct}.", ct);
            return;
        }
        b.PorcentajeAnterior = act.AvanceReal;
        b.PorcentajeNuevo = calc.Porcentaje;
        b.CantidadDia = calc.CantidadDia;

        if (b.PorcentajeNuevo is decimal nuevo && nuevo < act.AvanceReal && string.IsNullOrWhiteSpace(b.Motivo))
        {
            await CambiarPasoAsync(b, "motivo", ct);
            await EnviarTextoAsync(contacto, numero,
                $"⚠️ {nombreAct} tenía *{act.AvanceReal:0.#}%* y con este dato quedaría en *{nuevo:0.#}%*.\n\n" +
                "¿Es una corrección? Escribe el *motivo* (o *cancelar*).", ct);
            return;
        }

        if (!b.RecursosRespondidos)
        {
            await CambiarPasoAsync(b, "recursos", ct);
            // Se repite lo entendido para que la persona lo verifique antes de seguir.
            var entendido = b.PorcentajeNuevo is decimal pn
                ? $"*{act.AvanceReal:0.#}% → {pn:0.#}%*"
                : $"sigue en *{act.AvanceReal:0.#}%* (% pendiente: sin cantidad total en el cronograma)";
            var cantidadTxt = b.CantidadDia is decimal cd ? $"{cd:0.##} {unidad} en el día · " : "";
            await EnviarBotonesYGuardarAsync(contacto, numero,
                saludo + $"✍️ Anoto en {nombreAct}: {cantidadTxt}{entendido}\n\n" +
                "👷 ¿Cuántas personas y cuántas horas trabajaron?\n" +
                "Ej.: _8 personas 9 horas_. No hacen falta nombres.",
                [("rep_sinrec", "Omitir")], ct);
            return;
        }

        if (!b.FotosRespondidas)
        {
            await CambiarPasoAsync(b, "fotos", ct);
            var fotos = await _db.FotosHelvis.CountAsync(f => f.ReporteAvanceHelvisId == b.Id, ct);
            var yaTiene = fotos > 0 ? $" (ya tengo {fotos})" : "";
            await EnviarBotonesYGuardarAsync(contacto, numero,
                $"📷 ¿Tienes fotos de la actividad{yaTiene}? Envíalas ahora (son opcionales) y luego toca *Continuar*.\n" +
                "_Si quieres dejar una observación, escríbela aquí._",
                [("rep_seguir", "Continuar")], ct);
            return;
        }

        await CambiarPasoAsync(b, "confirmar", ct);
        var resumen = await ResumenBorradorAsync(b, act, ct);
        await EnviarBotonesYGuardarAsync(contacto, numero, resumen,
            [("rep_ok", "✅ Confirmar"), ("rep_cambiar", "✏️ Cambiar valor"), ("rep_cancelar", "❌ Cancelar")], ct);
    }

    private async Task PreguntarActividadAsync(ContactoHelvis contacto, ReporteAvanceHelvis b, string saludo, CancellationToken ct)
    {
        await CambiarPasoAsync(b, "actividad", ct);
        var (disponibles, asignadas) = await ActividadesParaReportarAsync(contacto, b.ProyectoId, ct);

        if (disponibles.Count == 0)
        {
            b.Estado = EstadosReporteHelvis.Cancelado;
            await _db.SaveChangesAsync(ct);
            await EnviarTextoAsync(contacto, contacto.Telefono,
                saludo + "Este proyecto aún no tiene actividades en el cronograma vigente, así que no hay dónde registrar el avance.", ct);
            return;
        }

        var ofrecidas = IdsOfrecidos(b, disponibles);
        var lista = ofrecidas.Select(id => disponibles.First(a => a.Id == id)).ToList();

        // Muchas actividades y ninguna asignada: mejor que escriba el nombre.
        if (lista.Count == 0 || (!asignadas && b.OpcionesActividad is null && disponibles.Count > 10))
        {
            await EnviarTextoAsync(contacto, contacto.Telefono,
                saludo + "✍️ ¿De qué actividad es el reporte? Escribe una palabra del nombre (ej. _hincado_, _zanja_, _módulos_).\n\n" +
                "_Si se asignan actividades a este número en el panel de Helvis, aparecerán en una lista._", ct);
            return;
        }

        var cuerpo = saludo + (b.OpcionesActividad is null
            ? "✍️ ¿De qué actividad es el reporte?"
            : "Encontré varias actividades con ese nombre. ¿Cuál es?");

        if (lista.Count <= 10)
        {
            var filas = lista.Select(a => ($"act_{a.Id}", Recortar($"{a.CodigoWBS} {a.Nombre}", 24),
                (string?)$"{Recortar(a.Nombre, 50)} · va en {a.AvanceReal:0.#}%"));
            var ok = await _wa.EnviarListaAsync(contacto.Telefono, cuerpo, "Elegir actividad", filas, ct);
            await GuardarSalienteAsync(contacto, contacto.Telefono, cuerpo + "\n" + string.Join("\n", lista.Select(a => $"• {a.CodigoWBS} {a.Nombre}")), "interactive", ok, ct);
        }
    }

    /// <summary>Las actividades ofrecidas: las de la última lista de coincidencias o las primeras 10 disponibles.</summary>
    private static List<int> IdsOfrecidos(ReporteAvanceHelvis b, List<ActividadWBS> disponibles)
    {
        if (!string.IsNullOrWhiteSpace(b.OpcionesActividad))
            return b.OpcionesActividad.Split(',').Select(int.Parse).Where(id => disponibles.Any(a => a.Id == id)).ToList();
        return disponibles.Take(10).Select(a => a.Id).ToList();
    }

    /// <summary>
    /// Actividades que la persona puede reportar en el proyecto: las asignadas en el panel
    /// (cronograma vigente); si no tiene asignaciones, las actividades hoja del cronograma.
    /// Nunca padres: el avance de los padres se calcula de sus hijas (§6).
    /// </summary>
    private async Task<(List<ActividadWBS> Lista, bool Asignadas)> ActividadesParaReportarAsync(
        ContactoHelvis contacto, int proyectoId, CancellationToken ct)
    {
        var asignadas = await _db.AsignacionesHelvis
            .Where(a => a.ContactoHelvisId == contacto.Id && a.Activa && a.Actividad.ProyectoId == proyectoId &&
                        a.Actividad.CronogramaVersion != null && a.Actividad.CronogramaVersion.EsVigente)
            .Select(a => a.Actividad)
            .ToListAsync(ct);
        if (asignadas.Count > 0) return (OrdenarWbs(asignadas), true);

        var hojas = await _informes.ActividadesHojaVigentesAsync(proyectoId);
        return (OrdenarWbs(hojas), false);
    }

    private static List<ActividadWBS> OrdenarWbs(IEnumerable<ActividadWBS> actividades) =>
        actividades.OrderBy(a => string.Join(".", a.CodigoWBS.Split('.').Select(p => p.PadLeft(4, '0')))).ToList();

    // ==================== Cálculo ====================

    internal record ResultadoCalculo(decimal? Porcentaje, decimal? CantidadDia, decimal? CantidadAcumulada, string? Error);

    /// <summary>
    /// Convierte lo reportado en el avance acumulado de la actividad.
    /// Porcentaje: es el acumulado. Cantidad del día: se suma al acumulado hasta el día
    /// anterior (o al de hoy si es trabajo adicional). Cantidad acumulada: es el total.
    /// Sin cantidad total en el cronograma: % = null (pendiente), nunca 0.
    /// </summary>
    internal static ResultadoCalculo Calcular(ReporteAvanceHelvis b, ActividadWBS act, RegistroAvanceDiario? registroHoy)
    {
        var unidad = string.IsNullOrWhiteSpace(act.Unidad) ? "unidades" : act.Unidad;

        if (b.Porcentaje is decimal p)
        {
            if (p > 100) return new(null, null, null, "El avance máximo es 100 %.");
            if (p < 0) return new(null, null, null, "El porcentaje no puede ser negativo.");
            return new(p, null, null, null);
        }

        if (b.Cantidad is not decimal cantidad) return new(null, null, null, "Falta la cantidad.");
        if (cantidad < 0) return new(null, null, null, "La cantidad no puede ser negativa.");

        var total = act.CantidadTotal;
        var hoyPrevio = registroHoy?.CantidadEjecutadaDia ?? 0;
        var acumuladoActual = act.CantidadEjecutadaAcumulada;
        // Si la actividad tiene % pero nunca se le registraron cantidades, se estima el acumulado desde el %.
        if (acumuladoActual == 0 && total > 0 && act.AvanceReal > 0)
            acumuladoActual = Math.Round(act.AvanceReal / 100m * total, 2);
        var hastaAyer = Math.Max(0, acumuladoActual - hoyPrevio);

        decimal dia, acumulado;
        if (b.CantidadEsAcumulada == true)
        {
            acumulado = cantidad;
            dia = cantidad - hastaAyer;
        }
        else
        {
            dia = b.SumarAlDeHoy == true ? hoyPrevio + cantidad : cantidad;
            acumulado = hastaAyer + dia;
        }

        if (total > 0 && acumulado > total)
            return new(null, null, null,
                $"El acumulado quedaría en {acumulado:N0} {unidad} y el total del cronograma es {total:N0} {unidad}. Revisa el dato.");

        decimal? porcentaje = total > 0 ? Math.Min(100m, Math.Round(acumulado / total * 100m, 1)) : null;
        return new(porcentaje, dia, acumulado, null);
    }

    private async Task<string> ResumenBorradorAsync(ReporteAvanceHelvis b, ActividadWBS act, CancellationToken ct)
    {
        var unidad = string.IsNullOrWhiteSpace(act.Unidad) ? "unidades" : act.Unidad;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📝 *Resumen del reporte — {b.FechaTrabajo:dd/MM/yyyy}*");
        sb.AppendLine();
        sb.AppendLine($"*{act.CodigoWBS} {Recortar(act.Nombre, 70)}*");
        if (b.CantidadDia is decimal dia)
            sb.AppendLine($"Cantidad del día: *{dia:0.##} {unidad}*");
        if (b.PorcentajeNuevo is decimal nuevo)
            sb.AppendLine($"Avance: {act.AvanceReal:0.#}% → *{nuevo:0.#}%*");
        else
            sb.AppendLine($"Avance: sigue en {act.AvanceReal:0.#}% — _la actividad aún no tiene cantidad total en el " +
                          "cronograma; la cantidad queda guardada y el % se calcula cuando se cargue el total._");
        if (!string.IsNullOrWhiteSpace(b.Motivo)) sb.AppendLine($"Motivo de la corrección: {b.Motivo}");
        if (b.Personas is not null || b.Horas is not null)
        {
            var hp = b.Personas is int pe && b.Horas is decimal ho ? $" ({pe * ho:0.#} horas-persona)" : "";
            sb.AppendLine($"Personal: {(b.Personas is null ? "—" : $"{b.Personas} personas")} · " +
                          $"{(b.Horas is null ? "—" : $"{b.Horas:0.#} h")}{hp}");
        }
        var fotos = await _db.FotosHelvis.CountAsync(f => f.ReporteAvanceHelvisId == b.Id, ct);
        if (fotos > 0) sb.AppendLine($"Fotos: {fotos}");
        if (!string.IsNullOrWhiteSpace(b.Observaciones)) sb.AppendLine($"Observación: {b.Observaciones}");
        sb.AppendLine();
        sb.Append("¿Lo guardo en el informe diario?");
        return sb.ToString();
    }

    // ==================== Guardar ====================

    /// <summary>
    /// Escribe el reporte confirmado en el Informe diario del día (lo crea si no existe),
    /// con el mismo cálculo de la app, y actualiza el avance de la actividad y de sus padres.
    /// Se revalida contra el cronograma actual, por si cambió mientras se conversaba (§15).
    /// </summary>
    private async Task GuardarReporteAsync(ContactoHelvis contacto, ReporteAvanceHelvis b, CancellationToken ct)
    {
        var numero = contacto.Telefono;
        var act = await _db.ActividadesWBS.FirstOrDefaultAsync(a => a.Id == b.ActividadWBSId, ct);
        if (act is null)
        {
            b.Estado = EstadosReporteHelvis.Cancelado;
            await _db.SaveChangesAsync(ct);
            await EnviarTextoAsync(contacto, numero, "La actividad ya no existe en el cronograma; el reporte no se guardó.", ct);
            return;
        }

        var informe = await InformeDelDiaAsync(b.ProyectoId, b.FechaTrabajo, ct);
        if (informe?.Estado == EstadoInforme.Aprobado)
        {
            b.Estado = EstadosReporteHelvis.Cancelado;
            await _db.SaveChangesAsync(ct);
            await EnviarTextoAsync(contacto, numero,
                "🔒 Mientras conversábamos, el informe de ese día quedó *aprobado*; el reporte no se guardó. " +
                "Un cambio sobre un informe aprobado lo hace el aprobador en la app.", ct);
            return;
        }

        var registroHoy = informe?.RegistrosAvance.FirstOrDefault(r => r.ActividadWBSId == act.Id);
        var calc = Calcular(b, act, registroHoy);
        if (calc.Error is not null)
        {
            b.Cantidad = null; b.Porcentaje = null; b.CantidadEsAcumulada = null;
            await CambiarPasoAsync(b, "valor", ct);
            await EnviarTextoAsync(contacto, numero, $"⚠️ {calc.Error}\n\nEscribe de nuevo el avance.", ct);
            return;
        }

        if (informe is null)
        {
            informe = new InformeDiario
            {
                ProyectoId = b.ProyectoId,
                Fecha = b.FechaTrabajo,
                NumeroCertificado = await SiguienteNumeroInformeAsync(b.ProyectoId, ct),
                CreadoPor = $"Helvis · {contacto.Nombre}",
                Estado = EstadoInforme.Borrador
            };
            _db.InformesDiarios.Add(informe);
        }
        else if (informe.Estado == EstadoInforme.Rechazado)
        {
            // Devuelto para corrección: el nuevo dato lo deja otra vez en borrador.
            informe.Estado = EstadoInforme.Borrador;
            informe.FechaModificacion = DateTime.UtcNow;
        }

        var reportadoPor = $"{contacto.Nombre} (WhatsApp)";
        var registro = registroHoy;
        if (registro is null)
        {
            registro = new RegistroAvanceDiario
            {
                ProyectoId = b.ProyectoId,
                ActividadWBSId = act.Id,
                ReportadoPor = reportadoPor
            };
            informe.RegistrosAvance.Add(registro);
        }
        else if (!registro.ReportadoPor.Contains(contacto.Nombre))
        {
            registro.ReportadoPor = Recortar($"{registro.ReportadoPor}; {reportadoPor}", 200);
        }

        var anterior = act.AvanceReal;
        registro.Fecha = b.FechaTrabajo;
        registro.PorcentajeAvance = calc.Porcentaje ?? act.AvanceReal;
        if (calc.CantidadDia is decimal dia) registro.CantidadEjecutadaDia = dia;
        if (b.Personas is int personas) registro.PersonalEnSitio = personas;
        if (b.Horas is decimal horas) registro.HorasTrabajadas = horas;
        registro.Observaciones = Recortar(string.Join("\n",
            new[] { registro.Observaciones, LineaObservacion(b, act, calc) }.Where(s => !string.IsNullOrWhiteSpace(s))), 4000);
        registro.FechaModificacion = DateTime.UtcNow;

        await _informes.ActualizarCalculosAsync(registro, act);

        // La app nunca baja el acumulado (toma el máximo); una corrección confirmada con motivo sí lo baja.
        if (calc.Porcentaje is decimal pct && pct < registro.AvanceAcumulado)
        {
            registro.AvanceAcumulado = pct;
            registro.Desviacion = Math.Round(pct - registro.AvanceEsperado, 2);
            registro.Estado = _informes.DeterminarEstado(registro.Desviacion);
            act.AvanceReal = pct;
        }
        if (calc.CantidadAcumulada is decimal acum) act.CantidadEjecutadaAcumulada = acum;
        if (act.AvanceReal > 0 && act.FechaInicioReal is null) act.FechaInicioReal = b.FechaTrabajo;
        if (act.AvanceReal >= 100) { act.Estado = EstadoActividad.Completada; act.FechaFinReal ??= b.FechaTrabajo; }
        else if (act.AvanceReal > 0 && act.Estado == EstadoActividad.Pendiente) act.Estado = EstadoActividad.EnProgreso;

        await _db.SaveChangesAsync(ct);
        await _informes.RecalcularPadresWBSAsync(b.ProyectoId);

        b.Estado = EstadosReporteHelvis.Confirmado;
        b.Paso = null;
        b.FechaConfirmacion = DateTime.UtcNow;
        b.InformeDiarioId = informe.Id;
        b.RegistroAvanceDiarioId = registro.Id;
        b.PorcentajeAnterior = anterior;
        b.PorcentajeNuevo = calc.Porcentaje;
        b.CantidadDia = calc.CantidadDia;

        var fotos = await _db.FotosHelvis.Where(f => f.ReporteAvanceHelvisId == b.Id).ToListAsync(ct);
        foreach (var f in fotos)
        {
            f.InformeDiarioId = informe.Id;
            f.ActividadWBSId = act.Id;
            f.ProyectoId = b.ProyectoId;
        }
        await _db.SaveChangesAsync(ct);

        var avance = calc.Porcentaje is decimal n ? $"*{n:0.#}%*" : $"{act.AvanceReal:0.#}% (cantidad guardada; % pendiente)";
        var fotosTxt = fotos.Count > 0 ? $"\n📷 {fotos.Count} foto(s) adjunta(s)." : "";
        await EnviarBotonesYGuardarAsync(contacto, numero,
            $"✅ *Guardado* en el Informe diario N° {informe.NumeroCertificado} del {b.FechaTrabajo:dd/MM/yyyy}.\n" +
            $"{act.CodigoWBS} {Recortar(act.Nombre, 60)}: {avance}{fotosTxt}\n\n¿Algo más?",
            [("rep_otra", "➕ Otra actividad"), ("rep_fin", "📤 Terminé por hoy"), ("menu", "🏠 Menú")], ct);
    }

    private static string LineaObservacion(ReporteAvanceHelvis b, ActividadWBS act, ResultadoCalculo calc)
    {
        var unidad = string.IsNullOrWhiteSpace(act.Unidad) ? "unidades" : act.Unidad;
        var hora = DateTime.UtcNow.AddHours(-5);
        var partes = new List<string>();
        if (b.Cantidad is decimal c)
            partes.Add($"{c:0.##} {unidad} {(b.CantidadEsAcumulada == true ? "acumulados" : "del día")}");
        if (b.Porcentaje is decimal p) partes.Add($"{p:0.#}% acumulado");
        if (calc.Porcentaje is null) partes.Add("% pendiente: la actividad no tiene cantidad total en el cronograma");
        if (!string.IsNullOrWhiteSpace(b.Motivo)) partes.Add($"corrección: {b.Motivo}");
        if (!string.IsNullOrWhiteSpace(b.Observaciones)) partes.Add(b.Observaciones!);
        return $"[WhatsApp {hora:dd/MM HH:mm}] {string.Join(" · ", partes)}";
    }

    // ==================== Cierre del aporte ====================

    /// <summary>
    /// "Terminé por hoy": muestra lo consolidado de la persona, pasa el informe a revisión
    /// y avisa a los aprobadores del proyecto con los botones Aprobar / Devolver (§4, §12.2).
    /// </summary>
    private async Task TerminarAporteAsync(ContactoHelvis contacto, Proyecto proyecto, DateTime fecha, CancellationToken ct)
    {
        var mios = await _db.ReportesAvanceHelvis.Include(r => r.Actividad)
            .Where(r => r.ContactoHelvisId == contacto.Id && r.ProyectoId == proyecto.Id &&
                        r.FechaTrabajo == fecha && r.Estado == EstadosReporteHelvis.Confirmado)
            .OrderBy(r => r.Id)
            .ToListAsync(ct);
        var informe = await InformeDelDiaAsync(proyecto.Id, fecha, ct);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📋 *Tu aporte del {fecha:dd/MM/yyyy} — {proyecto.Codigo}*");
        sb.AppendLine();
        // Un renglón por actividad con lo último confirmado (los envíos sucesivos quedan en el historial).
        foreach (var r in mios.GroupBy(x => x.ActividadWBSId).Select(g => g.Last()))
        {
            var avance = r.PorcentajeNuevo is decimal n ? $"{n:0.#}%" : "% pendiente";
            var cant = r.CantidadDia is decimal d ? $" · {d:0.##} {r.Actividad?.Unidad ?? "und"} en el día" : "";
            sb.AppendLine($"• {r.Actividad?.CodigoWBS} {Recortar(r.Actividad?.Nombre ?? "", 45)}: {avance}{cant}");
        }
        if (mios.Count == 0) sb.AppendLine("No hay reportes confirmados ese día.");

        if (informe is null)
        {
            await EnviarTextoAsync(contacto, contacto.Telefono, sb.ToString().TrimEnd(), ct);
            return;
        }

        if (informe.Estado is EstadoInforme.Borrador or EstadoInforme.Rechazado)
        {
            informe.Estado = EstadoInforme.EnRevision;
            informe.FechaModificacion = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        var aprobadores = await AprobadoresAsync(proyecto.Id, contacto.Id, ct);
        sb.AppendLine();
        if (informe.Estado == EstadoInforme.Aprobado)
            sb.Append("El informe de ese día ya estaba aprobado.");
        else if (aprobadores.Count == 0)
            sb.Append($"Quedó listo para revisión (informe N° {informe.NumeroCertificado}). Aún no hay aprobadores " +
                      "configurados en el panel de Helvis; se puede aprobar en la app. ¡Gracias! 🙌");
        else
            sb.Append($"Quedó enviado a revisión de {string.Join(" y ", aprobadores.Select(a => a.Nombre))}. ¡Gracias! 🙌");

        await EnviarTextoAsync(contacto, contacto.Telefono, sb.ToString(), ct);

        if (informe.Estado == EstadoInforme.EnRevision)
            foreach (var aprobador in aprobadores)
                await EnviarSolicitudRevisionAsync(aprobador, informe, proyecto,
                    $"📨 {contacto.Nombre} terminó su reporte del día.", ct);
    }

    // ==================== Fotos ====================

    private async Task RecibirFotoAsync(ContactoHelvis contacto, MensajeWhatsApp mensaje, string saludo, CancellationToken ct)
    {
        var borrador = await BorradorActualAsync(contacto, ct);
        var archivo = mensaje.MediaId is null ? null : await _wa.DescargarMediaAsync(mensaje.MediaId, ct);
        if (archivo is null)
        {
            await EnviarTextoAsync(contacto, contacto.Telefono,
                saludo + "📷 No pude descargar la foto. ¿La envías de nuevo?", ct);
            return;
        }

        _db.FotosHelvis.Add(new FotoHelvis
        {
            ContactoHelvisId = contacto.Id,
            ProyectoId = borrador?.ProyectoId ?? contacto.ProyectoActivoId,
            ReporteAvanceHelvisId = borrador?.Id,
            ActividadWBSId = borrador?.ActividadWBSId,
            WaMediaId = mensaje.MediaId,
            MimeType = archivo.Value.MimeType,
            Contenido = archivo.Value.Contenido,
            Descripcion = string.IsNullOrWhiteSpace(mensaje.Texto) ? null : Recortar(mensaje.Texto, 1000)
        });
        await _db.SaveChangesAsync(ct);

        if (borrador is null)
        {
            await EnviarTextoAsync(contacto, contacto.Telefono,
                saludo + "📷 Foto guardada. Se adjunta sola al próximo *reporte de avance* que hagas en las próximas 2 horas.\n\n" +
                "Escribe *reportar* para registrar el avance.", ct);
            return;
        }

        var total = await _db.FotosHelvis.CountAsync(f => f.ReporteAvanceHelvisId == borrador.Id, ct);
        if (borrador.Paso == "fotos")
            await EnviarBotonesYGuardarAsync(contacto, contacto.Telefono,
                $"📷 Foto {total} guardada. Envía más o toca *Continuar*.", [("rep_seguir", "Continuar")], ct);
        else
            await EnviarTextoAsync(contacto, contacto.Telefono,
                $"📷 Foto guardada con el reporte en curso ({total}). Sigamos: responde la pregunta anterior.", ct);
    }

    // ==================== Utilidades del reporte ====================

    private async Task CambiarPasoAsync(ReporteAvanceHelvis b, string paso, CancellationToken ct)
    {
        b.Paso = paso;
        b.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private Task<InformeDiario?> InformeDelDiaAsync(int proyectoId, DateTime fecha, CancellationToken ct)
    {
        var desde = fecha.Date;
        var hasta = desde.AddDays(1);
        return _db.InformesDiarios
            .Include(i => i.RegistrosAvance)
            .Where(i => i.ProyectoId == proyectoId && i.Fecha >= desde && i.Fecha < hasta)
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync(ct);
    }

    private async Task<RegistroAvanceDiario?> RegistroDelDiaAsync(int proyectoId, int actividadId, DateTime fecha, CancellationToken ct)
    {
        var informe = await InformeDelDiaAsync(proyectoId, fecha, ct);
        return informe?.RegistrosAvance.FirstOrDefault(r => r.ActividadWBSId == actividadId);
    }

    /// <summary>Mismo consecutivo que la app (001, 002…), propio de cada proyecto.</summary>
    private async Task<string> SiguienteNumeroInformeAsync(int proyectoId, CancellationToken ct)
    {
        var numeros = await _db.InformesDiarios
            .Where(i => i.ProyectoId == proyectoId && i.NumeroCertificado != null)
            .Select(i => i.NumeroCertificado!)
            .ToListAsync(ct);
        var max = numeros.Select(n => int.TryParse(n, out var x) ? x : 0).DefaultIfEmpty(0).Max();
        return (max + 1).ToString("D3");
    }

    /// <summary>Cancela el reporte y cualquier conversación de varios pasos. Devuelve si había algo.</summary>
    private async Task<bool> CancelarConversacionesAsync(ContactoHelvis contacto, CancellationToken ct)
    {
        var borradores = await _db.ReportesAvanceHelvis
            .Where(r => r.ContactoHelvisId == contacto.Id && r.Estado == EstadosReporteHelvis.Borrador)
            .ToListAsync(ct);
        foreach (var b in borradores) { b.Estado = EstadosReporteHelvis.Cancelado; b.FechaModificacion = DateTime.UtcNow; }

        var habia = borradores.Count > 0 || contacto.Flujo is not null;
        contacto.Flujo = null;
        contacto.FlujoPaso = null;
        contacto.FlujoDatos = null;
        contacto.FlujoFecha = null;
        await _db.SaveChangesAsync(ct);
        return habia;
    }

    /// <summary>Botones (máx. 3) con registro en el historial.</summary>
    private async Task EnviarBotonesYGuardarAsync(ContactoHelvis? contacto, string numero, string cuerpo,
        (string Id, string Titulo)[] botones, CancellationToken ct)
    {
        var ok = await _wa.EnviarBotonesAsync(numero, cuerpo, botones, ct);
        await GuardarSalienteAsync(contacto, numero,
            cuerpo + "\n[" + string.Join(" | ", botones.Select(b => b.Titulo)) + "]", "interactive", ok, ct);
    }

    /// <summary>Durante una pausa (§13) Helvis no envía avisos a esa persona.</summary>
    private static bool PuedeRecibirAvisos(ContactoHelvis c) =>
        c.Activo && (c.PausadoHasta is null || c.PausadoHasta <= DateTime.UtcNow);
}
