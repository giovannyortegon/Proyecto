using AuditSentinel.Data;
using AuditSentinel.Models;
using AuditSentinel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Escaneos
{
    [Authorize(Roles = "Analista,Administrador")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        private readonly ExportService _exportService;
        public string ErrorMensaje { get; set; }

        public DetailsModel(ApplicationDBContext context, ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        public AuditSentinel.Models.Escaneos Escaneo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Escaneo = await _context.Escaneos
                .Include(e => e.EscaneosServidores).ThenInclude(es => es.Servidores)
                .Include(e => e.EscaneosPlantillas).ThenInclude(ep => ep.Plantillas)
                .Include(e => e.EscaneosVulnerabilidades).ThenInclude(ev => ev.Vulnerabilidades)
                .FirstOrDefaultAsync(m => m.IdEscaneo == id);

            if (Escaneo == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostIniciarAsync(int id, string error = null)
        {
            Escaneo = await _context.Escaneos
                .Include(e => e.EscaneosPlantillas)
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (Escaneo == null)
                return NotFound();


            if (!Escaneo.EscaneosPlantillas.Any())
                return RedirectToPage(new { id, error = "No hay plantilla asociada" });

            if (Escaneo.Estado == EstadoEscaneo.Completado) {
                return RedirectToPage(new { id, error = "No se puede iniciar un escaneo en estado COMPLETADO" });
            }

            var resultadosPrevios = _context.EscaneosVulnerabilidades
                .Where(ev => ev.IdEscaneo == id);

            _context.EscaneosVulnerabilidades.RemoveRange(resultadosPrevios);

            if (Escaneo.Estado != EstadoEscaneo.Nuevo || Escaneo.Estado != EstadoEscaneo.Fallido)
            {
                Escaneo.Estado = EstadoEscaneo.Pendiente;
                Escaneo.FechaEscaneo = DateTime.Now;
            }

            await _context.SaveChangesAsync();
         
            return RedirectToPage(new { id });
        }
        public async Task<IActionResult> OnPostDetenerAsync(int id)
        {
            // Cargar la propiedad para mantener la estabilidad de la página
            Escaneo = await _context.Escaneos.FindAsync(id);

            if (ScannerServerService.EscaneosEnCurso.TryRemove(id, out var cts))
            {
                cts.Cancel();
            }

            if (Escaneo != null)
            {
                Escaneo.Estado = EstadoEscaneo.Fallido;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnGetExportarAsync(int id, string format)
        {
            var escaneoCompleto = await _context.Escaneos
                .Include(e => e.EscaneosServidores)
                    .ThenInclude(es => es.Servidores)
                .Include(e => e.EscaneosVulnerabilidades)
                    .ThenInclude(ev => ev.Vulnerabilidades)
                .Include(e => e.EscaneosPlantillas)
                    .ThenInclude(ep => ep.Plantillas)
                .FirstOrDefaultAsync(m => m.IdEscaneo == id);

            if (escaneoCompleto == null) return NotFound();

            var fileName = $"ReporteDetallado_Escaneo_{id}.{format}";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            switch (format.ToLower())
            {
                //case "csv":
                //    _exportService.ExportEscaneoToCsv(escaneoCompleto, filePath);
                //    return File(System.IO.File.ReadAllBytes(filePath), "text/csv", fileName);

                //case "html":
                //    _exportService.ExportEscaneoToHtml(escaneoCompleto, filePath);
                //    return File(System.IO.File.ReadAllBytes(filePath), "text/html", fileName);

                case "pdf":
                    _exportService.ExportEscaneoToPdf(escaneoCompleto, filePath);
                    return File(System.IO.File.ReadAllBytes(filePath), "application/pdf", fileName);

                default:
                    return BadRequest("Formato no soportado.");
            }
        }

    }
}