using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSentinel.Pages.Servidores
{
    [Authorize(Roles = "Analista,Administrador")]
    public class EditModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public EditModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditSentinel.Models.Servidores Servidores { get; set; } = default!;
        public List<SelectListItem> SistemasOperativos { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
                SistemasOperativos = Enum.GetValues(typeof(SistemaOperativo))
                .Cast<SistemaOperativo>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.GetType()
                            .GetMember(e.ToString())[0]
                            .GetCustomAttributes(typeof(DisplayAttribute), false)
                            .Cast<DisplayAttribute>()
                            .FirstOrDefault()?.Name ?? e.ToString()
                })
                .ToList();

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

            SistemasOperativos = Enum.GetValues(typeof(SistemaOperativo))
                .Cast<SistemaOperativo>()
                .Select(so => new SelectListItem
                {
                    Value = so.ToString(),
                    Text = GetDisplayName(so)
                })
                .ToList();


            // Validar nombre duplicado
            bool nombreDuplicado = await _context.Servidores
                .AnyAsync(s =>
                    s.NombreServidor == Servidores.NombreServidor &&
                    s.IdServidor != Servidores.IdServidor);

            if (nombreDuplicado)
            {
                ModelState.AddModelError(
                    "Servidores.NombreServidor",
                    "Ya existe un servidor con este nombre."
                );
                return Page();
            }

            // Validar IP duplicada
            bool ipDuplicada = await _context.Servidores
                .AnyAsync(s =>
                    s.IP == Servidores.IP &&
                    s.IdServidor != Servidores.IdServidor);

            if (ipDuplicada)
            {
                ModelState.AddModelError(
                    "Servidores.IP",
                    "Ya existe un servidor con esta IP."
                );
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

        private string GetDisplayName(SistemaOperativo so)
        {
            throw new NotImplementedException();
        }

        private bool ServidoresExists(int id)
        {
            return _context.Servidores.Any(e => e.IdServidor == id);
        }
    }
}
