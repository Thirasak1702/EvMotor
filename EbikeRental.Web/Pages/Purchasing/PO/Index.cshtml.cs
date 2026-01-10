using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Purchasing.PO;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IPurchaseOrderService _poService;

    public IndexModel(IPurchaseOrderService poService)
    {
        _poService = poService;
    }

    public List<PurchaseOrderDto> PurchaseOrders { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? DocumentNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Vendor { get; set; }

    // Pagination properties
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; private set; }
    public int TotalPages { get; private set; }

    public async Task OnGetAsync()
    {
        var result = await _poService.GetAllAsync();
        if (result.Success)
        {
            var allPOs = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                allPOs = allPOs.Where(po => po.DocumentNumber.Contains(DocumentNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                allPOs = allPOs.Where(po => po.OrderDate.Date >= FromDate.Value.Date).ToList();
            }

            if (ToDate.HasValue)
            {
                allPOs = allPOs.Where(po => po.OrderDate.Date <= ToDate.Value.Date).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                allPOs = allPOs.Where(po => po.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Vendor))
            {
                allPOs = allPOs.Where(po => po.VendorName.Contains(Vendor, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Calculate pagination
            TotalItems = allPOs.Count;
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Ensure valid page number
            if (PageNumber < 1) PageNumber = 1;
            if (PageNumber > TotalPages && TotalPages > 0) PageNumber = TotalPages;

            // Apply pagination
            PurchaseOrders = allPOs
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
