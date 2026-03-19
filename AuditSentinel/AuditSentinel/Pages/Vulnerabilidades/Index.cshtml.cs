
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Vulnerabilidades
{
    [Authorize(Roles = "Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public IndexModel(ApplicationDBContext context) => _context = context;

        public IList<AuditSentinel.Models.Vulnerabilidades> Items { get; set; } = new List<AuditSentinel.Models.Vulnerabilidades>();

        // (Opcional) filtros simples
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }
        [BindProperty(SupportsGet = true)]
        public NivelRiesgo? Riesgo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;   // <<< 5 por página

        // Datos para la UI
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            //Search = search;
            //Riesgo = riesgo;

            var query = _context.Vulnerabilidades.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(v => v.NombreVulnerabilidad.Contains(Search));

            //if (Riesgo.HasValue) {
            //    query = query.Where(vr => vr.NivelRiesgo == Riesgo.Value);
            //    Console.WriteLine($"Riesgo recibido: {Riesgo.Value}");
            //}

            // Total de registros (para calcular páginas)
            TotalItems = await query.CountAsync();

            // Calcular total de páginas
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Corregir PageNumber fuera de rango
            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            Items = await query
                .OrderByDescending(v => v.FechaDetectada)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}