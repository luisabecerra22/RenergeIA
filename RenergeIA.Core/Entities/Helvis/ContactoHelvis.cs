namespace RenergeIA.Core.Entities.Helvis;

// Persona autorizada a conversar con Helvis (el asistente de WhatsApp).
// El teléfono es la identidad: se guarda normalizado como lo envía Meta
// (solo dígitos con indicativo de país, ej. "573187222977").
// Los permisos son independientes: consultar no implica reportar, corregir ni aprobar
// (regla del documento ESPECIFICACION_HELVIS §3).
public class ContactoHelvis : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;   // único, solo dígitos
    public string? Cargo { get; set; }
    public string? Email { get; set; }

    public bool Activo { get; set; } = true;

    // Permisos (explícitos e independientes)
    public bool PuedeConsultar { get; set; } = true;
    public bool PuedeReportar { get; set; }
    public bool PuedeCorregir { get; set; }
    public bool PuedeAprobar { get; set; }
    public bool EsAdministrador { get; set; }

    // Pausa de mensajes (documento §13): mientras esté vigente, Helvis no insiste.
    public DateTime? PausadoHasta { get; set; }
    public string? MotivoPausa { get; set; }

    // Proyecto "activo" de la conversación: el bot lo pregunta al saludar y lo recuerda
    // unas horas; "proyecto" lo cambia. Sin FK: se valida contra los proyectos asignados.
    public int? ProyectoActivoId { get; set; }
    public DateTime? ProyectoActivoFecha { get; set; }

    // Opción pedida mientras Helvis preguntaba el proyecto (decisión H del panel):
    // se responde apenas la persona elige el proyecto y se limpia.
    public string? OpcionPendiente { get; set; }

    // Conversación de varios pasos en curso (bloqueo, devolución de informe…):
    // Flujo = cuál, FlujoPaso = qué se espera, FlujoDatos = lo recogido (JSON).
    // El reporte de avance tiene su propio borrador (ReporteAvanceHelvis).
    public string? Flujo { get; set; }
    public string? FlujoPaso { get; set; }
    public string? FlujoDatos { get; set; }
    public DateTime? FlujoFecha { get; set; }

    // Destinatarios de avisos (configurables en el panel, documento §11.3 y §12.5)
    public bool RecibeInformes { get; set; }   // informe aprobado (Michelle, Luisa)
    public bool RecibeBloqueos { get; set; }   // bloqueos que detienen el avance (Yesid, Javier)

    public string? Notas { get; set; }

    public List<ContactoHelvisProyecto> Proyectos { get; set; } = [];
}

// Proyectos que el contacto puede ver/reportar. Un contacto puede tener varios.
public class ContactoHelvisProyecto : EntidadBase
{
    public int ContactoHelvisId { get; set; }
    public ContactoHelvis Contacto { get; set; } = null!;
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
}
