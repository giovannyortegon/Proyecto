
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<AuditSentinel.Models.Usuarios> _userManager;

        public DeleteModel(UserManager<AuditSentinel.Models.Usuarios> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty] public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            UserId = user.Id;
            UserName = user.UserName ?? user.Email ?? user.Id;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null) return NotFound();

            // Evitar que el administrador se elimine a sí mismo
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == user.Id)
            {
                ErrorMessage = "No puedes eliminar tu propia cuenta.";
                UserName = user.UserName ?? user.Email ?? user.Id;
                return Page();
            }

            // (Opcional) Evitar eliminar el último Administrador
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Administrador"))
            {
                var allUsers = _userManager.Users.ToList();
                int adminCount = 0;
                foreach (var u in allUsers)
                {
                    var r = await _userManager.GetRolesAsync(u);
                    if (r.Contains("Administrador")) adminCount++;
                }

                if (adminCount <= 1)
                {
                    ErrorMessage = "No se puede eliminar el único Administrador del sistema.";
                    UserName = user.UserName ?? user.Email ?? user.Id;
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                UserName = user.UserName ?? user.Email ?? user.Id;
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
