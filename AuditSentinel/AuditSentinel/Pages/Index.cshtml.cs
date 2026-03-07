#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AuditSentinel.Data;
using AuditSentinel.Services;

namespace AuditSentinel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDBContext _context;
        private readonly EmailService _emailService;
        private readonly GraficaService _graficaService;

        // ── Estadísticas ─────────────────────────────────────────────────
        public int TotalEscaneos       { get; set; }
        public int TotalVulnerabilidades { get; set; }
        public int TotalReportes       { get; set; }
        public int TotalServidores     { get; set; }

        // Ruta web de la imagen para el <img src="...">
        public string RutaGrafica      { get; set; } = string.Empty;

        // ── Formulario de contacto ────────────────────────────────────────
        [BindProperty]
        public CorreosViewModel Correos { get; set; } = new();

        public IndexModel(
            ILogger<IndexModel> logger,
            ApplicationDBContext context,
            EmailService emailService,
            GraficaService graficaService)
        {
            _logger        = logger;
            _context       = context;
            _emailService  = emailService;
            _graficaService = graficaService;
        }

        public void OnGet()
        {
            TotalEscaneos        = _context.Escaneos.Count();
            TotalVulnerabilidades = _context.Vulnerabilidades.Count();
            TotalReportes        = _context.Reportes.Count();
            TotalServidores      = _context.Servidores.Count();

            try
            {
                // Genera la imagen y devuelve la ruta web
                _graficaService.GenerarGrafica();
                RutaGrafica = _graficaService.RutaWebGrafica();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar la gráfica de escaneos.");
                RutaGrafica = string.Empty;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var mensajeCorreo = $@"
                <h3>Nueva Solicitud de Auditoría</h3>
                <p><strong>Nombre:</strong>  {Correos.Nombre}</p>
                <p><strong>Empresa:</strong> {Correos.Empresa}</p>
                <p><strong>Email:</strong>   {Correos.Email}</p>
                <p><strong>Mensaje:</strong> {Correos.Mensaje}</p>";

            await _emailService.SendEmailAsync(
                "Nueva Solicitud AuditSentinel", mensajeCorreo);

            TempData["Exito"] = "Solicitud enviada correctamente.";
            return RedirectToPage("MensajeEnviado");
        }
    }

    // ── ViewModel formulario de contacto ─────────────────────────────────
    public class CorreosViewModel
    {
        public string Nombre  { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public string Email   { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}