using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.Vulnerabilidades
{
    public class DetailsModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public DetailsModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public AuditSentinel.Models.Vulnerabilidades Vulnerabilidades { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vulnerabilidades = await _context.Vulnerabilidades.FirstOrDefaultAsync(m => m.IdVulnerabilidad == id);

            if (vulnerabilidades is not null)
            {
                Vulnerabilidades = vulnerabilidades;

                return Page();
            }

            return NotFound();
        }
    }
}
