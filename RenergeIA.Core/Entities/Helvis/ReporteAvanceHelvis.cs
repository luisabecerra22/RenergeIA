namespace RenergeIA.Core.Entities.Helvis;

// Reporte de avance recogido por WhatsApp (documento §4 y §7).
// Mientras la persona no confirma, es un BORRADOR: no toca el avance oficial.
// Al confirmar se escribe UNA sola vez en el Informe diario (RegistroAvanceDiario)
// y queda el vínculo para no aplicarlo dos veces.
public class ReporteAvanceHelvis : EntidadBase
{
    public int ContactoHelvisId { get; set; }
    public ContactoHelvis Contacto { get; set; } = null!;
    public int ProyectoId { get; set; }
    public int? ActividadWBSId { get; set; }
    public ActividadWBS? Actividad { get; set; }

    // Fecha del trabajo (día Colombia). Puede ser el día anterior hasta las 10:00 a. m. (§7.3).
    public DateTime FechaTrabajo { get; set; }

    public string Estado { get; set; } = EstadosReporteHelvis.Borrador;
    // Siguiente dato que Helvis está esperando (actividad, valor, tipo, motivo, recursos, fotos, confirmar…)
    public string? Paso { get; set; }

    public string? TextoOriginal { get; set; }

    // Lo que dijo la persona
    public decimal? Cantidad { get; set; }          // en la unidad de la actividad
    public bool? CantidadEsAcumulada { get; set; }  // null = aún no se sabe si es del día o acumulada
    public decimal? Porcentaje { get; set; }        // porcentaje acumulado dicho por la persona

    // Resultado calculado al cerrar el borrador
    public decimal? PorcentajeAnterior { get; set; }
    public decimal? PorcentajeNuevo { get; set; }   // null = cantidad sin total del WBS: pendiente de convertir
    public decimal? CantidadDia { get; set; }
    public bool? SumarAlDeHoy { get; set; }         // ya había reporte hoy: true = trabajo adicional, false = reemplaza; null = sin preguntar
    public string? Motivo { get; set; }             // obligatorio si el avance baja (§7.2)

    // Recursos (opcionales): personas y horas, sin nombres (§8.1)
    public int? Personas { get; set; }
    public decimal? Horas { get; set; }
    public string? Observaciones { get; set; }

    // Control del diálogo: pasos opcionales ya respondidos y actividades ofrecidas
    // en la última lista numerada (ids separados por coma, para entender "2").
    public bool RecursosRespondidos { get; set; }
    public bool FotosRespondidas { get; set; }
    public string? OpcionesActividad { get; set; }

    public DateTime? FechaConfirmacion { get; set; }
    public int? InformeDiarioId { get; set; }
    public int? RegistroAvanceDiarioId { get; set; }
}

public static class EstadosReporteHelvis
{
    public const string Borrador = "borrador";
    public const string Confirmado = "confirmado";
    public const string Cancelado = "cancelado";
}

// Foto recibida por WhatsApp. Se guarda en la base de datos (no en el disco del
// contenedor, que se pierde en cada despliegue) y se liga al reporte, la actividad
// y el informe diario cuando se confirma.
public class FotoHelvis : EntidadBase
{
    public int? ContactoHelvisId { get; set; }
    public ContactoHelvis? Contacto { get; set; }
    public int? ProyectoId { get; set; }
    public int? ReporteAvanceHelvisId { get; set; }
    public int? ActividadWBSId { get; set; }
    public int? InformeDiarioId { get; set; }
    public string? WaMediaId { get; set; }
    public string MimeType { get; set; } = "image/jpeg";
    public byte[] Contenido { get; set; } = [];
    public string? Descripcion { get; set; }
}
