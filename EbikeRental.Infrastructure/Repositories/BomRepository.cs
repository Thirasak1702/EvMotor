using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class BomRepository : Repository<BillOfMaterial>, IBomRepository
{
    public BomRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<BillOfMaterial?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.BillOfMaterials
            .Include(b => b.ParentItem)
            .Include(b => b.BomItems)
                .ThenInclude(bi => bi.ComponentItem)
            .Include(b => b.BomProcesses)
            .Include(b => b.BomQcs)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<BillOfMaterial>> GetAllWithDetailsAsync()
    {
        return await _context.BillOfMaterials
            .Include(b => b.ParentItem)
            .Include(b => b.BomItems)
                .ThenInclude(bi => bi.ComponentItem)
            .Include(b => b.BomProcesses)
            .Include(b => b.BomQcs)
            .OrderBy(b => b.BomCode)
            .ToListAsync();
    }

    public async Task<BillOfMaterial?> GetByCodeWithDetailsAsync(string bomCode)
    {
        return await _context.BillOfMaterials
            .Include(b => b.ParentItem)
            .Include(b => b.BomItems)
                .ThenInclude(bi => bi.ComponentItem)
            .Include(b => b.BomProcesses)
            .Include(b => b.BomQcs)
            .FirstOrDefaultAsync(b => b.BomCode == bomCode && b.IsActive);
    }

    public async Task<List<BillOfMaterial>> GetByParentItemIdWithDetailsAsync(int parentItemId)
    {
        return await _context.BillOfMaterials
            .Include(b => b.ParentItem)
            .Include(b => b.BomItems)
                .ThenInclude(bi => bi.ComponentItem)
            .Include(b => b.BomProcesses)
            .Include(b => b.BomQcs)
            .Where(b => b.ParentItemId == parentItemId)
            .OrderBy(b => b.Version)
            .ToListAsync();
    }

    public async Task<bool> BomCodeExistsAsync(string bomCode, int? excludeId = null)
    {
        var query = _context.BillOfMaterials.Where(b => b.BomCode == bomCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<string> GenerateBomCodeAsync()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var prefix = $"BOM{year:D4}{month:D2}";
        
        var lastBom = await _context.BillOfMaterials
            .Where(x => x.BomCode.StartsWith(prefix))
            .OrderByDescending(x => x.BomCode)
            .FirstOrDefaultAsync();

        if (lastBom == null)
        {
            return $"{prefix}0001";
        }

        var lastNumber = int.Parse(lastBom.BomCode.Substring(prefix.Length));
        return $"{prefix}{(lastNumber + 1):D4}";
    }
}

