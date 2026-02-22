using AuditSentinel.Data;
using AuditSentinel.Models;
using AuditSentinel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Escaneos
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public DetailsModel(ApplicationDBContext context)
        {
            _context = context;
        }

        public AuditSentinel.Models.Escaneos Escaneo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Escaneo = await _context.Escaneos
                .Include(e => e.EscaneosServidores)
                    .ThenInclude(es => es.Servidores)
                .Include(e => e.EscaneosPlantillas)
                    .ThenInclude(ep => ep.Plantillas)
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (Escaneo == null)
                return NotFound();

            return Page();
        }

        //  Exportación individual desde Detalles
        public async Task<IActionResult> OnGetExportAsync(int id, string format)
        {
            var escaneo = await _context.Escaneos
                .Include(e => e.EscaneosServidores)
                    .ThenInclude(es => es.Servidores)
                .Include(e => e.EscaneosPlantillas)
                    .ThenInclude(ep => ep.Plantillas)
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (escaneo == null)
                return NotFound();

            var service = new ExportService();
            var filePath = Path.Combine(Path.GetTempPath(), $"Escaneo_{id}.{format}");

            switch (format.ToLower())
            {
                case "csv":
                    service.ExportEscaneoToCsv(escaneo, filePath);
                    break;
                case "html":
                    service.ExportEscaneoToHtml(escaneo, filePath);
                    break;
                case "pdf":
                    service.ExportEscaneoToPdf(escaneo, filePath);
                    break;
                default:
                    return BadRequest("Formato no soportado");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", $"Escaneo_{id}.{format}");
        }
    }
}