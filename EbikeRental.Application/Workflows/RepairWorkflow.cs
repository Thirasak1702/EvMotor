using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Shared;

namespace EbikeRental.Application.Workflows;

public class RepairWorkflow
{
    public Result CanRequestRepair(Asset asset)
    {
        // Can request repair from almost any state except maybe Retired/Lost
        if (asset.Status == AssetStatus.Retired || asset.Status == AssetStatus.Lost)
        {
            return Result.Fail($"Cannot request repair for asset {asset.AssetCode} with status {asset.Status}");
        }

        return Result.Ok();
    }

    public Result CanStartRepair(RepairOrder order)
    {
        if (order.Status != RepairStatus.Requested && order.Status != RepairStatus.Pending)
        {
            return Result.Fail($"Cannot start repair. Current status: {order.Status}");
        }

        return Result.Ok();
    }

    public Result CanCompleteRepair(RepairOrder order)
    {
        if (order.Status != RepairStatus.InProgress)
        {
            return Result.Fail($"Cannot complete repair. It must be In Progress first.");
        }

        return Result.Ok();
    }
}
