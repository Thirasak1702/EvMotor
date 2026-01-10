using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetByIdWithItemsAsync(int id);
    Task<List<PurchaseOrder>> GetAllWithItemsAsync();
    Task<string> GenerateDocumentNumberAsync();
}
