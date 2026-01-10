using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Production.MaterialIssue;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMaterialIssueService _miService;

    public IndexModel(IMaterialIssueService miService)
    {
        _miService = miService;
    }

    public List<MaterialIssueDto> MaterialIssues { get; set; } = new();

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
        var result = await _miService.GetAllAsync();
        if (result.Success && result.Data != null)
        {
            MaterialIssues = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                MaterialIssues = MaterialIssues.Where(mi => mi.DocumentNumber.Contains(DocumentNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                MaterialIssues = MaterialIssues.Where(mi => mi.IssueDate >= FromDate.Value).ToList();
            }

            if (ToDate.HasValue)
            {
                MaterialIssues = MaterialIssues.Where(mi => mi.IssueDate <= ToDate.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                MaterialIssues = MaterialIssues.Where(mi => mi.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(ProductionOrderNumber))
            {
                MaterialIssues = MaterialIssues.Where(mi => 
                    !string.IsNullOrEmpty(mi.ProductionOrderNumber) && 
                    mi.ProductionOrderNumber.Contains(ProductionOrderNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }

    public async Task<IActionResult> OnPostPostAsync(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var result = await _miService.PostAsync(id, userId);
        
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
        var result = await _miService.DeleteAsync(id);
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
