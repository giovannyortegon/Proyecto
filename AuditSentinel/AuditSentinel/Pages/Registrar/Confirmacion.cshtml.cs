using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Registrar
{
    [Authorize(Roles = "Administrador")]
    public class ConfirmacionModel : PageModel
    {
        private readonly UserManager<AuditSentinel.Models.Usuarios> _userManager;

        public ConfirmacionModel(UserManager<AuditSentinel.Models.Usuarios> userManager)
        {
            _userManager = userManager;
        }
        public AuditSentinel.Models.Usuarios UsuarioActualizado { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();


            UsuarioActualizado = await _userManager.FindByIdAsync(id);

            if (UsuarioActualizado == null) return NotFound();

            return Page();
        }
    }
}
