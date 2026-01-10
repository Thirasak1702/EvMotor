using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IMaterialIssueService
{
    Task<Result<List<MaterialIssueDto>>> GetAllAsync();
    Task<Result<MaterialIssueDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(MaterialIssueDto dto, int userId);
    Task<Result> UpdateAsync(int id, MaterialIssueDto dto, int userId);
    Task<Result> DeleteAsync(int id);
    Task<Result> PostAsync(int id, int userId);
    Task<Result<MaterialIssueDto>> GenerateFromProductionOrderAsync(int productionOrderId);
    Task<Result<List<MaterialIssueDto>>> GetByProductionOrderIdAsync(int poId);
}
