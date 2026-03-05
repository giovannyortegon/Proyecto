using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSentinel.Pages.Servidores
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
            return Page();
        }

        [BindProperty]
        public AuditSentinel.Models.Servidores Servidores { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verificar si ya existe un servidor con la misma clave primaria o campo único
            var existe = await _context.Servidores.AnyAsync(s => s.NombreServidor == Servidores.NombreServidor || s.IP == Servidores.IP);
  
            if (existe)
            {
                ModelState.AddModelError(string.Empty, "El servidor ya existe en la base de datos.");
                return Page(); // Regresa a la página mostrando el error
            }

            _context.Servidores.Add(Servidores);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");

        }
    }
}
