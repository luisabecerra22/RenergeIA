using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RenergeIA.Core.Enums;
using RenergeIA.Core.Helpers;
using RenergeIA.Infrastructure.Data;
using RenergeIA.Infrastructure.Identity;

namespace RenergeIA.Web.Services.Acceso;

// Lo que una persona puede ver y hacer en la app. Se arma una vez por persona y se
// guarda en caché un minuto (los cambios del panel se ven al minuto, o al instante
// para quien los hizo porque el panel invalida la caché).
public sealed class AccesoUsuario
{
    public static readonly AccesoUsuario Anonimo = new() { Activo = false };

    public string? UsuarioId { get; init; }
    public string Nombre { get; init; } = "";
    public bool Activo { get; init; }
    public bool EsAdministrador { get; init; }

    // Usuario creado antes del panel y nunca configurado: conserva acceso total.
    public bool SinConfigurar { get; init; }

    public bool AccesoTotal => Activo && (EsAdministrador || SinConfigurar);

    public HashSet<int> Proyectos { get; init; } = [];
    public Dictionary<(int? ProyectoId, string Modulo), NivelAcceso> Permisos { get; init; } = [];

    public bool VeProyecto(int proyectoId) => AccesoTotal || (Activo && Proyectos.Contains(proyectoId));

    public NivelAcceso Nivel(int? proyectoId, string modulo)
    {
        if (!Activo) return NivelAcceso.Ninguno;
        if (AccesoTotal) return NivelAcceso.Aprobar;
        if (proyectoId is int id && !Proyectos.Contains(id)) return NivelAcceso.Ninguno;
        return Permisos.TryGetValue((proyectoId, modulo), out var n) ? n : NivelAcceso.Ninguno;
    }

    public bool Puede(int? proyectoId, string modulo, NivelAcceso minimo = NivelAcceso.Ver) =>
        Nivel(proyectoId, modulo) >= minimo;

    public bool VeAlgunaAreaHseq(int proyectoId) =>
        ModulosAcceso.AreasHseq.Any(m => Puede(proyectoId, m));

    public bool Cumple(ModulosAcceso.Requisito r)
    {
        if (r.Tipo == ModulosAcceso.TipoRequisito.Libre) return true;
        if (!Activo) return false;
        if (AccesoTotal) return true;
        return r.Tipo switch
        {
            ModulosAcceso.TipoRequisito.Administrador => false,
            ModulosAcceso.TipoRequisito.Corporativo => r.Modulos!.Any(m => Puede(null, m, r.Nivel)),
            ModulosAcceso.TipoRequisito.Proyecto => r.ProyectoId is int id && Proyectos.Contains(id)
                && (r.Modulos is not { Length: > 0 } || r.Modulos.Any(m => Puede(id, m, r.Nivel))),
            _ => false
        };
    }
}

public class PermisosService(
    IServiceScopeFactory scopes,
    AuthenticationStateProvider auth,
    IMemoryCache cache)
{
    private static string Clave(string usuarioId) => "acceso:" + usuarioId;

    public static void Invalidar(IMemoryCache cache, string usuarioId) => cache.Remove(Clave(usuarioId));

    // Acceso de quien está usando la página (sirve en SSR y dentro del circuito).
    public async Task<AccesoUsuario> ActualAsync()
    {
        var estado = await auth.GetAuthenticationStateAsync();
        return await ParaAsync(estado.User);
    }

    public async Task<AccesoUsuario> ParaAsync(ClaimsPrincipal? user)
    {
        var id = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user?.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(id))
            return AccesoUsuario.Anonimo;

        if (cache.TryGetValue(Clave(id), out AccesoUsuario? enCache) && enCache is not null)
            return enCache;

        var acceso = await CargarAsync(id);
        cache.Set(Clave(id), acceso, TimeSpan.FromMinutes(1));
        return acceso;
    }

    // Scope propio: el DbContext del circuito se comparte con la página y no admite
    // dos consultas a la vez.
    private async Task<AccesoUsuario> CargarAsync(string usuarioId)
    {
        await using var scope = scopes.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<RenergeIADbContext>();

        var u = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usuarioId);
        if (u is null) return AccesoUsuario.Anonimo;

        var esAdmin = await (from ur in db.UserRoles
                             join r in db.Roles on ur.RoleId equals r.Id
                             where ur.UserId == usuarioId && r.Name == Roles.Administrador
                             select ur).AnyAsync();

        var proyectos = await db.ProyectosUsuario.AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId).Select(x => x.ProyectoId).ToListAsync();
        var permisos = await db.PermisosUsuario.AsNoTracking()
            .Where(x => x.UsuarioId == usuarioId && x.Nivel > NivelAcceso.Ninguno).ToListAsync();

        return new AccesoUsuario
        {
            UsuarioId = u.Id,
            Nombre = string.IsNullOrWhiteSpace(u.NombreCompleto) ? u.Email ?? "" : u.NombreCompleto,
            Activo = u.Activo,
            EsAdministrador = esAdmin,
            SinConfigurar = !u.PermisosConfigurados,
            Proyectos = proyectos.ToHashSet(),
            Permisos = permisos.ToDictionary(p => (p.ProyectoId, p.Modulo), p => p.Nivel)
        };
    }
}
