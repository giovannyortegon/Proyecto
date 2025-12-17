using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSentinel.Pages.Escaneos
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
        public EstadoEscaneo? BEstado { get; set; }

        public IList<AuditSentinel.Models.Escaneos> Escaneos { get;set; } = new List<AuditSentinel.Models.Escaneos>();

        public async Task OnGetAsync(string? search, EstadoEscaneo? estado)
        {
            Search = search;
            BEstado = estado;

            var query = _context.Escaneos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(e => e.NombreEscaneo.Contains(Search));

            if (BEstado.HasValue)
                query = query.Where(ee => ee.Estado == BEstado.Value);

            Escaneos = await query
                .OrderByDescending(e => e.FechaEscaneo)
                .ToListAsync();

            //Escaneos = await _context.Escaneos.ToListAsync();
        }
    }
}
