
using AuditSentinel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Vulnerabilidades
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public DetailsModel(ApplicationDBContext context) => _context = context;

        public AuditSentinel.Models.Vulnerabilidades Vulnerabilidad { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vulnerabilidad = await _context.Vulnerabilidades
                .Include(v => v.EscaneosVulnerabilidades)
                .ThenInclude(ev => ev.Escaneos)
                .Include(v => v.PlantillasVulnerabilidades)
                .ThenInclude(pv => pv.Plantillas)
                .FirstOrDefaultAsync(v => v.IdVulnerabilidad == id);

            if (Vulnerabilidad == null) return NotFound();
            return Page();
        }
    }
}