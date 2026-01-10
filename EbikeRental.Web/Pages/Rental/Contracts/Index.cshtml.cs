using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Rental.Contracts;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IRentalService _rentalService;

    public IndexModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public List<RentalDto> Rentals { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ContractNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Customer { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _rentalService.GetAllAsync();
        if (result.Success)
        {
            Rentals = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(ContractNumber))
            {
                Rentals = Rentals.Where(r => r.ContractNumber.Contains(ContractNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                Rentals = Rentals.Where(r => r.RentalStartDate >= FromDate.Value).ToList();
            }

            if (ToDate.HasValue)
            {
                Rentals = Rentals.Where(r => r.RentalStartDate <= ToDate.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                Rentals = Rentals.Where(r => r.Status.ToString().Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Customer))
            {
                Rentals = Rentals.Where(r => r.CustomerName.Contains(Customer, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }
}
