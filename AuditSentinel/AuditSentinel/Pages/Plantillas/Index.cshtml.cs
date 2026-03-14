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

        public async Task<IActionResult> OnGetAsync()
        {

            var query = _context.Plantillas
                .Include(p => p.PlantillasVulnerabilidades)
                .AsQueryable();

            // Filtrado seguro para Enums
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var searchLower = Search.ToLower();
                // Filtramos por Versión en SQL y traemos a memoria lo necesario para el Enum si es complejo,
                // o usamos una aproximación compatible con SQL.
                query = query.Where(p => p.Version.Contains(Search));

                // Nota: Si necesitas buscar específicamente por el NOMBRE del Enum (Linux, Windows...), 
                // lo ideal es convertir el Search a una lista de Enums que coincidan y filtrar por ID.
            }

            TotalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Ajustes de rango de página
            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            Items = await query
                .OrderBy(p => p.NombrePlantilla)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}