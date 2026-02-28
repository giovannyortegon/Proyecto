using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuditSentinel.Data;
using AuditSentinel.Services; // Asegúrate que este namespace coincida

namespace AuditSentinel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDBContext _context;
        private readonly EmailService _emailService;

        public int TotalEscaneos { get; set; }
        public int TotalVulnerabilidades { get; set; }
        public int TotalReportes { get; set; }
        public int TotalServidores { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            ApplicationDBContext context,
            EmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
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
        public string Empresa { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Mensaje { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var mensajeCorreo = $@"
                <h3>Nueva Solicitud de Auditoría</h3>
                <p><strong>Nombre:</strong> {Nombre}</p>
                <p><strong>Empresa:</strong> {Empresa}</p>
                <p><strong>Email:</strong> {Email}</p>
                <p><strong>Mensaje:</strong> {Mensaje}</p>
            ";

            await _emailService.SendEmailAsync(
                "Nueva Solicitud AuditSentinel",
                mensajeCorreo
            );

            TempData["Exito"] = "Solicitud enviada correctamente.";

            return RedirectToPage("MensajeEnviado");
        }
    }
}
