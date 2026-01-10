using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Users;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IUserService _userService;

    public InfoModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public new UserDto User { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var result = await _userService.GetByIdAsync(id.Value);
            if (result.Success)
            {
                User = result.Data;
                return Page();
            }
            return RedirectToPage("./Index");
        }

        User.IsActive = true;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Note: User creation/update logic would need to be implemented in UserService
        // For now, just redirect back
        return RedirectToPage("./Index");
    }
}
