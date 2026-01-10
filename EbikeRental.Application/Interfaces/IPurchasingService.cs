using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IPurchasingService
{
    Task<Result<List<PurchaseDto>>> GetAllAsync();
    Task<Result<int>> CreateRequestAsync(PurchaseDto purchaseDto);
    Task<Result> ApproveRequestAsync(int id);
}
