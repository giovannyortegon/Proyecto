
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Usuarios
{
    [Authorize(Roles = "Administrador")]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ViewModel para edición
        public class EditInputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string UserName { get; set; } = string.Empty;

            public List<string> Roles { get; set; } = new();
        }

        [BindProperty] public EditInputModel Input { get; set; } = new();
        [BindProperty] public string UserId { get; set; } = string.Empty;

        public List<SelectListItem> AvailableRoles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            UserId = user.Id;
            Input.Email = user.Email ?? string.Empty;
            Input.UserName = user.UserName ?? string.Empty;
            Input.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            AvailableRoles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null) return NotFound();

            //if (!ModelState.IsValid)
            //{
                
            //    return Page();
            //}
            //AvailableRoles = _roleManager.Roles
            //        .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            //        .ToList();

            // Actualiza Email/UserName
            user.Email = Input.Email;
            user.UserName = Input.UserName;

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
            {
                foreach (var e in update.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                AvailableRoles = _roleManager.Roles
                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                    .ToList();
                return Page();
            }

            // Actualiza roles: quita los que ya no están y agrega los nuevos
            var currentRoles = await _userManager.GetRolesAsync(user);
            var toAdd = Input.Roles.Except(currentRoles).ToArray();
            var toRemove = currentRoles.Except(Input.Roles).ToArray();

            if (toRemove.Length > 0)
            {
                var r1 = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!r1.Succeeded)
                {
                    foreach (var e in r1.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return Page();
                }
            }

            if (toAdd.Length > 0)
            {
                // Crea roles inexistentes (por si acaso)
                foreach (var roleName in toAdd)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                var r2 = await _userManager.AddToRolesAsync(user, toAdd);
                if (!r2.Succeeded)
                {
                    foreach (var e in r2.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    return Page();
                }
            }

            return RedirectToPage("Index");
        }
    }
}

