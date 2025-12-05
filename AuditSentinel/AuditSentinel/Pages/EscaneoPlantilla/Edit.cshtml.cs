using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.EscaneoPlantilla
{
    public class EditModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public EditModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EscaneosPlantillas EscaneosPlantillas { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var escaneosplantillas =  await _context.EscaneosPlantillas.FirstOrDefaultAsync(m => m.IdEscaneo == id);
            if (escaneosplantillas == null)
            {
                return NotFound();
            }
            EscaneosPlantillas = escaneosplantillas;
           ViewData["IdEscaneo"] = new SelectList(_context.Escaneos, "IdEscaneo", "NombreEscaneo");
           ViewData["IdPlantilla"] = new SelectList(_context.Plantillas, "IdPlantilla", "NombrePlantilla");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(EscaneosPlantillas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EscaneosPlantillasExists(EscaneosPlantillas.IdEscaneo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EscaneosPlantillasExists(int id)
        {
            return _context.EscaneosPlantillas.Any(e => e.IdEscaneo == id);
        }
    }
}
