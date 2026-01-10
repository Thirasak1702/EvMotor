using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Shared;

namespace EbikeRental.Application.Workflows;

public class RentalWorkflow
{
    public Result CanRent(Asset asset)
    {
        if (asset.Status != AssetStatus.Available)
        {
            return Result.Fail($"Asset {asset.AssetCode} is not available for rental. Current status: {asset.Status}");
        }
        
        if (!asset.IsActive)
        {
            return Result.Fail($"Asset {asset.AssetCode} is inactive.");
        }

        return Result.Ok();
    }

    public Result CanReturn(RentalContract contract)
    {
        if (contract.Status != RentalStatus.Active && contract.Status != RentalStatus.Overdue)
        {
            return Result.Fail($"Contract {contract.ContractNumber} is not active.");
        }

        return Result.Ok();
    }
}
