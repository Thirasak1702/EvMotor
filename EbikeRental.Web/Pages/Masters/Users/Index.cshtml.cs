using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Users;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService)
    {
        _userService = userService;
    }

    public List<UserDto> Users { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Name { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _userService.GetAllAsync();
        if (result.Success)
        {
            Users = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Users = Users.Where(u => (u.FirstName + " " + u.LastName).Contains(Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                Users = Users.Where(u => u.Email.Contains(Email, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Role))
            {
                Users = Users.Where(u => u.Roles != null && u.Roles.Any(r => r.Contains(Role, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            if (IsActive.HasValue)
            {
                Users = Users.Where(u => u.IsActive == IsActive.Value).ToList();
            }
        }
    }
}
