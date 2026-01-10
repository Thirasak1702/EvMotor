using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IProductionReceiptRepository : IRepository<ProductionReceipt>
{
    Task<ProductionReceipt?> GetByIdWithItemsAsync(int id);
    Task<List<ProductionReceipt>> GetAllWithItemsAsync();
    Task<string> GenerateDocumentNumberAsync();
    Task<List<ProductionReceipt>> GetByProductionOrderIdAsync(int productionOrderId);
}
