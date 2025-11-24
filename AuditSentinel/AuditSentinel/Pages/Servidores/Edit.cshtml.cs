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

namespace AuditSentinel.Pages.Servidores
{
    public class EditModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public EditModel(AuditSentinel.Data.ApplicationDBContext context)
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

            var servidores =  await _context.Servidores.FirstOrDefaultAsync(m => m.IdServidor == id);
            if (servidores == null)
            {
                return NotFound();
            }
            Servidores = servidores;
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

            _context.Attach(Servidores).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServidoresExists(Servidores.IdServidor))
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

        private bool ServidoresExists(int id)
        {
            return _context.Servidores.Any(e => e.IdServidor == id);
        }
    }
}
