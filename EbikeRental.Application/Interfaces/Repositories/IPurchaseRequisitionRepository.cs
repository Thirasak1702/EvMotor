using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IPurchaseRequisitionRepository : IRepository<PurchaseRequisition>
{
    Task<PurchaseRequisition?> GetByIdWithItemsAsync(int id);
    Task<List<PurchaseRequisition>> GetAllWithItemsAsync();
    Task<string> GenerateDocumentNumberAsync();
}
