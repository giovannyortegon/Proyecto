using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AuditSentinel.Pages.Registrar
{
    //[Authorize(Roles = "Administrador")]
    public class RegistrarModel : PageModel
    {
        private readonly UserManager<AuditSentinel.Models.Usuarios> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegistrarModel(UserManager<AuditSentinel.Models.Usuarios> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public AuditSentinel.Models.Registro Registro { get; set; } = new();

        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validación de modelo
            if (!ModelState.IsValid)
                return Page();

            // Validar password explícitamente (evita ArgumentNullException)
            if (string.IsNullOrWhiteSpace(Registro.Password))
            {
                ModelState.AddModelError(nameof(Registro.Password), "La contraseña es obligatoria.");
                return Page();
            }

            //Validar Email/ Usuario
            if (string.IsNullOrWhiteSpace(Registro.Email))
            {
                ModelState.AddModelError(nameof(Registro.Email), "El email es obligatorio.");
                return Page();
            }

            // Crear usuario
            var user = new AuditSentinel.Models.Usuarios
            {   
                Nombre = Registro.Nombre,
                Apellido = Registro.Apellido,
                UserName = Registro.Email,
                Email = Registro.Email,
                EmailConfirmed = true // Opcional: confirma email de inmediato
            };

            var createResult = await _userManager.CreateAsync(user, Registro.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            // Asegurar que el rol exista antes de asignarlo
            //if (!string.IsNullOrWhiteSpace(Registro.Rol))
            //{
            //    var roleExists = await _roleManager.RoleExistsAsync(Registro.Rol);
            //    if (!roleExists)
            //    {
            //        var createRole = await _roleManager.CreateAsync(new IdentityRole(Registro.Rol));
            //        if (!createRole.Succeeded)
            //        {
            //            // Si no se pudo crear el rol, revertir usuario o notificar
            //            foreach (var error in createRole.Errors)
            //                ModelState.AddModelError(string.Empty, $"Error creando rol '{Registro.Rol}': {error.Description}");

            //            return Page();
            //        }
            //    }

            //    var addToRoleResult = await _userManager.AddToRoleAsync(user, Registro.Rol);
            //    if (!addToRoleResult.Succeeded)
            //    {
            //        foreach (var error in addToRoleResult.Errors)
            //            ModelState.AddModelError(string.Empty, $"Error asignando rol '{Registro.Rol}': {error.Description}");

            //        return Page();
            //    }
            //}


            // 3) Asignación de Roles (lista)
            //    - Limpia nulos/espacios
            var rolesSolicitados = (Registro.Rol ?? new List<string>())
                                  .Where(r => !string.IsNullOrWhiteSpace(r))
                                  .Select(r => r.Trim())
                                  .Distinct()
                                  .ToList();

            if (rolesSolicitados.Count > 0)
            {
                // 3.1) Asegurar que cada rol exista
                foreach (var rol in rolesSolicitados)
                {
                    var exists = await _roleManager.RoleExistsAsync(rol);
                    if (!exists)
                    {
                        var createRole = await _roleManager.CreateAsync(new IdentityRole(rol));
                        if (!createRole.Succeeded)
                        {
                            foreach (var error in createRole.Errors)
                                ModelState.AddModelError(string.Empty, $"Error creando rol '{rol}': {error.Description}");
                            return Page();
                        }
                    }
                }

                // 3.2) Agregar todos los roles de una vez (AddToRolesAsync) o en bucle
                var addToRolesResult = await _userManager.AddToRolesAsync(user, rolesSolicitados);
                if (!addToRolesResult.Succeeded)
                {
                    foreach (var error in addToRolesResult.Errors)
                        ModelState.AddModelError(string.Empty, $"Error asignando roles: {error.Description}");
                    return Page();
                }
            }


            SuccessMessage = "Usuario creado correctamente";
            ModelState.Clear(); // Limpia el estado si quieres reiniciar el formulario
            Registro = new();   // Limpia el modelo

            return RedirectToPage("./Index");
        }
    }
}

