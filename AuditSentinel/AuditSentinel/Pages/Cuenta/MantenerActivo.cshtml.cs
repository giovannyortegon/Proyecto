using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuditSentinel.Pages.Cuenta
{
    public class MantenerActivoModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new JsonResult(new { utc = DateTimeOffset.UtcNow });
        }

    }
}
