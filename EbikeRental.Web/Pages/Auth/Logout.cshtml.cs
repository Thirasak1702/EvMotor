using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Auth;

public class LogoutModel : PageModel
{
    private readonly IAuthService _authService;

    public LogoutModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _authService.LogoutAsync();
        return RedirectToPage("/Auth/Login");
    }
}
