using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class ProductionReceiptRepository : Repository<ProductionReceipt>, IProductionReceiptRepository
{
    public ProductionReceiptRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ProductionReceipt?> GetByIdWithItemsAsync(int id)
    {
        return await _context.ProductionReceipts
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.ProductionOrder)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ProductionReceipt>> GetAllWithItemsAsync()
    {
        return await _context.ProductionReceipts
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.ProductionOrder)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateDocumentNumberAsync()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var prefix = $"PRD{year:D4}{month:D2}";
        
        var lastDoc = await _context.ProductionReceipts
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

    public async Task<List<ProductionReceipt>> GetByProductionOrderIdAsync(int productionOrderId)
    {
        return await _context.ProductionReceipts
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.ProductionOrderId == productionOrderId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}
