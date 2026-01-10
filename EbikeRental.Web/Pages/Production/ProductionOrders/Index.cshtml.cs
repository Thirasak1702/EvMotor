using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Production.ProductionOrders;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IProductionService _productionService;
    private readonly IQualityCheckService _qcService;

    public IndexModel(IProductionService productionService, IQualityCheckService qcService)
    {
        _productionService = productionService;
        _qcService = qcService;
    }

    public List<ProductionOrderDto> ProductionOrders { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? OrderNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Item { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _productionService.GetAllOrdersAsync();
        if (result.Success)
        {
            ProductionOrders = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                ProductionOrders = ProductionOrders.Where(po => po.OrderNumber.Contains(OrderNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                ProductionOrders = ProductionOrders.Where(po => po.PlannedStartDate >= FromDate.Value).ToList();
            }

            if (ToDate.HasValue)
            {
                ProductionOrders = ProductionOrders.Where(po => po.PlannedStartDate <= ToDate.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                ProductionOrders = ProductionOrders.Where(po => po.Status.ToString().Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Item))
            {
                ProductionOrders = ProductionOrders.Where(po => po.ItemName.Contains(Item, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Calculate Completed Quantity and Progress from QC (same logic as Track.cshtml.cs)
            await CalculateCompletedFromQCForAllOrders();
        }
    }

    private async Task CalculateCompletedFromQCForAllOrders()
    {
        foreach (var order in ProductionOrders)
        {
            try
            {
                // Load Quality Checks for this Production Order
                var qcResult = await _qcService.GetByProductionOrderIdAsync(order.Id);
                
                if (qcResult.Success && qcResult.Data != null && qcResult.Data.Any())
                {
                    // Calculate Completed Quantity from QC Items that have Passed status
                    decimal completedQuantityDecimal = 0;

                    foreach (var qc in qcResult.Data)
                    {
                        if (qc.Status == Domain.Enums.QualityCheckStatus.Passed && qc.Items != null)
                        {
                            // Sum PassedQuantity from all items in Passed QC
                            completedQuantityDecimal += qc.Items.Sum(item => item.PassedQuantity);
                        }
                    }

                    // Convert to int and update the order's CompletedQuantity
                    order.CompletedQuantity = (int)Math.Floor(completedQuantityDecimal);
                    
                    // Note: ProgressPercentage is calculated automatically by the DTO property
                }
                else
                {
                    // No QC data - use default values (0)
                    order.CompletedQuantity = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating QC for Order {order.OrderNumber}: {ex.Message}");
                order.CompletedQuantity = 0;
            }
        }
    }

    public async Task<IActionResult> OnPostStartAsync(int id)
    {
        var result = await _productionService.StartProductionAsync(id);
        return new JsonResult(new { success = result.Success, message = result.Message });
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
        var result = await _productionService.CompleteProductionAsync(id);
        return new JsonResult(new { success = result.Success, message = result.Message });
    }
}

