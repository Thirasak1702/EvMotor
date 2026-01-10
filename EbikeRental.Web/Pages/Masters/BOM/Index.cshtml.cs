using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.BOM;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IBomService _bomService;

    public IndexModel(IBomService bomService)
    {
        _bomService = bomService;
    }

    public List<BomDto> Boms { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? BomCode { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ParentItem { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Version { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    // Pagination properties
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; private set; }
    public int TotalPages { get; private set; }

    public async Task OnGetAsync()
    {
        var result = await _bomService.GetAllAsync();
        if (result.Success)
        {
            var allBoms = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(BomCode))
            {
                allBoms = allBoms.Where(b => b.BomCode.Contains(BomCode, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(ParentItem))
            {
                allBoms = allBoms.Where(b => b.ParentItemName.Contains(ParentItem, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Version))
            {
                allBoms = allBoms.Where(b => b.Version.Contains(Version, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (IsActive.HasValue)
            {
                allBoms = allBoms.Where(b => b.IsActive == IsActive.Value).ToList();
            }

            // Calculate pagination
            TotalItems = allBoms.Count;
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Ensure valid page number
            if (PageNumber < 1) PageNumber = 1;
            if (PageNumber > TotalPages && TotalPages > 0) PageNumber = TotalPages;

            // Apply pagination
            Boms = allBoms
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
