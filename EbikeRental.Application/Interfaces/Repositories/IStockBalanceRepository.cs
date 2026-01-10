using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IStockBalanceRepository
{
    Task<IEnumerable<StockBalance>> GetAllAsync();
    Task<StockBalance?> GetByIdAsync(int id);
    Task<StockBalance?> GetByItemAndWarehouseAsync(int itemId, int warehouseId, string? batchNumber = null, string? serialNumber = null);
    Task<IEnumerable<StockBalance>> GetByItemAsync(int itemId);
    Task<IEnumerable<StockBalance>> GetByWarehouseAsync(int warehouseId);
    Task<decimal> GetAvailableQuantityAsync(int itemId, int warehouseId, string? batchNumber = null);
    Task AddAsync(StockBalance balance);
    Task UpdateAsync(StockBalance balance);
    Task DeleteAsync(StockBalance balance);
}
