using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IInventoryTransactionRepository
{
    Task<IEnumerable<InventoryTransaction>> GetAllAsync();
    Task<InventoryTransaction?> GetByIdAsync(int id);
    Task<IEnumerable<InventoryTransaction>> GetByItemAsync(int itemId);
    Task<IEnumerable<InventoryTransaction>> GetByWarehouseAsync(int warehouseId);
    Task<IEnumerable<InventoryTransaction>> GetByItemAndWarehouseAsync(int itemId, int warehouseId);
    Task<IEnumerable<InventoryTransaction>> GetByReferenceAsync(string referenceType, int referenceId);
    Task<string> GenerateTransactionNumberAsync();
    Task AddAsync(InventoryTransaction transaction);
    Task UpdateAsync(InventoryTransaction transaction);
    Task DeleteAsync(InventoryTransaction transaction);
}
