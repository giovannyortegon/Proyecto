using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;

namespace AuditSentinel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDBContext _context;

        public int TotalEscaneos { get; set; }
        public int TotalVulnerabilidades { get; set; }
        public int TotalReportes { get; set; }
        public int TotalServidores { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            TotalEscaneos = _context.Escaneos.Count();
            TotalVulnerabilidades = _context.Vulnerabilidades.Count();
            TotalReportes = _context.Reportes.Count();
            TotalServidores = _context.Servidores.Count();
        }

        [BindProperty]
        public string Nombre { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Mensaje { get; set; }

        public IActionResult OnPost()
        {
            // Aquí luego podemos guardarlo en base de datos
            Console.WriteLine($"Nuevo contacto: {Nombre} - {Email}");

            return RedirectToPage();
        }
    }
}

