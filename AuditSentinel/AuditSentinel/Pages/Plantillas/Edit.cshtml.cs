
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Plantillas
{
    [Authorize(Roles = "Auditor,Analista,Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public EditModel(ApplicationDBContext context) => _context = context;

        [BindProperty] public AuditSentinel.Models.Plantillas Plantilla { get; set; } = new();
        public List<SelectListItem> VulnerabilidadesList { get; set; } = new();
        [BindProperty] public int[] SelectedVulnerabilidades { get; set; } = Array.Empty<int>();

        private async Task LoadVulsAsync()
        {
            VulnerabilidadesList = await _context.Vulnerabilidades
                .OrderBy(v => v.NombreVulnerabilidad)
                .Select(v => new SelectListItem
                {
                    Value = v.IdVulnerabilidad.ToString(),
                    Text = $"{v.NombreVulnerabilidad} ({v.NivelRiesgo.ToString()})"
                }).ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Plantilla = await _context.Plantillas
                .Include(p => p.PlantillasVulnerabilidades)
                .FirstOrDefaultAsync(p => p.IdPlantilla == id);

            if (Plantilla == null) return NotFound();

            await LoadVulsAsync();

            SelectedVulnerabilidades = Plantilla.PlantillasVulnerabilidades
                .Select(pv => pv.IdVulnerabilidad)
                .ToArray();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadVulsAsync();

            if (!ModelState.IsValid) return Page();

            var dbPlantilla = await _context.Plantillas
                .Include(p => p.PlantillasVulnerabilidades)
                .FirstOrDefaultAsync(p => p.IdPlantilla == Plantilla.IdPlantilla);

            if (dbPlantilla == null) return NotFound();

            // Actualizar campos simples
            dbPlantilla.NombrePlantilla = Plantilla.NombrePlantilla;
            dbPlantilla.Version = Plantilla.Version;

            // Validar existencia de vulnerabilidades seleccionadas
            var vulsValidas = await _context.Vulnerabilidades
                .Where(v => SelectedVulnerabilidades.Contains(v.IdVulnerabilidad))
                .Select(v => v.IdVulnerabilidad)
                .ToListAsync();

            var faltantes = SelectedVulnerabilidades.Except(vulsValidas).ToArray();
            if (faltantes.Any())
            {
                ModelState.AddModelError(nameof(SelectedVulnerabilidades),
                    $"Hay vulnerabilidades seleccionadas que no existen: {string.Join(",", faltantes)}");
                return Page();
            }

            // Sincronizar join: agregar las nuevas y eliminar las no seleccionadas
            var actuales = dbPlantilla.PlantillasVulnerabilidades.Select(pv => pv.IdVulnerabilidad).ToHashSet();
            var seleccionadas = SelectedVulnerabilidades.ToHashSet();

            var aAgregar = seleccionadas.Except(actuales);
            var aEliminar = actuales.Except(seleccionadas);

            foreach (var idVul in aAgregar)
                _context.PlantillasVulnerabilidades.Add(new PlantillasVulnerabilidades
                {
                    IdPlantilla = dbPlantilla.IdPlantilla,
                    IdVulnerabilidad = idVul
                });

            foreach (var idVul in aEliminar)
            {
                var entidad = dbPlantilla.PlantillasVulnerabilidades
                    .First(pv => pv.IdVulnerabilidad == idVul);
                _context.PlantillasVulnerabilidades.Remove(entidad);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}