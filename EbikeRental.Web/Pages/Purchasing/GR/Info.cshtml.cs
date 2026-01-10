using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Purchasing.GR;

[Authorize]
public class InfoModel : PageModel
{
    private readonly IGoodsReceiptService _grService;
    private readonly IItemService _itemService;
    private readonly IWarehouseService _warehouseService;
    private readonly IPurchaseOrderService _poService;
    private readonly ILogger<InfoModel> _logger;

    public InfoModel(
        IGoodsReceiptService grService, 
        IItemService itemService, 
        IWarehouseService warehouseService, 
        IPurchaseOrderService poService,
        ILogger<InfoModel> logger)
    {
        _grService = grService;
        _itemService = itemService;
        _warehouseService = warehouseService;
        _poService = poService;
        _logger = logger;
    }

    [BindProperty]
    public GoodsReceiptDto GR { get; set; } = new();

    public List<ItemDto> Items { get; set; } = new();
    public List<WarehouseDto> Warehouses { get; set; } = new();
    public List<PurchaseOrderDto> ApprovedPOs { get; set; } = new();

    public string ItemsJson { get; private set; } = "[]";
    public string GrItemsJson { get; private set; } = "[]";

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        try
        {
            _logger.LogInformation("[GR INFO] Loading page. ID: {Id}", id);
            
            await LoadItems();
            await LoadWarehouses();
            await LoadApprovedPOs();

            if (id.HasValue)
            {
                _logger.LogInformation("[GR INFO] Loading GR with ID: {Id}", id.Value);
                var result = await _grService.GetByIdAsync(id.Value);
                
                if (result.Success && result.Data != null)
                {
                    GR = result.Data;
                    _logger.LogInformation("[GR INFO] GR loaded successfully. Doc: {DocNumber}", GR.DocumentNumber);
                }
                else
                {
                    _logger.LogWarning("[GR INFO] Failed to load GR. Message: {Message}", result.Message);
                    TempData["ErrorMessage"] = result.Message ?? "Goods receipt not found";
                    return RedirectToPage("./Index");
                }
            }
            else
            {
                _logger.LogInformation("[GR INFO] Creating new GR");
                GR = new GoodsReceiptDto
                {
                    ReceiptDate = DateTime.Today,
                    Status = "Draft",
                    DocumentNumber = "Auto-generated"
                };
            }

            PrepareJsonPayloads();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GR INFO] Error loading page. ID: {Id}", id);
            TempData["ErrorMessage"] = $"Error loading goods receipt: {ex.Message}";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadItems();
            await LoadWarehouses();
            await LoadApprovedPOs();
            PrepareJsonPayloads();
            return Page();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (GR.Id == 0)
        {
            // ? Create new GR
            var result = await _grService.CreateAsync(GR, userId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("./Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                await LoadItems();
                await LoadWarehouses();
                await LoadApprovedPOs();
                PrepareJsonPayloads();
                return Page();
            }
        }
        else
        {
            // ?? Check if status is changing from Draft to Posted
            var existingGR = await _grService.GetByIdAsync(GR.Id);
            
            if (existingGR.Success && existingGR.Data != null)
            {
                var oldStatus = existingGR.Data.Status;
                var newStatus = GR.Status;

                // ?? If changing from Draft to Posted, call PostAsync instead of UpdateAsync
                if (oldStatus == "Draft" && newStatus == "Posted")
                {
                    Console.WriteLine($"[INFO PAGE] Status change detected: Draft ? Posted");
                    Console.WriteLine($"[INFO PAGE] Calling PostAsync instead of UpdateAsync...");
                    
                    // First update the GR data (but keep status as Draft)
                    var originalStatus = GR.Status;
                    GR.Status = "Draft"; // Keep as Draft for update
                    
                    var updateResult = await _grService.UpdateAsync(GR.Id, GR, userId);
                    if (!updateResult.Success)
                    {
                        TempData["ErrorMessage"] = $"Failed to update GR: {updateResult.Message}";
                        await LoadItems();
                        await LoadWarehouses();
                        await LoadApprovedPOs();
                        PrepareJsonPayloads();
                        return Page();
                    }
                    
                    // Now call PostAsync to change status and add stock
                    var postResult = await _grService.PostAsync(GR.Id, userId);
                    if (postResult.Success)
                    {
                        TempData["SuccessMessage"] = $"Goods receipt posted successfully! {postResult.Message}";
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Failed to post GR: {postResult.Message}";
                        await LoadItems();
                        await LoadWarehouses();
                        await LoadApprovedPOs();
                        PrepareJsonPayloads();
                        return Page();
                    }
                }
                else
                {
                    // ? Normal update (status not changing to Posted)
                    var result = await _grService.UpdateAsync(GR.Id, GR, userId);
                    if (result.Success)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result.Message;
                        await LoadItems();
                        await LoadWarehouses();
                        await LoadApprovedPOs();
                        PrepareJsonPayloads();
                        return Page();
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Goods receipt not found";
                return RedirectToPage("./Index");
            }
        }
    }

    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> OnGetPODetailsAsync(int poId)
    {
        var result = await _poService.GetByIdAsync(poId);
        if (result.Success && result.Data != null)
        {
            var po = result.Data;
            var response = new
            {
                po.Id,
                po.DocumentNumber,
                po.VendorName,
                Items = po.Items.Select(i => new
                {
                    i.Id,
                    i.ItemId,
                    i.ItemCode,
                    i.ItemName,
                    i.Description,
                    i.Quantity,
                    i.UnitOfMeasure,
                    i.UnitPrice,
                    i.Notes
                }).ToList()
            };
            return new JsonResult(response);
        }
        return new JsonResult(new { success = false, message = result.Message });
    }

    private async Task LoadItems()
    {
        try
        {
            var result = await _itemService.GetAllAsync();
            if (result.Success)
            {
                Items = result.Data;
                _logger.LogInformation("[GR INFO] Loaded {Count} items", Items.Count);
            }
            else
            {
                Items = new List<ItemDto>();
                _logger.LogWarning("[GR INFO] No items loaded. Message: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GR INFO] Error loading items");
            Items = new List<ItemDto>();
        }
    }

    private async Task LoadWarehouses()
    {
        try
        {
            var result = await _warehouseService.GetAllAsync();
            if (result.Success)
            {
                Warehouses = result.Data;
                _logger.LogInformation("[GR INFO] Loaded {Count} warehouses", Warehouses.Count);
            }
            else
            {
                Warehouses = new List<WarehouseDto>();
                _logger.LogWarning("[GR INFO] No warehouses loaded. Message: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GR INFO] Error loading warehouses");
            Warehouses = new List<WarehouseDto>();
        }
    }

    private async Task LoadApprovedPOs()
    {
        try
        {
            var result = await _poService.GetApprovedPurchaseOrdersAsync();
            if (result.Success)
            {
                ApprovedPOs = result.Data;
                _logger.LogInformation("[GR INFO] Loaded {Count} approved POs", ApprovedPOs.Count);
            }
            else
            {
                ApprovedPOs = new List<PurchaseOrderDto>();
                _logger.LogWarning("[GR INFO] No approved POs loaded. Message: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GR INFO] Error loading approved POs");
            ApprovedPOs = new List<PurchaseOrderDto>();
        }
    }

    private void PrepareJsonPayloads()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        ItemsJson = JsonSerializer.Serialize(Items ?? new List<ItemDto>(), options);
        GrItemsJson = JsonSerializer.Serialize(GR.Items ?? new List<GoodsReceiptItemDto>(), options);
    }
}
