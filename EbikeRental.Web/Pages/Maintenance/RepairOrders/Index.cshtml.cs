using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Maintenance.RepairOrders;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IRepairService _repairService;

    public IndexModel(IRepairService repairService)
    {
        _repairService = repairService;
    }

    public List<RepairDto> RepairOrders { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? OrderNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Asset { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _repairService.GetAllAsync();
        if (result.Success)
        {
            RepairOrders = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                RepairOrders = RepairOrders.Where(ro => ro.OrderNumber.Contains(OrderNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                RepairOrders = RepairOrders.Where(ro => ro.RequestedDate >= FromDate.Value).ToList();
            }

            if (ToDate.HasValue)
            {
                RepairOrders = RepairOrders.Where(ro => ro.RequestedDate <= ToDate.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                RepairOrders = RepairOrders.Where(ro => ro.Status.ToString().Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Asset))
            {
                RepairOrders = RepairOrders.Where(ro => ro.AssetCode.Contains(Asset, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }
}
