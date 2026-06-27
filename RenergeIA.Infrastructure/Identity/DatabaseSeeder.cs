using Microsoft.AspNetCore.Identity;

namespace RenergeIA.Infrastructure.Identity;

public static class DatabaseSeeder
{
    public static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        var roles = new[]
        {
            Roles.Administrador, Roles.DirectorGeneral, Roles.GerenteProyecto,
            Roles.IngenierosResidente, Roles.InspectorCalidad, Roles.CoordinadorHSE,
            Roles.AdministradorContrato, Roles.JefeAlmacen, Roles.SupervisorCampo,
            Roles.ControlCostos, Roles.Documentador, Roles.Consultor, Roles.Subcontratista
        };

        foreach (var rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        const string adminEmail = "admin@renergeia.com";
        const string adminPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                NombreCompleto = "Administrador del Sistema",
                Cargo = "Administrador",
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(admin, adminPassword);
            if (resultado.Succeeded)
                await userManager.AddToRoleAsync(admin, Roles.Administrador);
        }

        const string luisaEmail = "luisabecerra22@gmail.com";
        const string luisaPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(luisaEmail) is null)
        {
            var luisa = new ApplicationUser
            {
                UserName = luisaEmail,
                Email = luisaEmail,
                NombreCompleto = "Luisa Becerra",
                Cargo = "Administradora",
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(luisa, luisaPassword);
            if (resultado.Succeeded)
                await userManager.AddToRoleAsync(luisa, Roles.Administrador);
        }
    }
}
