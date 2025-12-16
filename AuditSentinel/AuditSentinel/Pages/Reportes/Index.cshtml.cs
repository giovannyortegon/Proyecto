
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Reportes
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public IndexModel(ApplicationDBContext context) => _context = context;

        public IList<AuditSentinel.Models.Reportes> Items { get; set; } = new List<AuditSentinel.Models.Reportes>();

        public async Task OnGetAsync()
        {
            Items = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .OrderByDescending(r => r.Creado)
                .ToListAsync();
        }
    }
}
