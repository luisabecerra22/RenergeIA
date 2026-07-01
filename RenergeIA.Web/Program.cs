using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RenergeIA.Infrastructure.Data;
using RenergeIA.Infrastructure.Identity;
using RenergeIA.Web.Components;
using RenergeIA.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RenergeIADbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<RenergeIADbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/login";
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<InformeDiarioService>();
builder.Services.AddScoped<DocumentoService>();
builder.Services.AddScoped<CostoService>();
builder.Services.AddScoped<HistogramaService>();
builder.Services.AddScoped<HomeDashboardService>();
builder.Services.AddScoped<ChecklistISO9001Service>();
builder.Services.AddScoped<NormaChecklistService>();
builder.Services.AddScoped<IAInspeccionService>();
builder.Services.AddHttpClient();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await DatabaseSeeder.SeedRolesAndAdminAsync(roleManager, userManager);
}

app.Run();
