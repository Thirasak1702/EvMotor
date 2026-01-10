using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IQualityCheckRepository : IRepository<QualityCheck>
{
    Task<List<QualityCheck>> GetAllWithDetailsAsync();
    Task<QualityCheck?> GetByIdWithDetailsAsync(int id);
    Task<List<QualityCheck>> GetByGoodsReceiptIdAsync(int grId);
    Task<List<QualityCheck>> GetByProductionOrderIdAsync(int poId);
    Task<string> GenerateDocumentNumberAsync();
}
