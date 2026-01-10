using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Production.ProductionOrders;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IProductionService _productionService;
    private readonly IItemService _itemService;
    private readonly IBomService _bomService;

    public InfoModel(IProductionService productionService, IItemService itemService, IBomService bomService)
    {
        _productionService = productionService;
        _itemService = itemService;
        _bomService = bomService;
    }

    [BindProperty]
    public ProductionOrderDto Order { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();
    public List<BomDto> Boms { get; set; } = new();
    public List<ProductionOrderProcessesDto> Processes { get; set; } = new();
    public List<ProductionOrderQcsDto> QcSteps { get; set; } = new();

    public async Task OnGetAsync(int? id)
    {
        await LoadItems();
        await LoadBoms();

        if (id.HasValue)
        {
            var result = await _productionService.GetOrderByIdAsync(id.Value);
            if (result.Success)
            {
                Order = result.Data;
                Processes = result.Data.Processes;
                QcSteps = result.Data.QcSteps;
            }
        }
        else
        {
            Order = new ProductionOrderDto
            {
                OrderNumber = "Auto-generated",
                PlannedStartDate = DateTime.Now,
                PlannedEndDate = DateTime.Now.AddDays(14),
                Status = "Draft",
                Quantity = 1
            };
        }
    }

    public async Task<IActionResult> OnGetLoadBomAsync(string bomCode)
    {
        if (string.IsNullOrWhiteSpace(bomCode))
        {
            return new JsonResult(new { success = false, message = "BOM Code is required" });
        }

        var bomResult = await _bomService.GetByCodeAsync(bomCode);
        if (!bomResult.Success || bomResult.Data == null)
        {
            return new JsonResult(new { success = false, message = "BOM not found" });
        }

        var bom = bomResult.Data;
        return new JsonResult(new
        {
            success = true,
            bom = new
            {
                id = bom.Id,
                bomCode = bom.BomCode,
                bomName = $"{bom.BomCode} - {bom.Description}",
                parentItemId = bom.ParentItemId,
                items = bom.BomItems.Select(bi => new
                {
                    id = bi.Id,
                    componentItemId = bi.ComponentItemId,
                    componentItemCode = bi.ComponentItemCode,
                    componentItemName = bi.ComponentItemName,
                    quantity = bi.Quantity,
                    unitOfMeasure = bi.UnitOfMeasure,
                    sequence = bi.Sequence
                }).ToList(),
                processes = bom.BomProcesses.Select(bp => new
                {
                    id = bp.Id,
                    sequence = bp.Sequence,
                    workCode = bp.WorkCode,
                    workName = bp.WorkName,
                    numberOfPersons = bp.NumberOfPersons,
                    quantity = bp.Quantity,
                    unitOfMeasure = bp.UnitOfMeasure,
                    notes = bp.Notes
                }).ToList(),
                qcs = bom.BomQcs.Select(bq => new
                {
                    id = bq.Id,
                    sequence = bq.Sequence,
                    qcCode = bq.QcCode,
                    qcName = bq.QcName,
                    qcValues = bq.QcValues,
                    notes = bq.Notes
                }).ToList()
            }
        });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Clear OrderNumber validation error when creating new order or if it's "Auto-generated"
        if (Order.Id == 0 || Order.OrderNumber?.Equals("Auto-generated", StringComparison.OrdinalIgnoreCase) == true)
        {
            ModelState.Remove($"{nameof(Order)}.{nameof(Order.OrderNumber)}");
        }

        // Clear BomCode validation if empty (BOM will be loaded by BOM Code from input or by ItemId)
        if (string.IsNullOrWhiteSpace(Order.BomCode))
        {
            ModelState.Remove($"{nameof(Order)}.{nameof(Order.BomCode)}");
        }

        // Load data for validation errors display
        await LoadItems();
        await LoadBoms();

        if (!ModelState.IsValid)
        {
            // Log validation errors for debugging
            foreach (var error in ModelState)
            {
                if (error.Value.Errors.Count > 0)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Validation Error: {error.Key} - {err.ErrorMessage}");
                    }
                }
            }
            return Page();
        }

        if (Order.Id == 0)
        {
            // Set CreatedByUserId
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            Order.CreatedByUserId = userId;
            
            var result = await _productionService.CreateOrderAsync(Order);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                TempData["ErrorMessage"] = result.Message;
            }
        }
        else
        {
            var result = await _productionService.UpdateOrderAsync(Order);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                TempData["ErrorMessage"] = result.Message;
            }
        }

        return Page();
    }

    private async Task LoadItems()
    {
        var result = await _itemService.GetAllAsync();
        if (result.Success)
        {
            Items = result.Data;
        }
    }

    private async Task LoadBoms()
    {
        var result = await _bomService.GetAllAsync();
        if (result.Success)
        {
            Boms = result.Data;
        }
    }
}
