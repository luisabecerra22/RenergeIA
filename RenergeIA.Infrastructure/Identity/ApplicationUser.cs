using Microsoft.AspNetCore.Identity;

namespace RenergeIA.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // false = usuario anterior al panel de permisos: conserva acceso total hasta que
    // un administrador guarde su configuración (así nadie queda bloqueado al activarlo).
    public bool PermisosConfigurados { get; set; }

    // Perfil plantilla del que se copiaron los permisos (solo referencia).
    public int? PerfilAccesoId { get; set; }

    public DateTime? UltimoIngreso { get; set; }
}
