
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Usuarios
{
    //[Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserListItem> Users { get; set; } = new();

        public class UserListItem
        {
            public string Id { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public List<string> Roles { get; set; } = new();
        }

        public async Task OnGetAsync()
        {
            var all = _userManager.Users.ToList();
            foreach (var u in all)
            {
                Users.Add(new UserListItem
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email,
                    Roles = (await _userManager.GetRolesAsync(u)).ToList()
                });
            }
        }
    }
}
