
using AuditSentinel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Reportes
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public DeleteModel(ApplicationDBContext context) => _context = context;

        // Se muestra en la vista de confirmación
        [BindProperty]
        public AuditSentinel.Models.Reportes Reporte { get; set; } = new();

        // GET: Confirmación de eliminación
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Incluimos la relación para mostrar cuántos escaneos tiene asociados
            Reporte = await _context.Reportes
                .Include(r => r.EscaneosReportes) // -> colección puente
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (Reporte == null)
                return NotFound();

            return Page();
        }

        // POST: Eliminación definitiva
        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Cargar el reporte (puente opcional, solo si quieres tratar manualmente las relaciones)
            var entity = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (entity == null)
                return NotFound();

            // Si tu mapeo tiene Cascade en EscaneosReportes ↔ Reportes (como en tu DbContext), 
            // no es necesario borrar manualmente las filas del puente.
            // Si NO tuvieras Cascade, descomenta la siguiente línea:
            // _context.EscaneosReportes.RemoveRange(entity.EscaneosReportes);

            _context.Reportes.Remove(entity);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"No se pudo eliminar el reporte. Detalle: {ex.Message}");
                return Page();
            }
        }
    }
}