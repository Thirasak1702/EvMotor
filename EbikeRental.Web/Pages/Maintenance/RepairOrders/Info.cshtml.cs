using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EbikeRental.Web.Pages.Maintenance.RepairOrders;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IRepairService _repairService;
    private readonly IAssetService _assetService;

    public InfoModel(IRepairService repairService, IAssetService assetService)
    {
        _repairService = repairService;
        _assetService = assetService;
    }

    [BindProperty]
    public RepairDto RepairOrder { get; set; } = new();

    public SelectList AssetList { get; set; } = new SelectList(new List<AssetDto>(), "Id", "AssetCode");
    public SelectList StatusList { get; set; } = new SelectList(Enum.GetValues(typeof(RepairStatus)));

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        await LoadAssetsAsync();
        StatusList = new SelectList(Enum.GetValues(typeof(RepairStatus)));

        if (id.HasValue)
        {
            var result = await _repairService.GetByIdAsync(id.Value);
            if (result.Success)
            {
                RepairOrder = result.Data;
                return Page();
            }
            return RedirectToPage("./Index");
        }

        RepairOrder.RequestedDate = DateTime.Today;
        RepairOrder.Status = RepairStatus.Pending;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAssetsAsync();
            StatusList = new SelectList(Enum.GetValues(typeof(RepairStatus)));
            return Page();
        }

        if (RepairOrder.Id == 0)
        {
            var result = await _repairService.CreateRequestAsync(RepairOrder);
            if (result.Success)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
        }

        await LoadAssetsAsync();
        StatusList = new SelectList(Enum.GetValues(typeof(RepairStatus)));
        return Page();
    }

    private async Task LoadAssetsAsync()
    {
        var assetsResult = await _assetService.GetAllAsync();
        if (assetsResult.Success)
        {
            AssetList = new SelectList(assetsResult.Data, "Id", "AssetCode");
        }
    }
}
