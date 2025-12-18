
// Pages/Usuarios/Details.cshtml.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string UserId { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string? Email { get; private set; }
        public string[] Roles { get; private set; } = [];

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            UserId = user.Id;
            UserName = user.UserName ?? string.Empty;
            Email = user.Email;

            var roles = await _userManager.GetRolesAsync(user);
            Roles = roles?.ToArray() ?? [];

            return Page();
        }
    }
}
