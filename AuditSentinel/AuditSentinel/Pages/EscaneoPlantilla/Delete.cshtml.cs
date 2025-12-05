using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.EscaneoPlantilla
{
    public class DeleteModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public DeleteModel(AuditSentinel.Data.ApplicationDBContext context)
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

            var escaneosplantillas = await _context.EscaneosPlantillas.FirstOrDefaultAsync(m => m.IdEscaneo == id);

            if (escaneosplantillas is not null)
            {
                EscaneosPlantillas = escaneosplantillas;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var escaneosplantillas = await _context.EscaneosPlantillas.FindAsync(id);
            if (escaneosplantillas != null)
            {
                EscaneosPlantillas = escaneosplantillas;
                _context.EscaneosPlantillas.Remove(EscaneosPlantillas);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
