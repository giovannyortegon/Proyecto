using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Cuenta {
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AuditSentinel.Models.Usuarios> _signInManager;

        public LoginModel(SignInManager<AuditSentinel.Models.Usuarios> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public AuditSentinel.Models.Login Login { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Login.Email,
                Login.Password,
                true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Credenciales incorrectas";
            return Page();
        }
    }
}