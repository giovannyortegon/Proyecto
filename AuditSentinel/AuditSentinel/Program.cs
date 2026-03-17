using AuditSentinel.Data;
using AuditSentinel.Hubs;
using AuditSentinel.Models;
using AuditSentinel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// CONEXIÓN A BASE DE DATOS
// ==========================
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ==========================
// IDENTITY
// ==========================
builder.Services.AddIdentity<Usuarios, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

// ==========================
// COOKIES
// ==========================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;

    options.Cookie.Name = "AuthCookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // usa HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax; // o Strict si tu flujo lo permi
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // Opcional: configurar el nombre del header
});

// ==========================
// SERVICIOS
// ==========================
builder.Services.AddSignalR();
builder.Services.AddHostedService<ScannerServerService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<GraficaService>();
builder.Services.AddScoped<ExportService>();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole();

builder.Services.AddRazorPages();

// ==========================
// LICENCIA QUESTPDF
// ==========================
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();


// ==========================
// CREACIÓN AUTOMÁTICA DE ROLES
// ==========================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Administrador", "Analista", "Auditor" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errores = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                Console.WriteLine($"Error creando rol {role}: {errores}");
            }
        }
    }
}


// ==========================
// MANEJO DE ERRORES (CORREGIDO)
// ==========================
if (app.Environment.IsDevelopment())
{
    // En desarrollo: página de excepción detallada del framework
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}
else
{
    //En producción: páginas de error personalizadas
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();


app.MapGet("/Cuenta/MantenerActivo", [Authorize] () =>
{
    // Devolver algo mínimo. El simple hecho de llegar aquí ya cuenta como actividad.
    return Results.Ok(new { utc = DateTimeOffset.UtcNow });
});


app.MapHub<EscaneoHub>("/EscaneoHub");
app.MapRazorPages();

app.Run();