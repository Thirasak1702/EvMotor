using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Dashboard/Index");
        }
        return RedirectToPage("/Auth/Login");
    }
}
