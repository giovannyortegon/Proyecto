
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Plantillas
{
    [Authorize(Roles = "Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public IndexModel(ApplicationDBContext context) => _context = context;

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public IList<AuditSentinel.Models.Plantillas> Items { get; set; } = new List<AuditSentinel.Models.Plantillas>();

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync(string? search)
        {
            Search = search;

            // Base query
            var query = _context.Plantillas
                .Include(p => p.PlantillasVulnerabilidades)
                .AsQueryable();

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(p => p.NombrePlantilla.Contains(Search) || p.Version.Contains(Search));

            // Calcular total de registros
            TotalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            // Aplicar paginación y orden
            Items = await query
                .OrderBy(p => p.NombrePlantilla)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}
