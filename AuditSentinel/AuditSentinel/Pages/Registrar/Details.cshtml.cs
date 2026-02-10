
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
        private readonly UserManager<AuditSentinel.Models.Usuarios> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DetailsModel(UserManager<AuditSentinel.Models.Usuarios> userManager, RoleManager<IdentityRole> roleManager   )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public AuditSentinel.Models.Registro Registro { get; set; } = new();

        //public string UserId { get; private set; } = string.Empty;
        //public string UserName { get; private set; } = string.Empty;
        //public string? Email { get; private set; }
        //public string[] Roles { get; private set; } = [];

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            Registro.Id = user.Id;
            Registro.Nombre = user.Nombre ?? string.Empty;
            Registro.Apellido = user.Apellido?? string.Empty;
            Registro.UserName = user.UserName ?? string.Empty;
            Registro.Email = user.Email;
            Registro.Rol = (List<string>)await _userManager.GetRolesAsync(user);
            Registro.FechaCreado = user.FechaCreado;
        
            return Page();
        }
    }
}
