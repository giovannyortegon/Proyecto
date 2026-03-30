using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Pages.Registrar
{
    [Authorize(Roles = "Administrador")]
    public class CambiarPasswordModel : PageModel
    {
        private readonly UserManager<AuditSentinel.Models.Usuarios> _userManager;

        public CambiarPasswordModel(UserManager<AuditSentinel.Models.Usuarios> userManager) => _userManager = userManager;

        [BindProperty] public string IdUsuario { get; set; }
        [BindProperty][Required] public string Password { get; set; }
        [BindProperty][Compare("Password")] public string ConfirmacionPassword { get; set; }
        public string UserName { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            IdUsuario = id;
            UserName = user.UserName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByIdAsync(IdUsuario);
            if (user == null) return NotFound();

            // Método administrativo para forzar cambio de clave
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, Password);

            if (result.Succeeded)
            {
                TempData["MensajeExito"] = "Contraseña actualizada con éxito.";
                //return RedirectToPage("./Index");
                return RedirectToPage("./Confirmacion", new { id = user.Id });
            }

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return Page();
        }
    }
}