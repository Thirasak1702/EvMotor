using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EbikeRental.Application.Common.Filters;

namespace EbikeRental.Web.Pages.Masters.Items;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IItemService _itemService;

    public IndexModel(IItemService itemService)
    {
        _itemService = itemService;
    }

    public List<ItemDto> Items { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Code { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Name { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

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
        var filter = new ItemFilterParameters
        {
            ItemCode = Code,
            Name = Name,
            Category = Category,
            IsActive = IsActive,
            PageNumber = PageNumber <= 0 ? 1 : PageNumber,
            PageSize = PageSize <= 0 ? 10 : PageSize
        };

        var result = await _itemService.GetPagedAsync(filter);
        if (result.Success)
        {
            var paged = result.Data;
            Items = paged.Items;
            TotalItems = paged.TotalCount;
            TotalPages = paged.TotalPages;

            // Normalize current page values from response
            PageNumber = paged.PageNumber;
            PageSize = paged.PageSize;
        }
    }
}
