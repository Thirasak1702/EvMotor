using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Warehouses;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IWarehouseService _warehouseService;

    public IndexModel(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    public List<WarehouseDto> Warehouses { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Code { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Name { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Location { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _warehouseService.GetAllAsync();
        if (result.Success)
        {
            Warehouses = result.Data;

            // Apply filters
            if (!string.IsNullOrWhiteSpace(Code))
            {
                Warehouses = Warehouses.Where(w => w.Code.Contains(Code, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Warehouses = Warehouses.Where(w => w.Name.Contains(Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Location))
            {
                Warehouses = Warehouses.Where(w => w.Location.Contains(Location, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (IsActive.HasValue)
            {
                Warehouses = Warehouses.Where(w => w.IsActive == IsActive.Value).ToList();
            }
        }
    }
}
