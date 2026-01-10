using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Infrastructure.Data;
using EbikeRental.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class RepairRepository : Repository<RepairOrder>, IRepairRepository
{
    public RepairRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<RepairOrder>> GetPendingRepairsAsync()
    {
        return await _dbSet
            .Where(r => r.Status == RepairStatus.Requested || r.Status == RepairStatus.Pending)
            .Include(r => r.Asset)
            .ToListAsync();
    }

    public async Task<PagedResult<RepairOrder>> GetPagedRepairOrdersAsync(RepairOrderFilterParameters filter)
    {
        var query = _dbSet
            .Include(r => r.Asset)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.OrderNumber))
        {
            query = query.Where(r => r.OrderNumber.Contains(filter.OrderNumber));
        }

        if (!string.IsNullOrWhiteSpace(filter.AssetCode))
        {
            query = query.Where(r => r.Asset.AssetCode.Contains(filter.AssetCode));
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<RepairStatus>(filter.Status, out var status))
            {
                query = query.Where(r => r.Status == status);
            }
        }

        if (filter.RequestDateFrom.HasValue)
        {
            query = query.Where(r => r.RequestedDate >= filter.RequestDateFrom.Value);
        }

        if (filter.RequestDateTo.HasValue)
        {
            query = query.Where(r => r.RequestedDate <= filter.RequestDateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(r =>
                r.OrderNumber.Contains(filter.SearchTerm) ||
                r.Asset.AssetCode.Contains(filter.SearchTerm));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.RequestedDate)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<RepairOrder>(items, totalCount, filter.PageNumber, filter.PageSize);
    }
}
