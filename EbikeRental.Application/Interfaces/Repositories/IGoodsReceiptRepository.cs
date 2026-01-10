using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IGoodsReceiptRepository : IRepository<GoodsReceipt>
{
    Task<GoodsReceipt?> GetByIdWithItemsAsync(int id);
    Task<List<GoodsReceipt>> GetAllWithItemsAsync();
    Task<string> GenerateDocumentNumberAsync();
}
