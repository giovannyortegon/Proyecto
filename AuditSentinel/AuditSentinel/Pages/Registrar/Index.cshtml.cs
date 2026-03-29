using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Usuarios
{ 
    [Authorize(Roles = "Administrador")]

    public class IndexModel : PageModel
    {
        private readonly UserManager<AuditSentinel.Models.Usuarios> _userManager;

        public IndexModel(UserManager<AuditSentinel.Models.Usuarios> userManager)
        {
            _userManager = userManager;
        }

        public List<AuditSentinel.Models.Registro> Users { get; set; } = new();

        public bool IsEmpty => Users.Count == 0;

        public async Task<IActionResult> OnGetAsync()
        {
            var all = await Task.FromResult(_userManager.Users.ToList());

            if (all == null || !all.Any())
                return Page();

            foreach (var u in all)
            {
                Users.Add(new AuditSentinel.Models.Registro
                {
                    Id       = u.Id,
                    Nombre   = u.Nombre   ?? string.Empty,
                    Apellido = u.Apellido ?? string.Empty,
                    UserName = u.UserName ?? string.Empty,
                    Email    = u.Email,
                    Rol      = (await _userManager.GetRolesAsync(u)).ToList()
                });
            }

            return Page();
        }
    }
}