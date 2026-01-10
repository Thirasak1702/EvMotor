using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class PurchaseRequisitionRepository : Repository<PurchaseRequisition>, IPurchaseRequisitionRepository
{
    public PurchaseRequisitionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PurchaseRequisition?> GetByIdWithItemsAsync(int id)
    {
        return await _context.PurchaseRequisitions
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<PurchaseRequisition>> GetAllWithItemsAsync()
    {
        return await _context.PurchaseRequisitions
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateDocumentNumberAsync()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var prefix = $"PR{year:D4}{month:D2}";
        
        var lastDoc = await _context.PurchaseRequisitions
            .Where(x => x.DocumentNumber.StartsWith(prefix))
            .OrderByDescending(x => x.DocumentNumber)
            .FirstOrDefaultAsync();

        if (lastDoc == null)
        {
            return $"{prefix}0001";
        }

        var lastNumber = int.Parse(lastDoc.DocumentNumber.Substring(prefix.Length));
        return $"{prefix}{(lastNumber + 1):D4}";
    }
}
