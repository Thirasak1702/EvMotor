using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Production.MaterialIssue;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IMaterialIssueService _miService;
    private readonly IItemService _itemService;
    private readonly IWarehouseService _warehouseService;
    private readonly IProductionService _poService;

    public InfoModel(
        IMaterialIssueService miService,
        IItemService itemService,
        IWarehouseService warehouseService,
        IProductionService poService)
    {
        _miService = miService;
        _itemService = itemService;
        _warehouseService = warehouseService;
        _poService = poService;
    }

    [BindProperty]
    public MaterialIssueDto MI { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();
    public List<WarehouseDto> Warehouses { get; set; } = new();
    public List<ProductionOrderDto> ProductionOrders { get; set; } = new();

    public async Task OnGetAsync(int? id)
    {
        await LoadDropdowns();

        if (id.HasValue)
        {
            var result = await _miService.GetByIdAsync(id.Value);
            if (result.Success && result.Data != null)
            {
                MI = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        else
        {
            MI = new MaterialIssueDto
            {
                IssueDate = DateTime.Today,
                DocumentNumber = "Auto-generated",
                Status = "Draft"
            };
        }
    }

    public async Task<IActionResult> OnGetGenerateFromPOAsync(int poId)
    {
        var result = await _miService.GenerateFromProductionOrderAsync(poId);
        
        if (result.Success && result.Data != null)
        {
            return new JsonResult(new
            {
                success = true,
                data = result.Data,
                message = result.Message
            });
        }
        
        return new JsonResult(new
        {
            success = false,
            message = result.Message
        });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return Page();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (MI.Id == 0)
        {
            var result = await _miService.CreateAsync(MI, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                await LoadDropdowns();
                return Page();
            }
        }
        else
        {
            var result = await _miService.UpdateAsync(MI.Id, MI, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                await LoadDropdowns();
                return Page();
            }
        }
    }

    private async Task LoadDropdowns()
    {
        var itemsResult = await _itemService.GetAllAsync();
        if (itemsResult.Success && itemsResult.Data != null)
            Items = itemsResult.Data;

        var whResult = await _warehouseService.GetAllAsync();
        if (whResult.Success && whResult.Data != null)
            Warehouses = whResult.Data;

        var poResult = await _poService.GetAllOrdersAsync();
        if (poResult.Success && poResult.Data != null)
            ProductionOrders = poResult.Data
                .Where(po =>
                    !po.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) &&
                    !po.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                .ToList();
    }
}
