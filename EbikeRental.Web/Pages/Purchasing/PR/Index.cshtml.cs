using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Purchasing.PR;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IPurchaseRequisitionService _prService;

    public IndexModel(IPurchaseRequisitionService prService)
    {
        _prService = prService;
    }

    public List<PurchaseRequisitionDto> PurchaseRequisitions { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? DocumentNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Department { get; set; }

    // Pagination properties
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; private set; }
    public int TotalPages { get; private set; }

    public async Task OnGetAsync()
    {
        var result = await _prService.GetAllAsync();
        if (result.Success)
        {
            var allPRs = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                allPRs = allPRs.Where(pr => pr.DocumentNumber.Contains(DocumentNumber, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (FromDate.HasValue)
            {
                allPRs = allPRs.Where(pr => pr.Date.Date >= FromDate.Value.Date).ToList();
            }

            if (ToDate.HasValue)
            {
                allPRs = allPRs.Where(pr => pr.Date.Date <= ToDate.Value.Date).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                allPRs = allPRs.Where(pr => pr.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Department))
            {
                allPRs = allPRs.Where(pr => pr.DepartmentName.Contains(Department, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Calculate pagination
            TotalItems = allPRs.Count;
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Ensure valid page number
            if (PageNumber < 1) PageNumber = 1;
            if (PageNumber > TotalPages && TotalPages > 0) PageNumber = TotalPages;

            // Apply pagination
            PurchaseRequisitions = allPRs
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
