using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Escaneos
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public DetailsModel(ApplicationDBContext context)
        {
            _context = context;
        }

        public AuditSentinel.Models.Escaneos Escaneo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Escaneo = await _context.Escaneos
                .Include(e => e.EscaneosServidores)
                    .ThenInclude(es => es.Servidores)
                .Include(e => e.EscaneosPlantillas)
                    .ThenInclude(ep => ep.Plantillas)
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (Escaneo == null)
                return NotFound();

            return Page();
        }
    }
}
