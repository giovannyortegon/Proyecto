
using AuditSentinel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Vulnerabilidades
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public DeleteModel(ApplicationDBContext context) => _context = context;

        [BindProperty] public AuditSentinel.Models.Vulnerabilidades Vulnerabilidad { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vulnerabilidad = await _context.Vulnerabilidades
                .Include(v => v.EscaneosVulnerabilidades)
                .Include(v => v.PlantillasVulnerabilidades)
                .FirstOrDefaultAsync(v => v.IdVulnerabilidad == id);

            if (Vulnerabilidad == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var entity = await _context.Vulnerabilidades
                .Include(v => v.EscaneosVulnerabilidades)
                .Include(v => v.PlantillasVulnerabilidades)
                .FirstOrDefaultAsync(v => v.IdVulnerabilidad == id);

            if (entity == null) return NotFound();

            // Si tus relaciones están con DeleteBehavior.Cascade (como en tu DbContext),
            // no necesitas borrar manualmente los vínculos; el DB los elimina automáticamente.
            // En tu ApplicationDBContext las relaciones puente se configuraron con Cascade. [2](https://poligran-my.sharepoint.com/personal/giortegon1_poligran_edu_co/Documents/Archivos%20de%20Microsoft%C2%A0Copilot%20Chat/ApplicationDBContext.cs)

            _context.Vulnerabilidades.Remove(entity);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"No se pudo eliminar. Detalle: {ex.Message}");
                return Page();
            }
        }
    }
}

