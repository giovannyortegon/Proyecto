using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.Servidores
{
    public class DeleteModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public DeleteModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditSentinel.Models.Servidores Servidores { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servidores = await _context.Servidores.FirstOrDefaultAsync(m => m.IdServidor == id);

            if (servidores is not null)
            {
                Servidores = servidores;

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

            var servidores = await _context.Servidores.FindAsync(id);
            if (servidores != null)
            {
                Servidores = servidores;
                _context.Servidores.Remove(Servidores);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
