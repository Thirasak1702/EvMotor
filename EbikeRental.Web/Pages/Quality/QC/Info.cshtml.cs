using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Quality.QC;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IQualityCheckService _qcService;
    private readonly IItemService _itemService;
    private readonly IGoodsReceiptService _grService;
    private readonly IProductionService _poService;
    private readonly IRepository<ProductionOrderQcs> _poQcRepository;

    public InfoModel(
        IQualityCheckService qcService,
        IItemService itemService,
        IGoodsReceiptService grService,
        IProductionService poService,
        IRepository<ProductionOrderQcs> poQcRepository)
    {
        _qcService = qcService;
        _itemService = itemService;
        _grService = grService;
        _poService = poService;
        _poQcRepository = poQcRepository;
    }

    [BindProperty]
    public QualityCheckDto QC { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();
    public List<GoodsReceiptDto> GoodsReceipts { get; set; } = new();
    public List<ProductionOrderDto> ProductionOrders { get; set; } = new();

    public async Task OnGetAsync(int? id, int? productionOrderId)
    {
        await LoadDropdowns();

        if (id.HasValue)
        {
            var result = await _qcService.GetByIdAsync(id.Value);
            if (result.Success && result.Data != null)
            {
                QC = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        else
        {
            QC = new QualityCheckDto
            {
                InspectionDate = DateTime.Today,
                DocumentNumber = "Auto-generated",
                CheckType = QualityCheckType.Incoming,
                Status = QualityCheckStatus.Pending
            };

            // Load from Production Order if provided
            if (productionOrderId.HasValue)
            {
                await LoadFromProductionOrderAsync(productionOrderId.Value);
            }
        }
    }

    public async Task<IActionResult> OnGetLoadGoodsReceiptItemsAsync(int grId)
    {
        try
        {
            var grResult = await _grService.GetByIdAsync(grId);
            if (!grResult.Success || grResult.Data == null)
            {
                return new JsonResult(new { success = false, message = "Goods receipt not found" });
            }

            var gr = grResult.Data;

            // Return GR items
            return new JsonResult(new
            {
                success = true,
                message = "Goods Receipt items loaded successfully",
                items = gr.Items.Select(i => new
                {
                    itemId = i.ItemId,
                    itemCode = i.ItemCode,
                    itemName = i.ItemName,
                    quantity = i.ReceivedQuantity,
                    documentNumber = gr.DocumentNumber
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    public async Task<IActionResult> OnGetLoadProductionOrderQcAsync(int productionOrderId)
    {
        try
        {
            var poResult = await _poService.GetOrderByIdAsync(productionOrderId);
            if (!poResult.Success || poResult.Data == null)
            {
                return new JsonResult(new { success = false, message = "Production order not found" });
            }

            var po = poResult.Data;
            var qcSteps = po.QcSteps;

            // Check if can reference
            if (qcSteps == null || !qcSteps.Any())
            {
                // No QC Steps - can reference
                return new JsonResult(new
                {
                    success = true,
                    canReference = true,
                    message = "Production order has no QC steps. You can create inspection items manually.",
                    qcSteps = new List<object>()
                });
            }

            // Check if all QC steps are passed
            var pendingSteps = qcSteps.Where(q => q.Status != "Passed").ToList();

            if (pendingSteps.Any())
            {
                // Has pending or failed QC steps - cannot reference
                return new JsonResult(new
                {
                    success = false,
                    canReference = false,
                    message = $"Cannot reference: {pendingSteps.Count} QC step(s) are not passed yet. Please complete all QC steps in production tracking first.",
                    pendingSteps = pendingSteps.Select(q => new
                    {
                        sequence = q.Sequence,
                        qcCode = q.QcCode,
                        qcName = q.QcName,
                        status = q.Status
                    }).ToList()
                });
            }

            // All QC steps passed - can reference
            return new JsonResult(new
            {
                success = true,
                canReference = true,
                message = "All QC steps passed. Loading inspection items...",
                productionOrder = new
                {
                    id = po.Id,
                    orderNumber = po.OrderNumber,
                    itemId = po.ItemId,
                    itemCode = po.ItemCode,
                    itemName = po.ItemName,
                    quantity = po.Quantity
                },
                qcSteps = qcSteps.Select(q => new
                {
                    id = q.Id,
                    sequence = q.Sequence,
                    qcCode = q.QcCode,
                    qcName = q.QcName,
                    qcValues = q.QcValues,
                    status = q.Status,
                    actualValues = q.ActualValues,
                    notes = q.Notes
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
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

        if (QC.Id == 0)
        {
            var result = await _qcService.CreateAsync(QC, userId);
            if (result.Success)
            {
                // Check if should auto-complete Production Order
                await CheckAndCompleteProductionOrder();

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
            var result = await _qcService.UpdateAsync(QC.Id, QC, userId);
            if (result.Success)
            {
                // Check if should auto-complete Production Order
                await CheckAndCompleteProductionOrder();

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

    private async Task CheckAndCompleteProductionOrder()
    {
        try
        {
            // Only proceed if QC is for a Production Order and status is Passed
            if (!QC.ProductionOrderId.HasValue || QC.Status != QualityCheckStatus.Passed)
                return;

            // Check if all items are inspected, passed, and no rejections
            bool allInspected = QC.Items.All(item => item.InspectedQuantity > 0 && item.PassedQuantity > 0);
            bool hasRejection = QC.Items.Any(item => item.RejectedQuantity > 0);

            if (!allInspected || hasRejection)
                return;

            // Get Production Order
            var poResult = await _poService.GetOrderByIdAsync(QC.ProductionOrderId.Value);
            if (!poResult.Success || poResult.Data == null)
                return;

            var po = poResult.Data;

            // Check if all QC Steps are passed
            var allQcStepsPassed = po.QcSteps.All(q => q.Status == "Passed");

            if (allQcStepsPassed && po.Status == "InProgress")
            {
                // Complete the Production Order
                var completeResult = await _poService.CompleteProductionAsync(QC.ProductionOrderId.Value);
                if (completeResult.Success)
                {
                    TempData["SuccessMessage"] = (TempData["SuccessMessage"]?.ToString() ?? "") +
                        $" Production Order {po.OrderNumber} has been automatically completed!";
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the QC save
            System.Diagnostics.Debug.WriteLine($"Error auto-completing Production Order: {ex.Message}");
        }
    }

    private async Task LoadDropdowns()
    {
        var itemsResult = await _itemService.GetAllAsync();
        if (itemsResult.Success && itemsResult.Data != null)
            Items = itemsResult.Data;

        var grResult = await _grService.GetAllAsync();
        if (grResult.Success && grResult.Data != null)
            GoodsReceipts = grResult.Data.Where(gr => gr.Status == "Posted").ToList();

        var poResult = await _poService.GetAllOrdersAsync();
        if (poResult.Success && poResult.Data != null)
            ProductionOrders = poResult.Data;
    }

    private async Task LoadFromProductionOrderAsync(int productionOrderId)
    {
        try
        {
            var poResult = await _poService.GetOrderByIdAsync(productionOrderId);
            if (!poResult.Success || poResult.Data == null)
            {
                TempData["ErrorMessage"] = "Production order not found";
                return;
            }

            var po = poResult.Data;
            var qcSteps = po.QcSteps;

            // Check if can reference
            if (qcSteps != null && qcSteps.Any())
            {
                var pendingSteps = qcSteps.Where(q => q.Status != "Passed").ToList();

                if (pendingSteps.Any())
                {
                    TempData["ErrorMessage"] = $"Cannot reference: {pendingSteps.Count} QC step(s) are not passed yet.";
                    return;
                }
            }

            // Set QC properties
            QC.ProductionOrderId = po.Id;
            QC.CheckType = QualityCheckType.Final;
            QC.InspectedBy = User.Identity?.Name ?? "";

            // Create inspection item from production order
            QC.Items.Add(new QualityCheckItemDto
            {
                ItemId = po.ItemId,
                ItemCode = po.ItemCode,
                ItemName = po.ItemName,
                InspectedQuantity = po.Quantity,
                PassedQuantity = po.CompletedQuantity,
                RejectedQuantity = 0,
                ItemStatus = QualityCheckStatus.Pending
            });

            TempData["SuccessMessage"] = $"Loaded from Production Order: {po.OrderNumber}";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading from production order: {ex.Message}";
        }
    }
}
