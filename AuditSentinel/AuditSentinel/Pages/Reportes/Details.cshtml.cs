
using AuditSentinel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Reportes
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public DetailsModel(ApplicationDBContext context) => _context = context;

        public AuditSentinel.Models.Reportes Reporte { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Reporte = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .ThenInclude(er => er.Escaneos)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (Reporte == null) return NotFound();
            return Page();
        }
    }
}