using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.Acceso;

// Permisos de la app web por persona (panel /configuracion).
// El usuario es el de ASP.NET Identity (AspNetUsers.Id, string); la FK se configura
// en el DbContext porque ApplicationUser vive en Infrastructure.
// Las claves de módulo salen de ModulosAcceso (Core/Helpers): nunca escribirlas a mano.

// Proyectos en los que participa la persona. Sin fila aquí, el proyecto no existe para ella.
public class ProyectoUsuario : EntidadBase
{
    public string UsuarioId { get; set; } = string.Empty;
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
}

// Nivel de la persona en un módulo. ProyectoId null = módulo corporativo (Compromisos, Helvis, HSEQ global).
public class PermisoUsuario : EntidadBase
{
    public string UsuarioId { get; set; } = string.Empty;
    public int? ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }
    public string Modulo { get; set; } = string.Empty;
    public NivelAcceso Nivel { get; set; }
}

// Plantilla de permisos (Residente, HSE de obra…). Aplicarla copia sus niveles al usuario;
// después cada persona se ajusta sin tocar el perfil.
public class PerfilAcceso : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public List<PermisoPerfil> Permisos { get; set; } = [];
}

// Nivel del perfil por módulo. Para módulos de proyecto aplica a todos los proyectos asignados.
public class PermisoPerfil : EntidadBase
{
    public int PerfilAccesoId { get; set; }
    public PerfilAcceso Perfil { get; set; } = null!;
    public string Modulo { get; set; } = string.Empty;
    public NivelAcceso Nivel { get; set; }
}

// Registro de quién dio o quitó accesos y cuándo (solo agregar).
public class BitacoraAcceso : EntidadBase
{
    public string Autor { get; set; } = string.Empty;
    public string? UsuarioAfectado { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string? Detalle { get; set; }
}
