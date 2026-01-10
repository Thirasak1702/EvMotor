using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Rental.Returns;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IRentalService _rentalService;

    public IndexModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public List<RentalDto> ActiveRentals { get; set; } = new();

    public async Task OnGetAsync()
    {
        var result = await _rentalService.GetAllAsync();
        if (result.Success)
        {
            // Filter for active rentals only
            ActiveRentals = result.Data
                .Where(r => r.Status == EbikeRental.Domain.Enums.RentalStatus.Active)
                .ToList();
        }
    }
}
