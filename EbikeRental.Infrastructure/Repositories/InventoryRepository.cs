using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using EbikeRental.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class InventoryRepository : Repository<Item>, IInventoryRepository
{
    public InventoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> GetStockQuantityAsync(int itemId, int warehouseId)
    {
        // This is a simplified implementation. 
        // In a real system, you would query a Stock table or calculate from transactions.
        // For now, we'll count assets in the warehouse for that item.
        return await _context.Assets
            .CountAsync(a => a.ItemId == itemId && a.CurrentWarehouseId == warehouseId);
    }

    public async Task<PagedResult<Item>> GetPagedItemsAsync(ItemFilterParameters filter)
    {
        var query = _dbSet.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.ItemCode))
        {
            query = query.Where(i => i.Code.Contains(filter.ItemCode));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(i => i.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(i => i.Category.Contains(filter.Category));
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(i => i.IsActive == filter.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(i =>
                i.Code.Contains(filter.SearchTerm) ||
                i.Name.Contains(filter.SearchTerm) ||
                i.Category.Contains(filter.SearchTerm));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(i => i.Code)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Item>(items, totalCount, filter.PageNumber, filter.PageSize);
    }
}
