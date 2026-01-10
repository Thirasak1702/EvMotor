using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Inventory.Stock;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IStockBalanceRepository _stockBalanceRepository;
    private readonly IItemService _itemService;
    private readonly IWarehouseService _warehouseService;

    public IndexModel(
        IStockBalanceRepository stockBalanceRepository,
        IItemService itemService,
        IWarehouseService warehouseService)
    {
        _stockBalanceRepository = stockBalanceRepository;
        _itemService = itemService;
        _warehouseService = warehouseService;
    }

    public List<StockBalance> StockBalances { get; set; } = new();
    public List<ItemDto> Items { get; set; } = new();
    public List<WarehouseDto> Warehouses { get; set; } = new();
    
    // Filter properties
    [BindProperty(SupportsGet = true)]
    public string? ItemFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int? WarehouseFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? StockStatus { get; set; }

    public async Task OnGetAsync()
    {
        // Load all stock balances
        var allBalances = await _stockBalanceRepository.GetAllAsync();
        StockBalances = allBalances.ToList();
        
        // Apply filters
        if (!string.IsNullOrWhiteSpace(ItemFilter))
        {
            StockBalances = StockBalances.Where(s => 
                s.Item != null && 
                (s.Item.Code.Contains(ItemFilter, StringComparison.OrdinalIgnoreCase) ||
                 s.Item.Name.Contains(ItemFilter, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
        
        if (WarehouseFilter.HasValue)
        {
            StockBalances = StockBalances.Where(s => s.WarehouseId == WarehouseFilter.Value).ToList();
        }
        
        // Stock status filter
        if (!string.IsNullOrWhiteSpace(StockStatus))
        {
            StockBalances = StockStatus switch
            {
                "InStock" => StockBalances.Where(s => s.QuantityOnHand > 0).ToList(),
                "OutOfStock" => StockBalances.Where(s => s.QuantityOnHand <= 0).ToList(),
                "LowStock" => StockBalances.Where(s => s.QuantityOnHand > 0 && s.QuantityOnHand < 10).ToList(),
                _ => StockBalances
            };
        }
        
        // Load dropdowns
        await LoadDropdowns();
    }
    
    private async Task LoadDropdowns()
    {
        var itemsResult = await _itemService.GetAllAsync();
        if (itemsResult.Success && itemsResult.Data != null)
            Items = itemsResult.Data;
        
        var warehousesResult = await _warehouseService.GetAllAsync();
        if (warehousesResult.Success && warehousesResult.Data != null)
            Warehouses = warehousesResult.Data;
    }
}
