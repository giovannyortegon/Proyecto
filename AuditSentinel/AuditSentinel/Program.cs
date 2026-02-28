using AuditSentinel.Data;
using AuditSentinel.Models;
using AuditSentinel.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure; 

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Agregar soporte para identity
builder.Services.AddIdentity<Usuarios, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

// Configuración de cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccessDenied";
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole();

// Razor Pages
builder.Services.AddRazorPages();

// ⚡ Configuración de QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddSingleton<INmapScannerService, NmapScannerService>();

var app = builder.Build();

// Seed de roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Administrador", "Analista", "Auditor" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var create = await roleManager.CreateAsync(new IdentityRole(role));
            if (!create.Succeeded)
            {
                var errs = string.Join("; ", create.Errors.Select(e => $"{e.Code}: {e.Description}"));
                Console.WriteLine($"[SEED ROLES] Error creando rol '{role}': {errs}");
            }
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithRedirects("/Error/{0}");
}
else
{
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();