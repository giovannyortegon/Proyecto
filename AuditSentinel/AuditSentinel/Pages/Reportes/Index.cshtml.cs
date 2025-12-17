
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSentinel.Pages.Reportes
{
    [Authorize(Roles = "Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public IndexModel(ApplicationDBContext context) => _context = context;

        // Filtros
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public Cumplimiento? Cumplimiento { get; set; }

        // Datos
        public IList<AuditSentinel.Models.Reportes> Items { get; set; } = new List<AuditSentinel.Models.Reportes>();

        // Paginación
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync(string? search, Cumplimiento? cumplimiento)
        {
            // Mantener compatibilidad con parámetros explícitos (opcional)
            Search = search ?? Search;
            Cumplimiento = cumplimiento ?? Cumplimiento;

            // Base query con relaciones y sin tracking
            var query = _context.Reportes
                .Include(r => r.EscaneosReportes)
                .AsNoTracking()
                .AsQueryable();

            // Filtro: búsqueda por nombre (extiende a Descripcion si existe)
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var s = Search.Trim();
                query = query.Where(r => r.NombreReporte.Contains(s));
                // Si tu modelo tiene Descripcion:
                // query = query.Where(r => r.NombreReporte.Contains(s) || r.Descripcion.Contains(s));
            }

            // Filtro: cumplimiento
            if (Cumplimiento.HasValue)
            {
                query = query.Where(r => r.cumplimiento == Cumplimiento.Value);
            }

            // Total y páginas
            TotalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            // Orden + paginación aplicados sobre el MISMO query (con filtros)
            Items = await query
                .OrderByDescending(r => r.Creado)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}