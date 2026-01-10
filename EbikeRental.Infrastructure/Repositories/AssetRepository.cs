using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Infrastructure.Data;
using EbikeRental.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class AssetRepository : Repository<Asset>, IAssetRepository
{
    public AssetRepository(AppDbContext context) : base(context)
    {
    }

    public new async Task<Asset?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Item)
            .Include(a => a.CurrentWarehouse)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public new async Task<List<Asset>> GetAllAsync()
    {
        return await _dbSet
            .Include(a => a.Item)
            .Include(a => a.CurrentWarehouse)
            .ToListAsync();
    }

    public async Task<List<Asset>> GetAvailableAssetsAsync()
    {
        return await _dbSet
            .Where(a => a.Status == AssetStatus.Available && a.IsActive)
            .Include(a => a.Item)
            .Include(a => a.CurrentWarehouse)
            .ToListAsync();
    }

    public async Task<PagedResult<Asset>> GetPagedAssetsAsync(AssetFilterParameters filter)
    {
        var query = _dbSet
            .Include(a => a.Item)
            .Include(a => a.CurrentWarehouse)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.AssetCode))
        {
            query = query.Where(a => a.AssetCode.Contains(filter.AssetCode));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(a => a.Item != null && a.Item.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(a => a.Item != null && a.Item.Category.Contains(filter.Category));
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<AssetStatus>(filter.Status, out var status))
            {
                query = query.Where(a => a.Status == status);
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(a =>
                a.AssetCode.Contains(filter.SearchTerm) ||
                (a.Item != null && a.Item.Name.Contains(filter.SearchTerm)) ||
                (a.Item != null && a.Item.Category.Contains(filter.SearchTerm)));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(a => a.AssetCode)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Asset>(items, totalCount, filter.PageNumber, filter.PageSize);
    }
}
