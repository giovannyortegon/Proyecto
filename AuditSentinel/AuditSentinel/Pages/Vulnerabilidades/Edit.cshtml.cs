
using AuditSentinel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Vulnerabilidades
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public EditModel(ApplicationDBContext context) => _context = context;

        [BindProperty] public AuditSentinel.Models.Vulnerabilidades Vulnerabilidad { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Vulnerabilidad = await _context.Vulnerabilidades.FirstOrDefaultAsync(v => v.IdVulnerabilidad == id);
            if (Vulnerabilidad == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var entity = await _context.Vulnerabilidades.FindAsync(Vulnerabilidad.IdVulnerabilidad);
            if (entity == null) return NotFound();

            entity.NombreVulnerabilidad = Vulnerabilidad.NombreVulnerabilidad;
            entity.NivelRiesgo = Vulnerabilidad.NivelRiesgo;
            entity.Descripcion = Vulnerabilidad.Descripcion;
            entity.Comando = Vulnerabilidad.Comando;
            entity.ResultadoEsperado = Vulnerabilidad.ResultadoEsperado;
            entity.FechaDetectada = entity.FechaDetectada = Vulnerabilidad.FechaDetectada;

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}