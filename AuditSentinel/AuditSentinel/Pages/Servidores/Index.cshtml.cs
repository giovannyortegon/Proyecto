
using AuditSentinel.Data; // Ajusta al namespace de tu DbContext
using AuditSentinel.Models; // Ajusta al namespace de tu modelo Servidor
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSentinel.Pages.Servidores
{
    [Authorize(Roles = "Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public IndexModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        // Lista que se mostrará
        public IList<AuditSentinel.Models.Servidores> Servidores { get; set; } = new List<AuditSentinel.Models.Servidores>();

        // Parámetros de búsqueda y paginación
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 6;   // <<< 5 por página

        // Datos para la UI
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Base query
            IQueryable<AuditSentinel.Models.Servidores> query = _context.Servidores.AsNoTracking();

            // Filtro por búsqueda (nombre o descripción o IP, ajusta campos a tu modelo real)
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var s = Search.Trim();
                query = query.Where(x =>
                    x.NombreServidor.Contains(s) ||
                    x.IP.Contains(s));
                // Si tienes Descripcion: x.Descripcion.Contains(s)
            }

            // Total de registros (para calcular páginas)
            TotalItems = await query.CountAsync();

            // Calcular total de páginas
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Corregir PageNumber fuera de rango
            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            // Aplicar orden, salto y toma
            Servidores = await query
                .OrderBy(x => x.NombreServidor) // Ajusta el orden que prefieras
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}

