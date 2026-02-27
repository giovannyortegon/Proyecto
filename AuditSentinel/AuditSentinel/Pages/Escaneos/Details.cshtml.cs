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
        private readonly INmapScannerService _nmapService;
        private readonly ApplicationDBContext _context;

        public DetailsModel(INmapScannerService nmapService, ApplicationDBContext context)
        {
            _nmapService = nmapService;
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
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


        // Manejador para iniciar el escaneo vía AJAX
        public IActionResult OnPostIniciar(string ip)
        {
            // Ejecución asíncrona en el servicio (Fire and Forget)
            _ = _nmapService.EjecutarEscaneoAsync(Id, ip);
            return new JsonResult(new { iniciado = true });
        }

        // Manejador para detener el escaneo vía AJAX
        public IActionResult OnPostDetener()
        {
            _nmapService.DetenerEscaneo(Id);
            return new JsonResult(new { detenido = true });
        }

        // Manejador para obtener progreso y logs de errores vía AJAX
        public async Task<IActionResult> OnGetEstado()
        {
            var progreso = _nmapService.ObtenerProgreso(Id);

            // Consultamos los errores específicos de este escaneo
            var errores = await _context.LogErroresEscaneos
                .Where(l => l.EscaneoId == Id)
                .OrderByDescending(l => l.FechaError)
                .Select(l => new {
                    l.FechaError,
                    l.Fase,
                    l.Mensaje,
                    l.ComandoEjecutado
                })
                .ToListAsync();

            return new JsonResult(new { porcentaje = progreso, logs = errores });
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