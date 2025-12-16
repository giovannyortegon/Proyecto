using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSentinel.Pages.Servidores
{
    [Authorize(Roles = "Analista,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly AuditSentinel.Data.ApplicationDBContext _context;

        public IndexModel(AuditSentinel.Data.ApplicationDBContext context)
        {
            _context = context;
        }
        public string? Search { get; set; }
        public IList<AuditSentinel.Models.Servidores> Servidores { get;set; } = default!;

        public async Task OnGetAsync(string? search)
        {
            Search = search;

            var query = _context.Servidores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(s => s.SistemaOperativo.Contains(Search) || (s.NombreServidor != null && s.NombreServidor.Contains(Search)));

            Servidores = await query
                .OrderByDescending(s => s.Create_is).ToListAsync();


            //Servidores = await _context.Servidores.ToListAsync();
        }
    }
}
