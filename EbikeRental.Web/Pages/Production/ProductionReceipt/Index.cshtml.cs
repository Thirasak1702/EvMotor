using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Production.ProductionReceipt;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IProductionReceiptService _prService;

    public IndexModel(IProductionReceiptService prService)
    {
        _prService = prService;
    }

    public List<ProductionReceiptDto> ProductionReceipts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? DocumentNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ProductionOrderNumber { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _prService.GetAllAsync();
        if (result.Success && result.Data != null)
        {
            ProductionReceipts = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                ProductionReceipts = ProductionReceipts.Where(pr => pr.DocumentNumber.Contains(DocumentNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                ProductionReceipts = ProductionReceipts.Where(pr => pr.ReceiptDate >= FromDate.Value).ToList();
            }

            if (ToDate.HasValue)
            {
                ProductionReceipts = ProductionReceipts.Where(pr => pr.ReceiptDate <= ToDate.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                ProductionReceipts = ProductionReceipts.Where(pr => pr.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(ProductionOrderNumber))
            {
                ProductionReceipts = ProductionReceipts.Where(pr => 
                    !string.IsNullOrEmpty(pr.ProductionOrderNumber) && 
                    pr.ProductionOrderNumber.Contains(ProductionOrderNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }

    public async Task<IActionResult> OnPostPostAsync(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var result = await _prService.PostAsync(id, userId);
        
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _prService.DeleteAsync(id);
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }
        return RedirectToPage();
    }
}
