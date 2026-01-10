using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Warehouses;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IWarehouseService _warehouseService;

    public InfoModel(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [BindProperty]
    public WarehouseDto Warehouse { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var result = await _warehouseService.GetByIdAsync(id.Value);
            if (result.Success)
            {
                Warehouse = result.Data;
                return Page();
            }
            return RedirectToPage("./Index");
        }

        Warehouse.IsActive = true;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Warehouse.Id == 0)
        {
            var result = await _warehouseService.CreateAsync(Warehouse);
            if (result.Success)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
        }
        else
        {
            var result = await _warehouseService.UpdateAsync(Warehouse);
            if (result.Success)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
        }

        return Page();
    }
}
