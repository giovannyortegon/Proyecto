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
                .Include(e => e.EscaneosServidores).ThenInclude(es => es.Servidores)
                .Include(e => e.EscaneosPlantillas).ThenInclude(ep => ep.Plantillas)
                .Include(e => e.EscaneosVulnerabilidades).ThenInclude(ev => ev.Vulnerabilidades)
                .FirstOrDefaultAsync(m => m.IdEscaneo == id);

            if (Escaneo == null)
            {
                return NotFound();
            }

            return Page();
        }

        // Handler para el botón "Ejecutar Scan"
        public async Task<IActionResult> OnPostIniciarAsync(int id)
        {
            var escaneo = await _context.Escaneos
                .Include(e => e.EscaneosPlantillas) // Cargamos la relación
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (escaneo == null) return NotFound();

            // VALIDACIÓN: Si no hay plantilla, no podemos escanear
            if (!escaneo.EscaneosPlantillas.Any())
            {
                // Aquí podrías retornar un mensaje de error a la vista
                return RedirectToPage(new { id = id, error = "No hay plantilla asociada" });
            }

            // Limpiar resultados anteriores para permitir un re-escaneo limpio
            var resultadosPrevios = _context.EscaneosVulnerabilidades.Where(ev => ev.IdEscaneo == id);
            _context.EscaneosVulnerabilidades.RemoveRange(resultadosPrevios);

            escaneo.Estado = EstadoEscaneo.Pendiente;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = id });
        }
        // Handler para el botón "Abortar"
        public async Task<IActionResult> OnPostDetenerAsync(int id)
        {
            // Intentar cancelar mediante el token en el servicio ScannerServerService
            if (ScannerServerService.EscaneosEnCurso.TryGetValue(id, out var cts))
            {
                cts.Cancel();
            }

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