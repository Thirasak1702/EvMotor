using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Purchasing.PO;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IPurchaseOrderService _poService;
    private readonly IItemService _itemService;
    private readonly IPurchaseRequisitionService _prService;

    public InfoModel(IPurchaseOrderService poService, IItemService itemService, IPurchaseRequisitionService prService)
    {
        _poService = poService;
        _itemService = itemService;
        _prService = prService;
    }

    [BindProperty]
    public PurchaseOrderDto PO { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();
    public List<PurchaseRequisitionDto> ApprovedPRs { get; set; } = new();

    public async Task OnGetAsync(int? id)
    {
        await LoadItems();
        await LoadApprovedPRs();

        if (id.HasValue)
        {
            var result = await _poService.GetByIdAsync(id.Value);
            if (result.Success && result.Data != null)
            {
                PO = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                RedirectToPage("./Index");
            }
        }
        else
        {
            PO = new PurchaseOrderDto
            {
                OrderDate = DateTime.Today,
                ExpectedDeliveryDate = DateTime.Today.AddDays(7),
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
            await LoadApprovedPRs();
            return Page();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (PO.Id == 0)
        {
            var result = await _poService.CreateAsync(PO, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                await LoadItems();
                await LoadApprovedPRs();
                return Page();
            }
        }
        else
        {
            var result = await _poService.UpdateAsync(PO.Id, PO, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                await LoadItems();
                await LoadApprovedPRs();
                return Page();
            }
        }
    }

    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> OnGetPRDetailsAsync(int prId)
    {
        var result = await _prService.GetByIdAsync(prId);
        if (result.Success && result.Data != null)
        {
            var pr = result.Data;
            var response = new
            {
                success = true,
                id = pr.Id,
                documentNumber = pr.DocumentNumber,
                departmentName = pr.DepartmentName,
                items = pr.Items.Select(i => new
                {
                    id = i.Id,
                    itemId = i.ItemId,
                    itemCode = i.ItemCode,
                    itemName = i.ItemName,
                    description = i.Description,
                    quantity = i.Quantity,
                    unitOfMeasure = i.UnitOfMeasure,
                    estimatedUnitPrice = i.EstimatedUnitPrice,
                    notes = i.Notes
                }).ToList()
            };
            return new JsonResult(response);
        }
        return new JsonResult(new { success = false, message = result.Message });
    }

    private async Task LoadItems()
    {
        var result = await _itemService.GetAllAsync();
        if (result.Success)
        {
            Items = result.Data;
        }
    }

    private async Task LoadApprovedPRs()
    {
        var result = await _prService.GetApprovedPurchaseRequisitionsAsync();
        if (result.Success)
        {
            ApprovedPRs = result.Data;
        }
    }
}
