
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Vulnerabilidades
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public IndexModel(ApplicationDBContext context) => _context = context;

        public IList<AuditSentinel.Models.Vulnerabilidades> Items { get; set; } = new List<AuditSentinel.Models.Vulnerabilidades>();

        // (Opcional) filtros simples
        public string? Search { get; set; }
        public NivelRiesgo? Riesgo { get; set; }

        public async Task OnGetAsync(string? search, NivelRiesgo? riesgo)
        {
            Search = search;
            Riesgo = riesgo;

            var query = _context.Vulnerabilidades.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(v => v.NombreVulnerabilidad.Contains(Search) || (v.Descripcion != null && v.Descripcion.Contains(Search)));

            if (Riesgo.HasValue)
                query = query.Where(v => v.NivelRiesgo == Riesgo.Value);

            Items = await query
                .OrderByDescending(v => v.FechaDetectada)
                .ToListAsync();
        }
    }
}