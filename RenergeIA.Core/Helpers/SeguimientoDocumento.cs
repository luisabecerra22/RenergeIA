using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Helpers;

public enum SemaforoDocumento
{
    AlDia,
    SinFecha,
    Verde,
    Amarillo,
    Rojo
}

/// <summary>
/// Reglas de seguimiento de la Planificación Documental (FO-SI-GC-002-1):
/// días sin atender = hoy − fecha más reciente registrada (misma regla de la columna
/// "Tiempo de retraso" del Excel) y "en cancha de quién" está el documento.
/// </summary>
public static class SeguimientoDocumento
{
    public const int DiasAmarillo = 4;
    public const int DiasRojo = 8;

    public static DateTime HoyColombia() => DateTime.UtcNow.AddHours(-5).Date;

    /// <summary>Validado, Validado con comentarios e Informativos no requieren atención.</summary>
    public static bool RequiereAtencion(Documento d) =>
        d.Estado is EstadoDocumento.PendienteEmitir or EstadoDocumento.PendienteValidacion or EstadoDocumento.NoValidado;

    public static DateTime? UltimaFecha(Documento d) =>
        new[] { d.FechaEmision, d.FechaEntrega1, d.FechaDevolucion1, d.FechaEntrega2, d.FechaDevolucion2,
                d.FechaEntrega3, d.FechaDevolucion3, d.FechaEntrega4, d.FechaDevolucion4, d.FechaValidacion }
            .Where(f => f.HasValue).Max();

    public static int? DiasSinAtender(Documento d)
    {
        if (!RequiereAtencion(d)) return null;
        var ultima = UltimaFecha(d);
        if (!ultima.HasValue) return null;
        return Math.Max(0, (int)(HoyColombia() - ultima.Value.Date).TotalDays);
    }

    public static SemaforoDocumento Semaforo(Documento d)
    {
        if (!RequiereAtencion(d)) return SemaforoDocumento.AlDia;
        var dias = DiasSinAtender(d);
        if (dias is null) return SemaforoDocumento.SinFecha;
        if (dias >= DiasRojo) return SemaforoDocumento.Rojo;
        if (dias >= DiasAmarillo) return SemaforoDocumento.Amarillo;
        return SemaforoDocumento.Verde;
    }

    /// <summary>Columna "Responsable" de Ingeniería: en cancha de quién está el documento.</summary>
    public static string EnCanchaDe(Documento d) => d.Estado switch
    {
        EstadoDocumento.PendienteValidacion    => "Cliente",
        EstadoDocumento.PendienteEmitir        => "Renergeia",
        EstadoDocumento.NoValidado             => "Renergeia",
        EstadoDocumento.Validado               => "OK para construcción",
        EstadoDocumento.ValidadoConComentarios => "OK con comentarios",
        _                                      => "Informativo"
    };

    public static string TextoRetraso(Documento d)
    {
        if (!RequiereAtencion(d)) return "Al día";
        var dias = DiasSinAtender(d);
        return dias is null ? "No registra fecha" : $"{dias} día{(dias == 1 ? "" : "s")} sin atender";
    }
}
