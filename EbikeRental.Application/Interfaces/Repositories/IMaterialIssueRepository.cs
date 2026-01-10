using EbikeRental.Domain.Entities;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IMaterialIssueRepository : IRepository<MaterialIssue>
{
    Task<List<MaterialIssue>> GetAllWithDetailsAsync();
    Task<MaterialIssue?> GetByIdWithDetailsAsync(int id);
    Task<List<MaterialIssue>> GetByProductionOrderIdAsync(int poId);
    Task<string> GenerateDocumentNumberAsync();
}
