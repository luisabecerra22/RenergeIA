using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RenergeIA.Core.Entities.Acceso;
using RenergeIA.Core.Enums;
using RenergeIA.Core.Helpers;
using RenergeIA.Infrastructure.Data;
using RenergeIA.Infrastructure.Identity;

namespace RenergeIA.Web.Services.Acceso;

public record UsuarioFila(
    string Id, string Nombre, string Email, string? Cargo, bool Activo, bool EsAdministrador,
    bool Configurado, string? Perfil, int Proyectos, DateTime? UltimoIngreso);

// Lo que se edita en el modal del panel. Las claves de módulo salen de ModulosAcceso.
public class EdicionUsuario
{
    public string? Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Cargo { get; set; }
    public bool Activo { get; set; } = true;
    public bool EsAdministrador { get; set; }
    public int? PerfilAccesoId { get; set; }
    public string? Contrasena { get; set; }
    public bool Configurado { get; set; } = true;

    public HashSet<int> Proyectos { get; set; } = [];
    public Dictionary<string, NivelAcceso> Corporativos { get; set; } = [];
    public Dictionary<(int ProyectoId, string Modulo), NivelAcceso> PorProyecto { get; set; } = [];

    public NivelAcceso NivelProyecto(int proyectoId, string modulo) =>
        PorProyecto.TryGetValue((proyectoId, modulo), out var n) ? n : NivelAcceso.Ninguno;

    public NivelAcceso NivelCorporativo(string modulo) =>
        Corporativos.TryGetValue(modulo, out var n) ? n : NivelAcceso.Ninguno;
}

public class EdicionPerfil
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public Dictionary<string, NivelAcceso> Niveles { get; set; } = [];

    public NivelAcceso Nivel(string modulo) => Niveles.TryGetValue(modulo, out var n) ? n : NivelAcceso.Ninguno;
}

// Operaciones del panel /configuracion/usuarios. Toda modificación deja huella en
// BitacoraAccesos e invalida la caché de PermisosService de la persona afectada.
public class UsuariosAccesoService(
    RenergeIADbContext db,
    UserManager<ApplicationUser> users,
    RoleManager<IdentityRole> roles,
    IMemoryCache cache)
{
    public async Task<List<UsuarioFila>> ListarAsync()
    {
        var admins = (await users.GetUsersInRoleAsync(Roles.Administrador)).Select(u => u.Id).ToHashSet();
        var perfiles = await db.PerfilesAcceso.AsNoTracking().ToDictionaryAsync(p => p.Id, p => p.Nombre);
        var proyectos = await db.ProyectosUsuario.AsNoTracking()
            .GroupBy(x => x.UsuarioId).Select(g => new { g.Key, N = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.N);

        var lista = await db.Users.AsNoTracking().ToListAsync();
        return lista
            .Select(u => new UsuarioFila(u.Id, string.IsNullOrWhiteSpace(u.NombreCompleto) ? u.Email ?? "" : u.NombreCompleto,
                u.Email ?? "", u.Cargo, u.Activo, admins.Contains(u.Id), u.PermisosConfigurados,
                u.PerfilAccesoId is int p && perfiles.TryGetValue(p, out var n) ? n : null,
                proyectos.GetValueOrDefault(u.Id), u.UltimoIngreso))
            .OrderByDescending(f => f.Activo).ThenBy(f => f.Nombre)
            .ToList();
    }

    public async Task<EdicionUsuario?> ObtenerAsync(string id)
    {
        var u = await users.FindByIdAsync(id);
        if (u is null) return null;
        var e = new EdicionUsuario
        {
            Id = u.Id, Nombre = u.NombreCompleto, Email = u.Email ?? "", Cargo = u.Cargo,
            Activo = u.Activo, EsAdministrador = await users.IsInRoleAsync(u, Roles.Administrador),
            PerfilAccesoId = u.PerfilAccesoId, Configurado = u.PermisosConfigurados,
            Proyectos = (await db.ProyectosUsuario.AsNoTracking().Where(x => x.UsuarioId == id)
                .Select(x => x.ProyectoId).ToListAsync()).ToHashSet()
        };
        foreach (var p in await db.PermisosUsuario.AsNoTracking().Where(x => x.UsuarioId == id).ToListAsync())
        {
            if (p.ProyectoId is int pid) e.PorProyecto[(pid, p.Modulo)] = p.Nivel;
            else e.Corporativos[p.Modulo] = p.Nivel;
        }
        return e;
    }

    // Devuelve la lista de errores (vacía = guardado).
    public async Task<List<string>> GuardarAsync(EdicionUsuario e, string autorId, string autorNombre)
    {
        var errores = new List<string>();
        e.Nombre = e.Nombre.Trim();
        e.Email = e.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(e.Nombre)) errores.Add("Escribe el nombre.");
        if (string.IsNullOrWhiteSpace(e.Email) || !e.Email.Contains('@')) errores.Add("Escribe un correo válido.");
        if (e.Id is null && string.IsNullOrWhiteSpace(e.Contrasena)) errores.Add("Define una contraseña para el usuario nuevo.");
        if (e.Id == autorId && !e.EsAdministrador) errores.Add("No puedes quitar tu propio rol de administrador.");
        if (e.Id == autorId && !e.Activo) errores.Add("No puedes desactivar tu propio usuario.");
        if (errores.Count > 0) return errores;

        var otro = await users.FindByEmailAsync(e.Email);
        if (otro is not null && otro.Id != e.Id) return ["Ya existe un usuario con ese correo."];

        var anterior = e.Id is null ? null : await ObtenerAsync(e.Id);
        var esNuevo = e.Id is null;
        ApplicationUser u;
        if (esNuevo)
        {
            u = new ApplicationUser { UserName = e.Email, Email = e.Email, EmailConfirmed = true };
            Copiar(e, u);
            var r = await users.CreateAsync(u, e.Contrasena!);
            if (!r.Succeeded) return Traducir(r);
            e.Id = u.Id;
        }
        else
        {
            u = (await users.FindByIdAsync(e.Id!))!;
            var desactiva = u.Activo && !e.Activo;
            Copiar(e, u);
            u.UserName = e.Email;
            u.Email = e.Email;
            var r = await users.UpdateAsync(u);
            if (!r.Succeeded) return Traducir(r);
            if (!string.IsNullOrWhiteSpace(e.Contrasena))
            {
                var token = await users.GeneratePasswordResetTokenAsync(u);
                r = await users.ResetPasswordAsync(u, token, e.Contrasena);
                if (!r.Succeeded) return Traducir(r);
            }
            // Cierra la sesión abierta en otros equipos (Identity revisa el sello cada 30 min;
            // la guardia del layout ya lo bloquea al minuto).
            if (desactiva) await users.UpdateSecurityStampAsync(u);
        }

        if (!await roles.RoleExistsAsync(Roles.Administrador)) await roles.CreateAsync(new IdentityRole(Roles.Administrador));
        var eraAdmin = await users.IsInRoleAsync(u, Roles.Administrador);
        if (e.EsAdministrador && !eraAdmin) await users.AddToRoleAsync(u, Roles.Administrador);
        if (!e.EsAdministrador && eraAdmin) await users.RemoveFromRoleAsync(u, Roles.Administrador);

        // Reemplazar proyectos y permisos (solo los de proyectos asignados y con nivel).
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.ProyectosUsuario.Where(x => x.UsuarioId == u.Id).ExecuteDeleteAsync();
        await db.PermisosUsuario.Where(x => x.UsuarioId == u.Id).ExecuteDeleteAsync();
        db.ProyectosUsuario.AddRange(e.Proyectos.Select(p => new ProyectoUsuario { UsuarioId = u.Id, ProyectoId = p }));
        db.PermisosUsuario.AddRange(e.PorProyecto
            .Where(kv => kv.Value > NivelAcceso.Ninguno && e.Proyectos.Contains(kv.Key.ProyectoId))
            .Select(kv => new PermisoUsuario { UsuarioId = u.Id, ProyectoId = kv.Key.ProyectoId, Modulo = kv.Key.Modulo, Nivel = kv.Value }));
        db.PermisosUsuario.AddRange(e.Corporativos
            .Where(kv => kv.Value > NivelAcceso.Ninguno)
            .Select(kv => new PermisoUsuario { UsuarioId = u.Id, Modulo = kv.Key, Nivel = kv.Value }));

        db.BitacoraAccesos.Add(new BitacoraAcceso
        {
            Autor = autorNombre,
            UsuarioAfectado = $"{e.Nombre} ({e.Email})",
            Accion = esNuevo ? "Creó usuario" : "Modificó usuario",
            Detalle = await DescribirCambiosAsync(anterior, e)
        });
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        PermisosService.Invalidar(cache, u.Id);
        return [];
    }

    private static void Copiar(EdicionUsuario e, ApplicationUser u)
    {
        u.NombreCompleto = e.Nombre;
        u.Cargo = string.IsNullOrWhiteSpace(e.Cargo) ? null : e.Cargo.Trim();
        u.Activo = e.Activo;
        u.PerfilAccesoId = e.PerfilAccesoId;
        u.PermisosConfigurados = true; // desde que se guarda en el panel, manda la matriz
    }

    private static List<string> Traducir(IdentityResult r) => r.Errors.Select(x => x.Code switch
    {
        "PasswordTooShort" => "La contraseña debe tener al menos 8 caracteres.",
        "PasswordRequiresUpper" => "La contraseña debe tener al menos una mayúscula.",
        "PasswordRequiresDigit" => "La contraseña debe tener al menos un número.",
        "PasswordRequiresLower" => "La contraseña debe tener al menos una minúscula.",
        "DuplicateUserName" or "DuplicateEmail" => "Ya existe un usuario con ese correo.",
        "InvalidEmail" or "InvalidUserName" => "El correo no es válido.",
        _ => x.Description
    }).Distinct().ToList();

    // Texto legible de lo que cambió, para la bitácora.
    private async Task<string> DescribirCambiosAsync(EdicionUsuario? antes, EdicionUsuario despues)
    {
        var nombres = await db.Proyectos.AsNoTracking().ToDictionaryAsync(p => p.Id, p => p.Nombre);
        string Proy(int id) => nombres.GetValueOrDefault(id, $"Proyecto {id}");
        string Mod(string clave) => ModulosAcceso.DeProyecto.Concat(ModulosAcceso.Corporativos)
            .FirstOrDefault(m => m.Clave == clave)?.Nombre ?? clave;

        var c = new List<string>();
        antes ??= new EdicionUsuario { Activo = !despues.Activo, Configurado = false };
        if (antes.Cargo != despues.Cargo && despues.Id is not null && antes.Id is not null) c.Add($"Cargo: {antes.Cargo ?? "—"} → {despues.Cargo ?? "—"}");
        if (antes.Activo != despues.Activo) c.Add(despues.Activo ? "Activo" : "Desactivado");
        if (antes.EsAdministrador != despues.EsAdministrador) c.Add(despues.EsAdministrador ? "Ahora es administrador" : "Ya no es administrador");
        if (!string.IsNullOrWhiteSpace(despues.Contrasena) && antes.Id is not null) c.Add("Contraseña cambiada");
        if (!antes.Configurado) c.Add("Permisos configurados por primera vez");

        foreach (var p in despues.Proyectos.Except(antes.Proyectos)) c.Add($"+ Proyecto {Proy(p)}");
        foreach (var p in antes.Proyectos.Except(despues.Proyectos)) c.Add($"− Proyecto {Proy(p)}");

        var claves = antes.PorProyecto.Keys.Union(despues.PorProyecto.Keys)
            .Where(k => despues.Proyectos.Contains(k.ProyectoId));
        foreach (var k in claves)
        {
            var a = antes.Proyectos.Contains(k.ProyectoId) ? antes.NivelProyecto(k.ProyectoId, k.Modulo) : NivelAcceso.Ninguno;
            var d = despues.NivelProyecto(k.ProyectoId, k.Modulo);
            if (a != d) c.Add($"{Proy(k.ProyectoId)} · {Mod(k.Modulo)}: {ModulosAcceso.NombreNivel(a)} → {ModulosAcceso.NombreNivel(d)}");
        }
        foreach (var m in antes.Corporativos.Keys.Union(despues.Corporativos.Keys))
        {
            var a = antes.NivelCorporativo(m);
            var d = despues.NivelCorporativo(m);
            if (a != d) c.Add($"{Mod(m)}: {ModulosAcceso.NombreNivel(a)} → {ModulosAcceso.NombreNivel(d)}");
        }

        var texto = c.Count == 0 ? "Sin cambios de permisos" : string.Join("; ", c);
        return texto.Length > 3900 ? texto[..3900] + "…" : texto;
    }

    // ── Perfiles ─────────────────────────────────────────────────────────────

    public async Task<List<EdicionPerfil>> PerfilesAsync() =>
        (await db.PerfilesAcceso.AsNoTracking().Include(p => p.Permisos).OrderBy(p => p.Nombre).ToListAsync())
        .Select(p => new EdicionPerfil
        {
            Id = p.Id, Nombre = p.Nombre, Descripcion = p.Descripcion,
            Niveles = p.Permisos.ToDictionary(x => x.Modulo, x => x.Nivel)
        }).ToList();

    public async Task<string?> GuardarPerfilAsync(EdicionPerfil e, string autorNombre)
    {
        e.Nombre = e.Nombre.Trim();
        if (string.IsNullOrWhiteSpace(e.Nombre)) return "Escribe el nombre del perfil.";
        if (await db.PerfilesAcceso.AnyAsync(p => p.Nombre == e.Nombre && p.Id != e.Id))
            return "Ya existe un perfil con ese nombre.";

        var perfil = e.Id == 0 ? new PerfilAcceso() : await db.PerfilesAcceso.Include(p => p.Permisos).FirstAsync(p => p.Id == e.Id);
        perfil.Nombre = e.Nombre;
        perfil.Descripcion = string.IsNullOrWhiteSpace(e.Descripcion) ? null : e.Descripcion.Trim();
        perfil.FechaModificacion = DateTime.UtcNow;
        db.PermisosPerfil.RemoveRange(perfil.Permisos);
        perfil.Permisos = e.Niveles.Where(kv => kv.Value > NivelAcceso.Ninguno)
            .Select(kv => new PermisoPerfil { Modulo = kv.Key, Nivel = kv.Value }).ToList();
        if (e.Id == 0) db.PerfilesAcceso.Add(perfil);

        db.BitacoraAccesos.Add(new BitacoraAcceso
        {
            Autor = autorNombre,
            Accion = e.Id == 0 ? "Creó perfil" : "Modificó perfil",
            Detalle = $"{perfil.Nombre}: " + (perfil.Permisos.Count == 0 ? "sin módulos" : string.Join(", ",
                perfil.Permisos.Select(p => $"{ModulosAcceso.DeProyecto.Concat(ModulosAcceso.Corporativos).FirstOrDefault(m => m.Clave == p.Modulo)?.Nombre ?? p.Modulo} {ModulosAcceso.NombreNivel(p.Nivel)}")))
        });
        await db.SaveChangesAsync();
        return null;
    }

    // Borrar un perfil no cambia los permisos de quienes lo usaron (ya se copiaron).
    public async Task EliminarPerfilAsync(int id, string autorNombre)
    {
        var p = await db.PerfilesAcceso.FindAsync(id);
        if (p is null) return;
        await db.Users.Where(u => u.PerfilAccesoId == id).ExecuteUpdateAsync(s => s.SetProperty(u => u.PerfilAccesoId, (int?)null));
        db.PerfilesAcceso.Remove(p);
        db.BitacoraAccesos.Add(new BitacoraAcceso { Autor = autorNombre, Accion = "Eliminó perfil", Detalle = p.Nombre });
        await db.SaveChangesAsync();
    }

    // Perfiles de partida que la administradora puede crear con un clic y luego ajustar.
    public async Task CrearPerfilesSugeridosAsync(string autorNombre)
    {
        const NivelAcceso V = NivelAcceso.Ver, E = NivelAcceso.Editar, A = NivelAcceso.Aprobar;
        var sugeridos = new (string Nombre, string Desc, Dictionary<string, NivelAcceso> N)[]
        {
            ("Gerente de proyecto", "Todo el proyecto con aprobación; Compromisos para editar.",
                ModulosAcceso.DeProyecto.ToDictionary(m => m.Clave, _ => A)
                    .Concat(new Dictionary<string, NivelAcceso> { [ModulosAcceso.Compromisos] = E }).ToDictionary()),
            ("Residente de obra", "Informes y cronograma para editar; el resto en consulta, sin costos ni HSEQ.",
                new() { [ModulosAcceso.Dashboard] = V, [ModulosAcceso.Informes] = E, [ModulosAcceso.Cronograma] = E,
                        [ModulosAcceso.Documentos] = V, [ModulosAcceso.Restricciones] = E, [ModulosAcceso.Histogramas] = V,
                        [ModulosAcceso.Clima] = V, [ModulosAcceso.Alertas] = V }),
            ("HSE de obra", "Las cuatro áreas HSEQ para editar; informes y clima en consulta.",
                new() { [ModulosAcceso.Dashboard] = V, [ModulosAcceso.Informes] = V, [ModulosAcceso.HseqSeguridad] = E,
                        [ModulosAcceso.HseqCalidad] = E, [ModulosAcceso.HseqAmbiental] = E, [ModulosAcceso.HseqSocial] = E,
                        [ModulosAcceso.Clima] = V, [ModulosAcceso.Alertas] = V, [ModulosAcceso.HseqGlobal] = V }),
            ("Control de costos", "Costos e histogramas para editar; avance en consulta.",
                new() { [ModulosAcceso.Dashboard] = V, [ModulosAcceso.Costos] = E, [ModulosAcceso.Histogramas] = E,
                        [ModulosAcceso.Cronograma] = V }),
            ("Consulta gerencia", "Todo en solo consulta, incluidos Compromisos.",
                ModulosAcceso.DeProyecto.Concat(ModulosAcceso.Corporativos).Where(m => m.Clave != ModulosAcceso.Helvis)
                    .ToDictionary(m => m.Clave, _ => V)),
        };
        var existentes = await db.PerfilesAcceso.Select(p => p.Nombre).ToListAsync();
        foreach (var s in sugeridos.Where(s => !existentes.Contains(s.Nombre)))
            await GuardarPerfilAsync(new EdicionPerfil { Nombre = s.Nombre, Descripcion = s.Desc, Niveles = s.N }, autorNombre);
    }

    public Task<List<BitacoraAcceso>> BitacoraAsync(int maximo = 300) =>
        db.BitacoraAccesos.AsNoTracking().OrderByDescending(b => b.FechaCreacion).Take(maximo).ToListAsync();

    public async Task<List<(int Id, string Nombre)>> ProyectosAsync() =>
        (await db.Proyectos.AsNoTracking().OrderBy(p => p.Nombre).Select(p => new { p.Id, p.Nombre }).ToListAsync())
        .Select(p => (p.Id, p.Nombre)).ToList();
}
