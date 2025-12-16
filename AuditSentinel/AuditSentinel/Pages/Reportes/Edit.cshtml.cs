
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Reportes
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public EditModel(ApplicationDBContext context) => _context = context;

        [BindProperty] public AuditSentinel.Models.Reportes Reporte { get; set; } = new();
        public List<SelectListItem> EscaneosList { get; set; } = new();
        [BindProperty] public int[] SelectedEscaneos { get; set; } = Array.Empty<int>();

        private async Task LoadEscaneosAsync()
        {
            EscaneosList = await _context.Escaneos
                .OrderBy(e => e.NombreEscaneo)
                .Select(e => new SelectListItem
                {
                    Value = e.IdEscaneo.ToString(),
                    Text = $"{e.NombreEscaneo} ({e.Estado})"
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Reporte = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (Reporte == null) return NotFound();

            await LoadEscaneosAsync();

            SelectedEscaneos = Reporte.EscaneosReportes
                .Select(er => er.IdEscaneo)
                .ToArray();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadEscaneosAsync();

            if (!ModelState.IsValid) return Page();

            var dbReporte = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .FirstOrDefaultAsync(r => r.IdReporte == Reporte.IdReporte);

            if (dbReporte == null) return NotFound();

            // Actualizar campos simples
            dbReporte.NombreReporte = Reporte.NombreReporte;
            dbReporte.cumplimiento = Reporte.cumplimiento;
            dbReporte.Creado = Reporte.Creado;

            // Validar existencia de escaneos seleccionados
            var escaneosValidos = await _context.Escaneos
                .Where(e => SelectedEscaneos.Contains(e.IdEscaneo))
                .Select(e => e.IdEscaneo).ToListAsync();

            var faltantes = SelectedEscaneos.Except(escaneosValidos).ToArray();
            if (faltantes.Any())
            {
                ModelState.AddModelError(nameof(SelectedEscaneos),
                    $"Hay escaneos seleccionados que no existen: {string.Join(",", faltantes)}");
                return Page();
            }

            // Sincronizar join (add/remove)
            var actuales = dbReporte.EscaneosReportes.Select(er => er.IdEscaneo).ToHashSet();
            var seleccion = SelectedEscaneos.ToHashSet();

            var aAgregar = seleccion.Except(actuales);
            var aEliminar = actuales.Except(seleccion);

            foreach (var idEsc in aAgregar)
                _context.EscaneosReportes.Add(new EscaneosReportes
                {
                    IdReporte = dbReporte.IdReporte,
                    IdEscaneo = idEsc
                });

            foreach (var idEsc in aEliminar)
            {
                var entity = dbReporte.EscaneosReportes.First(er => er.IdEscaneo == idEsc);
                _context.EscaneosReportes.Remove(entity);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}