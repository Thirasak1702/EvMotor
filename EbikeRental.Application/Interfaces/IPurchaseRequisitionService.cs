using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IPurchaseRequisitionService
{
    Task<Result<List<PurchaseRequisitionDto>>> GetAllAsync();
    Task<Result<PurchaseRequisitionDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(PurchaseRequisitionDto dto, int userId);
    Task<Result> UpdateAsync(int id, PurchaseRequisitionDto dto, int userId);
    Task<Result> DeleteAsync(int id);
    Task<Result> ApproveAsync(int id, int userId);
    Task<Result> RejectAsync(int id, int userId, string reason);
    Task<Result<List<PurchaseRequisitionDto>>> GetApprovedPurchaseRequisitionsAsync();
}
