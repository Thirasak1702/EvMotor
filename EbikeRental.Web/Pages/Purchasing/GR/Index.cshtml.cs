using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Purchasing.GR;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IGoodsReceiptService _grService;

    public IndexModel(IGoodsReceiptService grService)
    {
        _grService = grService;
    }

    public List<GoodsReceiptDto> GoodsReceipts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? DocumentNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PONumber { get; set; }

    // Pagination properties
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; private set; }
    public int TotalPages { get; private set; }

    public async Task OnGetAsync()
    {
        var result = await _grService.GetAllAsync();
        if (result.Success)
        {
            var allGRs = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                allGRs = allGRs.Where(gr => gr.DocumentNumber.Contains(DocumentNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                allGRs = allGRs.Where(gr => gr.ReceiptDate.Date >= FromDate.Value.Date).ToList();
            }

            if (ToDate.HasValue)
            {
                allGRs = allGRs.Where(gr => gr.ReceiptDate.Date <= ToDate.Value.Date).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                allGRs = allGRs.Where(gr => gr.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(PONumber))
            {
                allGRs = allGRs.Where(gr => gr.PurchaseOrderNumber.Contains(PONumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Calculate pagination
            TotalItems = allGRs.Count;
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Ensure valid page number
            if (PageNumber < 1) PageNumber = 1;
            if (PageNumber > TotalPages && TotalPages > 0) PageNumber = TotalPages;

            // Apply pagination
            GoodsReceipts = allGRs
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }

    // Post Handler
    public async Task<IActionResult> OnPostPostAsync(int id)
    {
        try
        {
            var userId = 1; // TODO: Get from User.Identity
            var result = await _grService.PostAsync(id, userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error posting goods receipt: {ex.Message}";
        }

        return RedirectToPage();
    }

    // Cancel Handler
    public async Task<IActionResult> OnPostCancelAsync(int id, string reason)
    {
        try
        {
            var userId = 1; // TODO: Get from User.Identity
            var result = await _grService.CancelAsync(id, userId, reason);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error cancelling goods receipt: {ex.Message}";
        }

        return RedirectToPage();
    }

    // Delete Handler
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var result = await _grService.DeleteAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting goods receipt: {ex.Message}";
        }

        return RedirectToPage();
    }
}
