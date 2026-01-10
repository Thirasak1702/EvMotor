using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.BOM;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IBomService _bomService;
    private readonly IItemService _itemService;

    public InfoModel(IBomService bomService, IItemService itemService)
    {
        _bomService = bomService;
        _itemService = itemService;
    }

    [BindProperty]
    public BomDto Bom { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();

    public async Task OnGetAsync(int? id)
    {
        await LoadItems();

        if (id.HasValue)
        {
            var result = await _bomService.GetByIdAsync(id.Value);
            if (result.Success)
            {
                Bom = result.Data;
            }
        }
        else
        {
            Bom = new BomDto
            {
                BomCode = "Auto-generated",
                EffectiveDate = DateTime.Now,
                Version = "1.0",
                IsActive = true
            };
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Clear BomCode validation error when creating new BOM (will be auto-generated)
        if (Bom.Id == 0)
        {
            ModelState.Remove($"{nameof(Bom)}.{nameof(Bom.BomCode)}");
        }

        if (!ModelState.IsValid)
        {
            await LoadItems();
            return Page();
        }

        if (Bom.Id == 0)
        {
            var result = await _bomService.CreateAsync(Bom);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }
        else
        {
            var result = await _bomService.UpdateAsync(Bom);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
        }

        await LoadItems();
        return Page();
    }

    private async Task LoadItems()
    {
        var result = await _itemService.GetAllAsync();
        if (result.Success)
        {
            Items = result.Data;
        }
    }
}
