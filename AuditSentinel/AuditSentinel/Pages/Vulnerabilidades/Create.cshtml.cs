
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Vulnerabilidades
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public CreateModel(ApplicationDBContext context) => _context = context;

        [BindProperty] public AuditSentinel.Models.Vulnerabilidades Vulnerabilidad { get; set; } = new();

        public void OnGet()
        {
            // FechaDetectada ya tiene default en el modelo
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Vulnerabilidades.Add(Vulnerabilidad);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}

