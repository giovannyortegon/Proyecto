
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Plantillas
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public DeleteModel(ApplicationDBContext context) => _context = context;

        [BindProperty] public AuditSentinel.Models.Plantillas Plantilla { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Plantilla = await _context.Plantillas.FirstOrDefaultAsync(p => p.IdPlantilla == id);
            if (Plantilla == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var entity = await _context.Plantillas.FindAsync(id);
            if (entity != null)
            {
                _context.Plantillas.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}