using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.EscaneoPlantilla
{
    public class CreateModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public CreateModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["IdEscaneo"] = new SelectList(_context.Escaneos, "IdEscaneo", "NombreEscaneo");
        ViewData["IdPlantilla"] = new SelectList(_context.Plantillas, "IdPlantilla", "NombrePlantilla");
            return Page();
        }

        [BindProperty]
        public EscaneosPlantillas EscaneosPlantillas { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.EscaneosPlantillas.Add(EscaneosPlantillas);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
