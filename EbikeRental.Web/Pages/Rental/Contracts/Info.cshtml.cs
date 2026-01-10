using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EbikeRental.Web.Pages.Rental.Contracts;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IRentalService _rentalService;
    private readonly IAssetService _assetService;

    public InfoModel(IRentalService rentalService, IAssetService assetService)
    {
        _rentalService = rentalService;
        _assetService = assetService;
    }

    [BindProperty]
    public RentalDto Rental { get; set; } = new();

    public SelectList AssetList { get; set; } = new SelectList(new List<AssetDto>(), "Id", "AssetCode");

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        await LoadAssetsAsync();

        if (id.HasValue)
        {
            var result = await _rentalService.GetByIdAsync(id.Value);
            if (result.Success)
            {
                Rental = result.Data;
                return Page();
            }
            return RedirectToPage("./Index");
        }
        
        // Set defaults for new rental
        Rental.RentalStartDate = DateTime.Today;
        Rental.RentalEndDate = DateTime.Today.AddDays(1);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAssetsAsync();
            return Page();
        }

        if (Rental.Id == 0)
        {
            var result = await _rentalService.CreateContractAsync(Rental);
            if (result.Success)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
        }
        
        await LoadAssetsAsync();
        return Page();
    }

    private async Task LoadAssetsAsync()
    {
        var assetsResult = await _assetService.GetAvailableAssetsAsync();
        if (assetsResult.Success)
        {
            AssetList = new SelectList(assetsResult.Data, "Id", "AssetCode");
        }
        else
        {
            AssetList = new SelectList(new List<AssetDto>(), "Id", "AssetCode");
        }
    }
}
