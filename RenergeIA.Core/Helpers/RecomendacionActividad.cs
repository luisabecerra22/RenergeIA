using System.Globalization;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Helpers;

/// <summary>
/// Recomendación automática del Plan de acción del Dashboard, armada con datos reales de la
/// actividad (fechas, avance, restricciones abiertas ligadas por el Informe Diario y la última
/// observación) en vez de una frase fija. Reglas deterministas: mismos datos → mismo texto.
/// Si alguien escribe una recomendación manual (ActividadWBS.RecomendacionManual) esa gana.
/// </summary>
public static class RecomendacionActividad
{
    private static readonly CultureInfo Es = CultureInfo.GetCultureInfo("es-CO");

    public sealed record Datos(
        string Nombre,
        DateTime FechaInicioPlaneada,
        DateTime FechaFinPlaneada,
        decimal AvanceReal,
        decimal AvanceProgramado,
        bool EsCritica,
        IReadOnlyList<Restriccion> RestriccionesAbiertas,
        DateTime? FechaUltimaObservacion,
        string? UltimaObservacion,
        DateTime? FechaUltimoInforme);

    public static string Generar(Datos d, DateTime hoy)
    {
        var partes = new List<string>();
        var falta  = Math.Max(0m, 100m - d.AvanceReal);
        var fin    = d.FechaFinPlaneada.Date;
        var inicio = d.FechaInicioPlaneada.Date;
        hoy = hoy.Date;

        // 1. Situación frente al cronograma
        if (d.AvanceReal <= 0 && inicio < hoy)
        {
            partes.Add($"No ha iniciado y debió empezar el {F(inicio)} (hace {Dias(inicio, hoy)}).");
        }
        else if (fin < hoy)
        {
            partes.Add($"Venció el {F(fin)} (hace {Dias(fin, hoy)}) y falta {Pct(falta)} por ejecutar.");
        }
        else
        {
            var diasRestantes = (fin - hoy).Days;
            var ritmoSemanal  = diasRestantes > 0 ? falta / diasRestantes * 7m : falta;
            partes.Add(diasRestantes == 0
                ? $"Termina hoy y falta {Pct(falta)} por ejecutar."
                : $"Quedan {diasRestantes} días al {F(fin)}: falta {Pct(falta)}, se requieren ≈ {Pct(ritmoSemanal)} por semana para cerrar a tiempo.");
        }

        // 2. Restricciones abiertas ligadas a la actividad
        if (d.RestriccionesAbiertas.Count > 0)
        {
            var r = d.RestriccionesAbiertas
                .OrderBy(x => x.Estado == EstadoRestriccion.Abierta ? 0 : 1)
                .ThenByDescending(x => x.FechaIdentificacion)
                .First();
            var sb = $"Restricción {r.Numero}";
            if (!string.IsNullOrWhiteSpace(r.Tipo)) sb += $" ({r.Tipo.ToLowerInvariant()}, {EstadoTexto(r.Estado)})";
            else sb += $" ({EstadoTexto(r.Estado)})";
            sb += $": {Recortar(r.Descripcion, 90)}";
            if (!string.IsNullOrWhiteSpace(r.Responsable)) sb += $"; responsable {r.Responsable.Trim()}";
            if (r.FechaCompromiso is { } fc)
                sb += fc.Date < hoy
                    ? $", compromiso vencido el {F(fc)}"
                    : $", compromiso el {F(fc)}";
            sb += ".";
            if (d.RestriccionesAbiertas.Count > 1)
                sb += $" Hay {d.RestriccionesAbiertas.Count - 1} restricción(es) más abierta(s).";
            partes.Add(sb);
        }
        else
        {
            partes.Add("Sin restricción registrada: identificar la causa del atraso y registrarla en Restricciones.");
        }

        // 3. Última observación del Informe Diario
        if (!string.IsNullOrWhiteSpace(d.UltimaObservacion))
        {
            var cuando = d.FechaUltimaObservacion is { } fo ? $" ({F(fo)})" : "";
            partes.Add($"Última observación{cuando}: «{Recortar(d.UltimaObservacion, 110)}».");
        }
        else if (d.FechaUltimoInforme is { } fi && (hoy - fi.Date).Days >= 7)
        {
            partes.Add($"Sin informe diario desde el {F(fi)}: actualizar el avance en campo.");
        }
        else if (d.FechaUltimoInforme is null)
        {
            partes.Add("No tiene informes diarios: el avance mostrado es el digitado en el cronograma.");
        }

        // 4. Ruta crítica
        if (d.EsCritica)
            partes.Add("Está en la ruta crítica: cada día de atraso corre la fecha fin del proyecto.");

        return string.Join(" ", partes);
    }

    private static string F(DateTime f) => f.ToString("dd/MM/yyyy", Es);
    private static string Pct(decimal v) => v.ToString("0.#", Es) + " %";
    private static string Dias(DateTime desde, DateTime hasta)
    {
        var n = (hasta.Date - desde.Date).Days;
        return n == 1 ? "1 día" : $"{n} días";
    }
    private static string EstadoTexto(EstadoRestriccion e) => e switch
    {
        EstadoRestriccion.Abierta   => "abierta",
        EstadoRestriccion.EnGestion => "en gestión",
        EstadoRestriccion.Levantada => "levantada",
        _                           => "cancelada"
    };
    private static string Recortar(string? s, int max)
    {
        s = (s ?? "").Trim().Replace('\n', ' ').Replace('\r', ' ');
        return s.Length <= max ? s : s[..(max - 1)].TrimEnd() + "…";
    }
}
