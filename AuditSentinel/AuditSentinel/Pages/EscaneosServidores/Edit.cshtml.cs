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

namespace AuditSentinel.Pages.EscaneosServidores
{
    public class EditModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public EditModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditSentinel.Models.EscaneosServidores EscaneosServidores { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var escaneosservidores =  await _context.EscaneosServidores.FirstOrDefaultAsync(m => m.IdEscaneo == id);
            if (escaneosservidores == null)
            {
                return NotFound();
            }
            EscaneosServidores = escaneosservidores;
           ViewData["IdEscaneo"] = new SelectList(_context.Escaneos, "IdEscaneo", "NombreEscaneo");
           ViewData["IdServidor"] = new SelectList(_context.Servidores, "IdServidor", "IP");
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

            _context.Attach(EscaneosServidores).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EscaneosServidoresExists(EscaneosServidores.IdEscaneo))
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

        private bool EscaneosServidoresExists(int id)
        {
            return _context.EscaneosServidores.Any(e => e.IdEscaneo == id);
        }
    }
}
