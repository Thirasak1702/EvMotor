using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly AppDbContext _context;

    public InventoryTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryTransaction>> GetAllAsync()
    {
        return await _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<InventoryTransaction?> GetByIdAsync(int id)
    {
        return await _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<InventoryTransaction>> GetByItemAsync(int itemId)
    {
        return await _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.ItemId == itemId)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryTransaction>> GetByWarehouseAsync(int warehouseId)
    {
        return await _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryTransaction>> GetByItemAndWarehouseAsync(int itemId, int warehouseId)
    {
        return await _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.ItemId == itemId && x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryTransaction>> GetByReferenceAsync(string referenceType, int referenceId)
    {
        return await _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<string> GenerateTransactionNumberAsync()
    {
        var prefix = $"IT-{DateTime.Now:yyyy}-";
        var lastTransaction = await _context.InventoryTransactions
            .Where(x => x.TransactionNumber.StartsWith(prefix))
            .OrderByDescending(x => x.TransactionNumber)
            .FirstOrDefaultAsync();

        if (lastTransaction == null)
        {
            return $"{prefix}0001";
        }

        var lastNumber = int.Parse(lastTransaction.TransactionNumber.Substring(prefix.Length));
        return $"{prefix}{(lastNumber + 1):D4}";
    }

    public async Task AddAsync(InventoryTransaction transaction)
    {
        Console.WriteLine($"      [REPO-TXN] AddAsync called");
        Console.WriteLine($"                 TxnNumber: {transaction.TransactionNumber}");
        Console.WriteLine($"                 ItemId: {transaction.ItemId}, Qty: {transaction.Quantity}");
        
        await _context.InventoryTransactions.AddAsync(transaction);
        
        Console.WriteLine($"      [REPO-TXN] Calling SaveChangesAsync...");
        var result = await _context.SaveChangesAsync();
        
        Console.WriteLine($"      [REPO-TXN] ? SaveChanges: {result} row(s) affected");
    }

    public async Task UpdateAsync(InventoryTransaction transaction)
    {
        _context.InventoryTransactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(InventoryTransaction transaction)
    {
        _context.InventoryTransactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }
}
