
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Error
{
    public class NotFoundModel : PageModel
    {
        public string? OriginalPath { get; private set; }
        public string? OriginalQueryString { get; private set; }

        public void OnGet()
        {
            // StatusCodePagesWithReExecute coloca estos datos en HttpContext.Items
            OriginalPath = HttpContext.Items["originalPath"] as string;
            OriginalQueryString = HttpContext.Items["originalQueryString"] as string;
        }
    }
}
