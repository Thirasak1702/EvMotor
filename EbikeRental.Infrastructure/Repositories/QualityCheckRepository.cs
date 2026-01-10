using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class QualityCheckRepository : Repository<QualityCheck>, IQualityCheckRepository
{
    public QualityCheckRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<QualityCheck>> GetAllWithDetailsAsync()
    {
        return await _context.QualityChecks
            .Include(qc => qc.Items)
                .ThenInclude(i => i.Item)
            .Include(qc => qc.GoodsReceipt)
            .Include(qc => qc.ProductionOrder)
            .OrderByDescending(qc => qc.CreatedAt)
            .ToListAsync();
    }

    public async Task<QualityCheck?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.QualityChecks
            .Include(qc => qc.Items)
                .ThenInclude(i => i.Item)
            .Include(qc => qc.GoodsReceipt)
            .Include(qc => qc.ProductionOrder)
            .FirstOrDefaultAsync(qc => qc.Id == id);
    }

    public async Task<List<QualityCheck>> GetByGoodsReceiptIdAsync(int grId)
    {
        return await _context.QualityChecks
            .Include(qc => qc.Items)
                .ThenInclude(i => i.Item)
            .Where(qc => qc.GoodsReceiptId == grId)
            .OrderByDescending(qc => qc.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<QualityCheck>> GetByProductionOrderIdAsync(int poId)
    {
        return await _context.QualityChecks
            .Include(qc => qc.Items)
                .ThenInclude(i => i.Item)
            .Where(qc => qc.ProductionOrderId == poId)
            .OrderByDescending(qc => qc.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateDocumentNumberAsync()
    {
        var lastQC = await _context.QualityChecks
            .OrderByDescending(qc => qc.Id)
            .FirstOrDefaultAsync();

        var lastNumber = lastQC?.Id ?? 0;
        return $"QC-{DateTime.Now:yyyyMMdd}-{(lastNumber + 1):D4}";
    }
}
