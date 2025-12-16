
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Plantillas
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public IndexModel(ApplicationDBContext context) => _context = context;

        public IList<AuditSentinel.Models.Plantillas> Items { get; set; } = new List<AuditSentinel.Models.Plantillas>();

        public async Task OnGetAsync()
        {
            Items = await _context.Plantillas
                .Include(p => p.PlantillasVulnerabilidades)
                .OrderBy(p => p.NombrePlantilla)
                .ToListAsync();
        }
    }
}
