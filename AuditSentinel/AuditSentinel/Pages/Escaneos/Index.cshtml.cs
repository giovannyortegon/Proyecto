using AuditSentinel.Data;
using AuditSentinel.Models;
using AuditSentinel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Alias para evitar conflicto de nombres
using EscaneoModel = AuditSentinel.Models.Escaneos;

namespace AuditSentinel.Pages.Escaneos
{
    [Authorize(Roles = "Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public IndexModel(ApplicationDBContext context)
        {
            _context = context;
        }

        public string? Search { get; set; }
        public EstadoEscaneo? BEstado { get; set; }

        public IList<EscaneoModel> ListaEscaneos { get; set; } = new List<EscaneoModel>();

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string? search, EstadoEscaneo? estado)
        {
            Search = search;
            BEstado = estado;

            var query = _context.Escaneos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(e => e.NombreEscaneo.Contains(Search));

            if (BEstado.HasValue)
                query = query.Where(ee => ee.Estado == BEstado.Value);

            TotalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            ListaEscaneos = await query
                .OrderByDescending(e => e.FechaEscaneo)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetExportAsync(string format)
        {
            var query = _context.Escaneos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(e => e.NombreEscaneo.Contains(Search));

            if (BEstado.HasValue)
                query = query.Where(ee => ee.Estado == BEstado.Value);

            var results = await query.OrderByDescending(e => e.FechaEscaneo).ToListAsync();

            var service = new ExportService();
            var filePath = Path.Combine(Path.GetTempPath(), $"Escaneos.{format}");

            switch (format.ToLower())
            {
                case "csv":
                    service.ExportToCsv(results, filePath);
                    break;
                case "html":
                    service.ExportToHtml(results, filePath);
                    break;
                case "pdf":
                    service.ExportToPdf(results, filePath);
                    break;
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", $"Escaneos.{format}");
        }
    }
}