using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IQualityCheckService
{
    Task<Result<List<QualityCheckDto>>> GetAllAsync();
    Task<Result<QualityCheckDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(QualityCheckDto dto, int userId);
    Task<Result> UpdateAsync(int id, QualityCheckDto dto, int userId);
    Task<Result> DeleteAsync(int id);
    Task<Result> ApproveAsync(int id, string approvedBy);
    Task<Result> RejectAsync(int id, string rejectedBy, string reason);
    Task<Result<List<QualityCheckDto>>> GetByGoodsReceiptIdAsync(int grId);
    Task<Result<List<QualityCheckDto>>> GetByProductionOrderIdAsync(int poId);
}
