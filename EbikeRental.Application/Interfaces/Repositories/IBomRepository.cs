using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IBomRepository : IRepository<BillOfMaterial>
{
    Task<BillOfMaterial?> GetByIdWithDetailsAsync(int id);
    Task<BillOfMaterial?> GetByCodeWithDetailsAsync(string bomCode);
    Task<List<BillOfMaterial>> GetAllWithDetailsAsync();
    Task<List<BillOfMaterial>> GetByParentItemIdWithDetailsAsync(int parentItemId);
    Task<bool> BomCodeExistsAsync(string bomCode, int? excludeId = null);
    Task<string> GenerateBomCodeAsync();
}

