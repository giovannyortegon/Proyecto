using AuditSentinel.Data;
using AuditSentinel.Models;
using AuditSentinel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Reportes
{
    //[Authorize(Roles = "Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        private readonly ExportService _exportService;

        public IndexModel(ApplicationDBContext context, ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        // ── Filtros ──────────────────────────────────────────────────────
        [BindProperty(SupportsGet = true)] public string? Search { get; set; }
        [BindProperty(SupportsGet = true)] public Cumplimiento? Cumplimiento { get; set; }

        // ── Tabla + Paginación ───────────────────────────────────────────
        public IList<AuditSentinel.Models.Reportes> Items { get; set; } = new List<AuditSentinel.Models.Reportes>();
        [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;
        public int PageSize   { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        // ── KPIs ─────────────────────────────────────────────────────────
        public int TotalReportes     { get; set; }
        public int TotalEscaneosVinc { get; set; }
        public int ReporteEsteMes    { get; set; }

        // ── Datos gráfica de dona para Chart.js ──────────────────────────
        public string GraficaLabels  { get; set; } = "[]";
        public string GraficaValores { get; set; } = "[]";
        public string GraficaColores { get; set; } = "[]";

        public async Task<IActionResult> OnGetAsync(string? search, Cumplimiento? cumplimiento)
        {
            Search       = search       ?? Search;
            Cumplimiento = cumplimiento ?? Cumplimiento;

            // ── Todos los reportes para KPIs y gráfica ───────────────────
            var todos = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .AsNoTracking()
                .ToListAsync();

            TotalReportes     = todos.Count;
            TotalEscaneosVinc = todos.Sum(r => r.EscaneosReportes?.Count ?? 0);
            ReporteEsteMes    = todos.Count(r => r.Creado.Month == DateTime.Now.Month
                                              && r.Creado.Year  == DateTime.Now.Year);

            // ── Agrupar por cumplimiento para la gráfica ─────────────────
            var grupos = todos
                .GroupBy(r => r.cumplimiento.ToString())
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            var paleta = new[] { "#28a745","#ffc107","#dc3545","#007bff","#6f42c1","#17a2b8","#fd7e14" };

            GraficaLabels  = "[" + string.Join(",", grupos.Select(g => $"\"{g.Label}\""))  + "]";
            GraficaValores = "[" + string.Join(",", grupos.Select(g => g.Count.ToString())) + "]";
            GraficaColores = "[" + string.Join(",", grupos.Select((g, i) => $"\"{paleta[i % paleta.Length]}\"")) + "]";

            // ── Query filtrada para la tabla ─────────────────────────────
            var query = _context.Reportes
                .Include(r => r.EscaneosReportes)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(r => r.NombreReporte.Contains(Search.Trim()));

            if (Cumplimiento.HasValue)
                query = query.Where(r => r.cumplimiento == Cumplimiento.Value);

            TotalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;

            Items = await query
                .OrderByDescending(r => r.Creado)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        // ── Exportar ─────────────────────────────────────────────────────
        public async Task<IActionResult> OnGetExportarAsync(string format)
        {
            var items = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .AsNoTracking()
                .OrderByDescending(r => r.Creado)
                .ToListAsync();

            var filePath = Path.Combine(Path.GetTempPath(), $"Reportes.{format}");

            switch (format.ToLower())
            {
                case "csv":
                    _exportService.ExportReportesToCsv(items, filePath);
                    return File(System.IO.File.ReadAllBytes(filePath), "text/csv", "Reportes.csv");
                case "html":
                    _exportService.ExportReportesToHtml(items, filePath);
                    return File(System.IO.File.ReadAllBytes(filePath), "text/html", "Reportes.html");
                case "pdf":
                    _exportService.ExportReportesToPdf(items, filePath);
                    return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", "Reportes.pdf");
                default:
                    return BadRequest("Formato no soportado.");
            }
        }
    }
}