using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AuditSentinel.Pages.Escaneos
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public CreateModel(ApplicationDBContext context)
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

        public void OnGet()
        {
            Servidores = _context.Servidores
                .Select(s => new SelectListItem
                {
                    Value = s.IdServidor.ToString(),
                    Text = $"{s.NombreServidor}-{s.IP}-{s.SistemaOperativo}"
                }).ToList();

            Plantillas = _context.Plantillas
                .Select(p => new SelectListItem
                {
                    Value = p.IdPlantilla.ToString(),
                    Text = p.NombrePlantilla
                }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    OnGet();
            //    return Page();
            //}

            _context.Escaneos.Add(Escaneo);
            await _context.SaveChangesAsync();

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
    }
}
