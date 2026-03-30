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
        //private readonly ExportService _exportService;

        public DetailsModel(ApplicationDBContext context)
        {
            _context = context;
            //_exportService = exportService;
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
        public async Task<IActionResult> OnPostIniciarAsync(int id)
        {
            var escaneo = await _context.Escaneos
                .Include(e => e.EscaneosPlantillas)
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (escaneo == null) return NotFound();

            if (!escaneo.EscaneosPlantillas.Any())
                return RedirectToPage(new { id = id, error = "No hay plantilla asociada" });

            var resultadosPrevios = _context.EscaneosVulnerabilidades.Where(ev => ev.IdEscaneo == id);
            _context.EscaneosVulnerabilidades.RemoveRange(resultadosPrevios);
            Console.WriteLine($"Estado Actual: {escaneo.Estado}");

            if (escaneo.Estado == EstadoEscaneo.Nuevo)
            {
                escaneo.Estado = EstadoEscaneo.Pendiente;
            }

            Console.WriteLine($"Estado Actual: {escaneo.Estado}");

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = id });
        }

        public async Task<IActionResult> OnPostDetenerAsync(int id)
        {
            if (ScannerServerService.EscaneosEnCurso.TryGetValue(id, out var cts))
                cts.Cancel();

            var escaneo = await _context.Escaneos.FindAsync(id);
            if (escaneo != null)
            {
                escaneo.Estado = EstadoEscaneo.Fallido;
                await _context.SaveChangesAsync();
            }
            return new OkResult();
        }
    }
}