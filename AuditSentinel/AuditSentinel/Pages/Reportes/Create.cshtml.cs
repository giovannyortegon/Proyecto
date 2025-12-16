
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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        public CreateModel(ApplicationDBContext context) => _context = context;

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

        public async Task OnGetAsync()
        {
            await LoadEscaneosAsync();
            // Creado tiene default DateTime.Now en el modelo
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadEscaneosAsync();

            //if (!ModelState.IsValid)
            //    return Page();

            // Validar que los escaneos seleccionados existan (evita FK 547)
            var escaneosValidos = await _context.Escaneos
                .Where(e => SelectedEscaneos.Contains(e.IdEscaneo))
                .Select(e => e.IdEscaneo)
                .ToListAsync();

            var faltantes = SelectedEscaneos.Except(escaneosValidos).ToArray();
            if (faltantes.Any())
            {
                ModelState.AddModelError(nameof(SelectedEscaneos),
                    $"Hay escaneos seleccionados que no existen: {string.Join(",", faltantes)}");
                return Page();
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1) Guardar reporte
                _context.Reportes.Add(Reporte);
                await _context.SaveChangesAsync(); // genera IdReporte

                // 2) Crear relaciones en EscaneosReportes (evitar duplicados)
                foreach (var idEscaneo in SelectedEscaneos.Distinct())
                {
                    _context.EscaneosReportes.Add(new EscaneosReportes
                    {
                        IdReporte = Reporte.IdReporte,
                        IdEscaneo = idEscaneo
                    });
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToPage("Index");
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"No se pudo guardar el reporte. Error: {ex.Message}");
                return Page();
            }
        }
    }
}