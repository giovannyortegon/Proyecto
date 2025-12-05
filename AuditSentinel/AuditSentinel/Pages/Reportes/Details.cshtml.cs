using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.Reportes
{
    public class DetailsModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public DetailsModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public AuditSentinel.Models.Reportes Reportes { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportes = await _context.Reportes.FirstOrDefaultAsync(m => m.IdReporte == id);

            if (reportes is not null)
            {
                Reportes = reportes;

                return Page();
            }

            return NotFound();
        }
    }
}
