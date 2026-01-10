using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Infrastructure.Data;
using EbikeRental.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class RentalRepository : Repository<RentalContract>, IRentalRepository
{
    public RentalRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<RentalContract>> GetActiveContractsAsync()
    {
        return await _dbSet
            .Where(r => r.Status == RentalStatus.Active)
            .Include(r => r.Asset)
            .ToListAsync();
    }

    public async Task<PagedResult<RentalContract>> GetPagedRentalContractsAsync(RentalContractFilterParameters filter)
    {
        var query = _dbSet
            .Include(r => r.Asset)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.ContractNumber))
        {
            query = query.Where(r => r.ContractNumber.Contains(filter.ContractNumber));
        }

        if (!string.IsNullOrWhiteSpace(filter.CustomerName))
        {
            query = query.Where(r => r.CustomerName.Contains(filter.CustomerName));
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<RentalStatus>(filter.Status, out var status))
            {
                query = query.Where(r => r.Status == status);
            }
        }

        if (filter.StartDateFrom.HasValue)
        {
            query = query.Where(r => r.RentalStartDate >= filter.StartDateFrom.Value);
        }

        if (filter.StartDateTo.HasValue)
        {
            query = query.Where(r => r.RentalStartDate <= filter.StartDateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(r =>
                r.ContractNumber.Contains(filter.SearchTerm) ||
                r.CustomerName.Contains(filter.SearchTerm));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.RentalStartDate)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<RentalContract>(items, totalCount, filter.PageNumber, filter.PageSize);
    }
}
