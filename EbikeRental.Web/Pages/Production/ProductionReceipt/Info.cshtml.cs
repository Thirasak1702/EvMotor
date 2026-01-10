using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Production.ProductionReceipt;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IProductionReceiptService _prService;
    private readonly IItemService _itemService;
    private readonly IWarehouseService _warehouseService;
    private readonly IProductionService _poService;

    public InfoModel(
        IProductionReceiptService prService,
        IItemService itemService,
        IWarehouseService warehouseService,
        IProductionService poService)
    {
        _prService = prService;
        _itemService = itemService;
        _warehouseService = warehouseService;
        _poService = poService;
    }

    [BindProperty]
    public ProductionReceiptDto PR { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();
    public List<WarehouseDto> Warehouses { get; set; } = new();
    public List<ProductionOrderDto> ProductionOrders { get; set; } = new();

    public async Task OnGetAsync(int? id)
    {
        await LoadDropdowns();

        if (id.HasValue)
        {
            var result = await _prService.GetByIdAsync(id.Value);
            if (result.Success && result.Data != null)
            {
                PR = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        else
        {
            PR = new ProductionReceiptDto
            {
                ReceiptDate = DateTime.Today,
                DocumentNumber = "Auto-generated",
                Status = "Draft"
            };
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return Page();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (PR.Id == 0)
        {
            var result = await _prService.CreateAsync(PR, userId);
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
            var result = await _prService.UpdateAsync(PR.Id, PR, userId);
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
            Items = itemsResult.Data.Where(i => i.Category == "Finished Product" || i.Category == "Semi-Finished").ToList();

        var whResult = await _warehouseService.GetAllAsync();
        if (whResult.Success && whResult.Data != null)
            Warehouses = whResult.Data;

        var poResult = await _poService.GetAllOrdersAsync();
        if (poResult.Success && poResult.Data != null)
            ProductionOrders = poResult.Data
                .Where(po => po.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                .ToList();
    }

    public async Task<IActionResult> OnGetLoadFromPOAsync(int poId)
    {
        var poResult = await _poService.GetOrderByIdAsync(poId);
        if (!poResult.Success || poResult.Data == null)
        {
            return new JsonResult(new
            {
                success = false,
                message = poResult.Message ?? "Production order not found"
            });
        }

        var po = poResult.Data;

        // หา Item หลักของ Production Order (สินค้าสำเร็จรูป) พร้อม UOM
        var itemsResult = await _itemService.GetAllAsync();
        var itemList = itemsResult.Success && itemsResult.Data != null ? itemsResult.Data : new List<ItemDto>();
        var item = itemList.FirstOrDefault(i => i.Id == po.ItemId);
        var unit = item?.UnitOfMeasure ?? "PCS";

        var dto = new
        {
            items = new[]
            {
                new
                {
                    itemId = po.ItemId,
                    itemCode = po.ItemCode,
                    itemName = po.ItemName,
                    plannedQuantity = (decimal)po.Quantity,
                    receivedQuantity = (decimal)po.Quantity,
                    unitOfMeasure = unit
                }
            }
        };

        return new JsonResult(new
        {
            success = true,
            data = dto,
            message = "Generated finished goods from Production Order"
        });
    }
}
