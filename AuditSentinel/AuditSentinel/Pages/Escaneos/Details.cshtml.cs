using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.Escaneos
{
    public class DetailsModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public DetailsModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public AuditSentinel.Models.Escaneos Escaneos { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var escaneos = await _context.Escaneos.FirstOrDefaultAsync(m => m.IdEscaneo == id);

            if (escaneos is not null)
            {
                Escaneos = escaneos;

                return Page();
            }

            return NotFound();
        }
    }
}
