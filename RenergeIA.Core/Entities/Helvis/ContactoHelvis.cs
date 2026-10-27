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
