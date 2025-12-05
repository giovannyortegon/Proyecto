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
    public class DetailsModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public DetailsModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

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
    }
}
