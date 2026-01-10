using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class PurchasingService : IPurchasingService
{
    public async Task<Result<List<PurchaseDto>>> GetAllAsync()
    {
        // Placeholder
        return Result<List<PurchaseDto>>.Ok(new List<PurchaseDto>());
    }

    public async Task<Result<int>> CreateRequestAsync(PurchaseDto purchaseDto)
    {
        // Placeholder
        return Result<int>.Ok(1, "Purchase request created");
    }

    public async Task<Result> ApproveRequestAsync(int id)
    {
        // Placeholder
        return Result.Ok("Purchase request approved");
    }
}
