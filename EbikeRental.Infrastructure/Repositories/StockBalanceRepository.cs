using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class StockBalanceRepository : IStockBalanceRepository
{
    private readonly AppDbContext _context;

    public StockBalanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StockBalance>> GetAllAsync()
    {
        return await _context.StockBalances
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .OrderBy(x => x.Item!.Code)
            .ThenBy(x => x.Warehouse!.Code)
            .ToListAsync();
    }

    public async Task<StockBalance?> GetByIdAsync(int id)
    {
        return await _context.StockBalances
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<StockBalance?> GetByItemAndWarehouseAsync(int itemId, int warehouseId, string? batchNumber = null, string? serialNumber = null)
    {
        return await _context.StockBalances
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.ItemId == itemId 
                                   && x.WarehouseId == warehouseId
                                   && x.BatchNumber == batchNumber
                                   && x.SerialNumber == serialNumber);
    }

    public async Task<IEnumerable<StockBalance>> GetByItemAsync(int itemId)
    {
        return await _context.StockBalances
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.ItemId == itemId)
            .OrderBy(x => x.Warehouse!.Code)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockBalance>> GetByWarehouseAsync(int warehouseId)
    {
        return await _context.StockBalances
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId)
            .OrderBy(x => x.Item!.Code)
            .ToListAsync();
    }

    public async Task<decimal> GetAvailableQuantityAsync(int itemId, int warehouseId, string? batchNumber = null)
    {
        var balances = await _context.StockBalances
            .Where(x => x.ItemId == itemId && x.WarehouseId == warehouseId)
            .Where(x => batchNumber == null || x.BatchNumber == batchNumber)
            .ToListAsync();

        // Calculate available quantity in memory (QuantityOnHand - QuantityReserved)
        return balances.Sum(x => x.QuantityOnHand - x.QuantityReserved);
    }

    public async Task AddAsync(StockBalance balance)
    {
        Console.WriteLine($"      [REPO-BALANCE] AddAsync called");
        Console.WriteLine($"                     ItemId: {balance.ItemId}, WarehouseId: {balance.WarehouseId}");
        
        await _context.StockBalances.AddAsync(balance);
        
        Console.WriteLine($"      [REPO-BALANCE] Calling SaveChangesAsync...");
        var result = await _context.SaveChangesAsync();
        
        Console.WriteLine($"      [REPO-BALANCE] ? SaveChanges: {result} row(s) affected");
    }

    public async Task UpdateAsync(StockBalance balance)
    {
        Console.WriteLine($"      [REPO-BALANCE] UpdateAsync called");
        Console.WriteLine($"                     Id: {balance.Id}, Qty: {balance.QuantityOnHand}");
        
        _context.StockBalances.Update(balance);
        
        Console.WriteLine($"      [REPO-BALANCE] Calling SaveChangesAsync...");
        var result = await _context.SaveChangesAsync();
        
        Console.WriteLine($"      [REPO-BALANCE] ? SaveChanges: {result} row(s) affected");
    }

    public async Task DeleteAsync(StockBalance balance)
    {
        _context.StockBalances.Remove(balance);
        await _context.SaveChangesAsync();
    }
}
