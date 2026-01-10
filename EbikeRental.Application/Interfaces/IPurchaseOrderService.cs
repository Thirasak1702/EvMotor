using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IPurchaseOrderService
{
    Task<Result<List<PurchaseOrderDto>>> GetAllAsync();
    Task<Result<PurchaseOrderDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(PurchaseOrderDto dto, int userId);
    Task<Result> UpdateAsync(int id, PurchaseOrderDto dto, int userId);
    Task<Result> DeleteAsync(int id);
    Task<Result> ConfirmAsync(int id, int userId);
    Task<Result> CancelAsync(int id, int userId, string reason);
    Task<Result<List<PurchaseOrderDto>>> GetApprovedPurchaseOrdersAsync();
}
