using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuditSentinel.Pages.Escaneos
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public DeleteModel(ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditSentinel.Models.Escaneos Escaneo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Escaneo = await _context.Escaneos
                .FirstOrDefaultAsync(e => e.IdEscaneo == id);

            if (Escaneo == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Escaneo = await _context.Escaneos.FindAsync(id);

            if (Escaneo != null)
            {
                _context.Escaneos.Remove(Escaneo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
