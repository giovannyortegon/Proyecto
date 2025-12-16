using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Plantillas
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public DetailsModel(ApplicationDBContext context) => _context = context;

        public AuditSentinel.Models.Plantillas Plantilla { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Plantilla = await _context.Plantillas
                .Include(p => p.PlantillasVulnerabilidades)
                .ThenInclude(pv => pv.Vulnerabilidades)
                .FirstOrDefaultAsync(p => p.IdPlantilla == id);

            if (Plantilla == null) return NotFound();
            return Page();
        }
    }
}