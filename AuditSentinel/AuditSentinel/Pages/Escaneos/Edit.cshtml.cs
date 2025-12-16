using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Escaneos
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public EditModel(ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditSentinel.Models.Escaneos Escaneo { get; set; }

        [BindProperty]
        public List<int> ServidoresSeleccionados { get; set; } = new();

        [BindProperty]
        public List<int> PlantillasSeleccionadas { get; set; } = new();

        public List<SelectListItem> Servidores { get; set; }
        public List<SelectListItem> Plantillas { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Escaneo = await _context.Escaneos
                .Include(e => e.EscaneosServidores)
                .Include(e => e.EscaneosPlantillas)
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (Escaneo == null)
                return NotFound();

            ServidoresSeleccionados = Escaneo.EscaneosServidores
                .Select(es => es.IdServidor)
                .ToList();

            PlantillasSeleccionadas = Escaneo.EscaneosPlantillas
                .Select(ep => ep.IdPlantilla)
                .ToList();

            CargarCombos();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return Page();
            }

            var escaneoDb = await _context.Escaneos
                .Include(e => e.EscaneosServidores)
                .Include(e => e.EscaneosPlantillas)
                .FirstOrDefaultAsync(e => e.IdEscaneo == Escaneo.IdEscaneo);

            if (escaneoDb == null)
                return NotFound();

            escaneoDb.NombreEscaneo = Escaneo.NombreEscaneo;
            escaneoDb.Estado = Escaneo.Estado;

            _context.EscaneosServidores.RemoveRange(escaneoDb.EscaneosServidores);
            _context.EscaneosPlantillas.RemoveRange(escaneoDb.EscaneosPlantillas);

            foreach (var idServidor in ServidoresSeleccionados)
            {
                _context.EscaneosServidores.Add(new AuditSentinel.Models.EscaneosServidores
                {
                    IdEscaneo = Escaneo.IdEscaneo,
                    IdServidor = idServidor
                });
            }

            foreach (var idPlantilla in PlantillasSeleccionadas)
            {
                _context.EscaneosPlantillas.Add(new EscaneosPlantillas
                {
                    IdEscaneo = Escaneo.IdEscaneo,
                    IdPlantilla = idPlantilla
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        private void CargarCombos()
        {
            Servidores = _context.Servidores
                .Select(s => new SelectListItem
                {
                    Value = s.IdServidor.ToString(),
                    Text = $"{s.NombreServidor}-{s.IP}-{s.SistemaOperativo})"
                }).ToList();

            Plantillas = _context.Plantillas
                .Select(p => new SelectListItem
                {
                    Value = p.IdPlantilla.ToString(),
                    Text = p.NombrePlantilla
                }).ToList();
        }
    }
}
