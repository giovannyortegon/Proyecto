using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Cuenta
{

    public class LogoutInactividadModel : PageModel
    {
        private readonly SignInManager<AuditSentinel.Models.Usuarios> _signInManager;

        public LogoutInactividadModel(SignInManager<AuditSentinel.Models.Usuarios> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet() { }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToPage("/Index");
        }
    }
}

