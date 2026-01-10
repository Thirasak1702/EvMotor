using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Masters.Items;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IItemService _itemService;

    public InfoModel(IItemService itemService)
    {
        _itemService = itemService;
    }

    [BindProperty]
    public ItemDto Item { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var result = await _itemService.GetByIdAsync(id.Value);
            if (result.Success)
            {
                Item = result.Data;
                return Page();
            }
            return RedirectToPage("./Index");
        }

        Item.IsActive = true;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Item.Id == 0)
        {
            var result = await _itemService.CreateAsync(Item);
            if (result.Success)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
        }
        else
        {
            var result = await _itemService.UpdateAsync(Item);
            if (result.Success)
            {
                return RedirectToPage("./Index");
            }
            ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
        }

        return Page();
    }
}
