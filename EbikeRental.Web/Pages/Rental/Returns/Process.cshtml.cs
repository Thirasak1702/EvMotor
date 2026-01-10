using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Rental.Returns;

[Authorize]
public class ProcessModel : PageModel
{
    private readonly IRentalService _rentalService;

    public ProcessModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public RentalDto Rental { get; set; } = new();

    [BindProperty]
    public DateTime ReturnDate { get; set; }

    [BindProperty]
    public string? Notes { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await _rentalService.GetByIdAsync(id);
        if (result.Success)
        {
            Rental = result.Data;
            ReturnDate = DateTime.Today;
            return Page();
        }

        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var result = await _rentalService.ReturnAssetAsync(id, ReturnDate, Notes);
        if (result.Success)
        {
            return RedirectToPage("./Index");
        }

        ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
        
        var rentalResult = await _rentalService.GetByIdAsync(id);
        if (rentalResult.Success)
        {
            Rental = rentalResult.Data;
        }

        return Page();
    }
}
