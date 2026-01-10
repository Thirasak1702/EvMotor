using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Quality.QC;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IQualityCheckService _qcService;

    public IndexModel(IQualityCheckService qcService)
    {
        _qcService = qcService;
    }

    public List<QualityCheckDto> QualityChecks { get; set; } = new();
    
    // Filter properties
    [BindProperty(SupportsGet = true)]
    public string? DocumentNumber { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? CheckType { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _qcService.GetAllAsync();
        if (result.Success && result.Data != null)
        {
            QualityChecks = result.Data;
            
            // Apply filters
            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                QualityChecks = QualityChecks.Where(q => 
                    q.DocumentNumber.Contains(DocumentNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            if (FromDate.HasValue)
            {
                QualityChecks = QualityChecks.Where(q => q.InspectionDate >= FromDate.Value).ToList();
            }
            
            if (ToDate.HasValue)
            {
                QualityChecks = QualityChecks.Where(q => q.InspectionDate <= ToDate.Value).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(CheckType))
            {
                QualityChecks = QualityChecks.Where(q => 
                    q.CheckType.ToString().Equals(CheckType, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(Status))
            {
                QualityChecks = QualityChecks.Where(q => 
                    q.Status.ToString().Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _qcService.DeleteAsync(id);
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
