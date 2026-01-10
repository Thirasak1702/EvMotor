using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class GoodsReceiptRepository : Repository<GoodsReceipt>, IGoodsReceiptRepository
{
    public GoodsReceiptRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<GoodsReceipt?> GetByIdWithItemsAsync(int id)
    {
        return await _context.GoodsReceipts
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.PurchaseOrder)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<GoodsReceipt>> GetAllWithItemsAsync()
    {
        return await _context.GoodsReceipts
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.PurchaseOrder)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateDocumentNumberAsync()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var prefix = $"GR{year:D4}{month:D2}";
        
        var lastDoc = await _context.GoodsReceipts
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
