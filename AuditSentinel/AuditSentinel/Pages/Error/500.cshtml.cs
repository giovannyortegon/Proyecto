
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Error
{
    public class ServerErrorModel : PageModel
    {
        public string? RequestId { get; private set; }
        public string? ExceptionMessage { get; private set; }

        public void OnGet()
        {
            // Al usar UseExceptionHandler("/Error/500"), puedes leer la excepción si es necesario
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            RequestId = HttpContext.TraceIdentifier;

        }
    }
}
