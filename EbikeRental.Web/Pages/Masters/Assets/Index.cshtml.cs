using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Shared.Constants;
using EbikeRental.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Assets;

[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Warehouse}")]
public class IndexModel : PageModel
{
    private readonly IAssetService _assetService;

    public IndexModel(IAssetService assetService)
    {
        _assetService = assetService;
    }

    public PagedResult<AssetDto> Assets { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? AssetCode { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Name { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task OnGetAsync()
    {
        var filter = new AssetFilterParameters
        {
            AssetCode = AssetCode,
            Name = Name,
            Category = Category,
            Status = Status,
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        var result = await _assetService.GetPagedAsync(filter);
        if (result.Success && result.Data != null)
        {
            Assets = result.Data;
        }
    }
}
