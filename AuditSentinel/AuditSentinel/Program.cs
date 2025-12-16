using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;




var builder = WebApplication.CreateBuilder(args);
// Configuracion de la cadena de conexion
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//Agregar soporte para identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Opcional: políticas
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

//
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccessDenied";
});



// Add services to the container.
builder.Services.AddRazorPages();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Administrador", "Analista", "Auditor" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}



if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithRedirects("/Error/{0}");

}
else
{
    // Manejo de excepciones 500 (redirige a /Error/500)
    app.UseExceptionHandler("Pages/Error/500");
    // Manejo de códigos de estado (ej. 404, 403, 401). Re-ejecuta la ruta /Error/{codigo}
    app.UseStatusCodePagesWithReExecute(pathFormat: """Pages/Error/{0}""");
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
