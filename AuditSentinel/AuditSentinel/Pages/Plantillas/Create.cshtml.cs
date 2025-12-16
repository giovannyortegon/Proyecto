
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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public CreateModel(ApplicationDBContext context) => _context = context;

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
                }).ToListAsync(); // Nombre, enum NivelRiesgo, IdVulnerabilidad
        }

        public async Task OnGetAsync()
        {
            await LoadVulsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadVulsAsync();

            //if (!ModelState.IsValid)
            //    return Page();

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

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1) Crear Plantilla
                _context.Plantillas.Add(Plantilla);
                await _context.SaveChangesAsync(); // genera IdPlantilla

                // 2) Crear relaciones en tabla puente (evitar duplicados)
                foreach (var idVul in SelectedVulnerabilidades.Distinct())
                {
                    _context.PlantillasVulnerabilidades.Add(new PlantillasVulnerabilidades
                    {
                        IdPlantilla = Plantilla.IdPlantilla,
                        IdVulnerabilidad = idVul
                    });
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return RedirectToPage("Index");
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"No se pudo guardar la plantilla. Error: {ex.Message}");
                return Page();
            }
        }
    }
}