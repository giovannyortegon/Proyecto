using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Pages.EscaneoPlantilla
{
    public class IndexModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public IndexModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IList<EscaneosPlantillas> EscaneosPlantillas { get;set; } = default!;

        public async Task OnGetAsync()
        {
            EscaneosPlantillas = await _context.EscaneosPlantillas
                .Include(e => e.Escaneos)
                .Include(e => e.Plantillas).ToListAsync();
        }
    }
}
