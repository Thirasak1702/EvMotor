using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Purchasing.PR;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IPurchaseRequisitionService _prService;
    private readonly IItemService _itemService;

    public InfoModel(IPurchaseRequisitionService prService, IItemService itemService)
    {
        _prService = prService;
        _itemService = itemService;
    }

    [BindProperty]
    public PurchaseRequisitionDto PR { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();

    public async Task OnGetAsync(int? id)
    {
        await LoadItems();

        if (id.HasValue)
        {
            var result = await _prService.GetByIdAsync(id.Value);
            if (result.Success && result.Data != null)
            {
                PR = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                RedirectToPage("./Index");
            }
        }
        else
        {
            PR = new PurchaseRequisitionDto
            {
                Date = DateTime.Today,
                Status = "Draft",
                DocumentNumber = "Auto-generated"
            };
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadItems();
            return Page();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (PR.Id == 0)
        {
            var result = await _prService.CreateAsync(PR, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                await LoadItems();
                return Page();
            }
        }
        else
        {
            var result = await _prService.UpdateAsync(PR.Id, PR, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                await LoadItems();
                return Page();
            }
        }
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
