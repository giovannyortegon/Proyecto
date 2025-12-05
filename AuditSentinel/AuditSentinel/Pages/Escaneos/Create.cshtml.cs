using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.Escaneos
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
        public AuditSentinel.Models.Escaneos Escaneos { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Escaneos.Add(Escaneos);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
