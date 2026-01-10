using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IProductionReceiptService
{
    Task<Result<List<ProductionReceiptDto>>> GetAllAsync();
    Task<Result<ProductionReceiptDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(ProductionReceiptDto dto, int userId);
    Task<Result> UpdateAsync(int id, ProductionReceiptDto dto, int userId);
    Task<Result> DeleteAsync(int id);
    Task<Result> PostAsync(int id, int userId);
    Task<Result> CancelAsync(int id, int userId, string reason);
    Task<Result<List<ProductionReceiptDto>>> GetByProductionOrderIdAsync(int productionOrderId);
}
