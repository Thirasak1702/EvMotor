using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class MaterialIssueRepository : Repository<MaterialIssue>, IMaterialIssueRepository
{
    public MaterialIssueRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<MaterialIssue>> GetAllWithDetailsAsync()
    {
        return await _context.MaterialIssues
            .Include(mi => mi.Items)
                .ThenInclude(i => i.Item)
            .Include(mi => mi.ProductionOrder)
            .Include(mi => mi.Warehouse)
            .OrderByDescending(mi => mi.CreatedAt)
            .ToListAsync();
    }

    public async Task<MaterialIssue?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.MaterialIssues
            .Include(mi => mi.Items)
                .ThenInclude(i => i.Item)
            .Include(mi => mi.ProductionOrder)
            .Include(mi => mi.Warehouse)
            .FirstOrDefaultAsync(mi => mi.Id == id);
    }

    public async Task<List<MaterialIssue>> GetByProductionOrderIdAsync(int poId)
    {
        return await _context.MaterialIssues
            .Include(mi => mi.Items)
                .ThenInclude(i => i.Item)
            .Where(mi => mi.ProductionOrderId == poId)
            .OrderByDescending(mi => mi.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateDocumentNumberAsync()
    {
        var lastMI = await _context.MaterialIssues
            .OrderByDescending(mi => mi.Id)
            .FirstOrDefaultAsync();

        var lastNumber = lastMI?.Id ?? 0;
        return $"MI-{DateTime.Now:yyyyMMdd}-{(lastNumber + 1):D4}";
    }
}
