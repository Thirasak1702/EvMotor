using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Assets;

[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Warehouse}")]
public class InfoModel : PageModel
{
    private readonly IAssetService _assetService;

    public InfoModel(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [BindProperty]
    public AssetDto Asset { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (Id.HasValue)
        {
            var result = await _assetService.GetByIdAsync(Id.Value);
            if (result.Success && result.Data != null)
            {
                Asset = result.Data;
            }
            else
            {
                return RedirectToPage("Index");
            }
        }
        else
        {
            Asset = new AssetDto 
            { 
                PurchaseDate = DateTime.Today,
                IsActive = true,
                Status = Domain.Enums.AssetStatus.InStock
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Asset.Id > 0)
        {
            var result = await _assetService.UpdateAsync(Asset);
            if (result.Success)
            {
                return RedirectToPage("Index");
            }
            ModelState.AddModelError(string.Empty, result.Message);
        }
        else
        {
            var result = await _assetService.CreateAsync(Asset);
            if (result.Success)
            {
                return RedirectToPage("Index");
            }
            ModelState.AddModelError(string.Empty, result.Message);
        }

        return Page();
    }
}
