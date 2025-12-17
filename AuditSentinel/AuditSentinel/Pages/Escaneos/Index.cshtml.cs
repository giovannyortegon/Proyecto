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


        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }


        public async Task OnGetAsync(string? search, EstadoEscaneo? estado)
        {
            Search = search;
            BEstado = estado;

            var query = _context.Escaneos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(e => e.NombreEscaneo.Contains(Search));

            if (BEstado.HasValue)
                query = query.Where(ee => ee.Estado == BEstado.Value);


            // Calcular total de registros
            TotalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (TotalPages > 0 && PageNumber > TotalPages) PageNumber = TotalPages;



            Escaneos = await query
                .OrderByDescending(e => e.FechaEscaneo)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            //Escaneos = await _context.Escaneos.ToListAsync();
        }
    }
}
