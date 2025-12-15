
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Cuenta
{
    // Opcional: si quieres que solo usuarios autenticados vean la página
    // [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LogoutModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // No hacemos logout por GET para evitar CSRF. Solo mostramos la vista con el botón.
        public void OnGet() { }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            // Si usas cookies o caches adicionales, puedes limpiarlos aquí.

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return LocalRedirect(returnUrl);

            // Redirige a inicio luego de cerrar sesión
            return RedirectToPage("/Index");
        }
    }
}

