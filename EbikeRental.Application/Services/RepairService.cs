using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Services;

public class RepairService : IRepairService
{
    private readonly IRepairRepository _repairRepository;
    private readonly IAssetRepository _assetRepository;

    public RepairService(IRepairRepository repairRepository, IAssetRepository assetRepository)
    {
        _repairRepository = repairRepository;
        _assetRepository = assetRepository;
    }

    public async Task<Result<List<RepairDto>>> GetAllAsync()
    {
        var repairs = await _repairRepository.GetAllAsync();
        var dtos = repairs.Select(r => new RepairDto
        {
            Id = r.Id,
            OrderNumber = r.OrderNumber,
            AssetId = r.AssetId,
            Description = r.Description,
            Status = r.Status,
            AssignedTechnicianId = r.AssignedTechnicianId,
            RequestedDate = r.RequestedDate,
            EstimatedCost = r.EstimatedCost
        }).ToList();

        return Result<List<RepairDto>>.Ok(dtos);
    }

    public async Task<Result<PagedResult<RepairDto>>> GetPagedAsync(RepairOrderFilterParameters filter)
    {
        var pagedRepairs = await _repairRepository.GetPagedRepairOrdersAsync(filter);
        
        var dtos = pagedRepairs.Items.Select(r => new RepairDto
        {
            Id = r.Id,
            OrderNumber = r.OrderNumber,
            AssetId = r.AssetId,
            AssetCode = r.Asset?.AssetCode ?? string.Empty,
            Description = r.Description,
            Status = r.Status,
            AssignedTechnicianId = r.AssignedTechnicianId,
            RequestedDate = r.RequestedDate,
            EstimatedCost = r.EstimatedCost
        }).ToList();

        var result = new PagedResult<RepairDto>(dtos, pagedRepairs.TotalCount, pagedRepairs.PageNumber, pagedRepairs.PageSize);
        return Result<PagedResult<RepairDto>>.Ok(result);
    }

    public async Task<Result<RepairDto>> GetByIdAsync(int id)
    {
        var repair = await _repairRepository.GetByIdAsync(id);
        if (repair == null) return Result<RepairDto>.Fail("Repair order not found");

        var dto = new RepairDto
        {
            Id = repair.Id,
            OrderNumber = repair.OrderNumber,
            AssetId = repair.AssetId,
            Description = repair.Description,
            Status = repair.Status,
            AssignedTechnicianId = repair.AssignedTechnicianId,
            RequestedDate = repair.RequestedDate,
            EstimatedCost = repair.EstimatedCost
        };

        return Result<RepairDto>.Ok(dto);
    }

    public async Task<Result<int>> CreateRequestAsync(RepairDto repairDto)
    {
        var asset = await _assetRepository.GetByIdAsync(repairDto.AssetId);
        if (asset == null) return Result<int>.Fail("Asset not found");

        var repair = new RepairOrder
        {
            OrderNumber = repairDto.OrderNumber,
            AssetId = repairDto.AssetId,
            Description = repairDto.Description,
            Status = RepairStatus.Requested,
            RequestedDate = DateTime.UtcNow,
            EstimatedCost = repairDto.EstimatedCost
        };

        await _repairRepository.AddAsync(repair);

        // Update asset status
        asset.Status = AssetStatus.UnderRepair; // Or InMaintenance
        await _assetRepository.UpdateAsync(asset);

        return Result<int>.Ok(repair.Id, "Repair request created successfully");
    }

    public async Task<Result> StartRepairAsync(int repairId, int technicianId)
    {
        var repair = await _repairRepository.GetByIdAsync(repairId);
        if (repair == null) return Result.Fail("Repair order not found");

        repair.Status = RepairStatus.InProgress;
        repair.AssignedTechnicianId = technicianId;
        repair.StartedDate = DateTime.UtcNow;

        await _repairRepository.UpdateAsync(repair);
        return Result.Ok("Repair started");
    }

    public async Task<Result> CompleteRepairAsync(int repairId, decimal cost, string notes)
    {
        var repair = await _repairRepository.GetByIdAsync(repairId);
        if (repair == null) return Result.Fail("Repair order not found");

        repair.Status = RepairStatus.Completed;
        repair.ActualCost = cost;
        repair.RepairNotes = notes;
        repair.CompletedDate = DateTime.UtcNow;

        await _repairRepository.UpdateAsync(repair);

        // Update asset status
        var asset = await _assetRepository.GetByIdAsync(repair.AssetId);
        if (asset != null)
        {
            asset.Status = AssetStatus.Available;
            await _assetRepository.UpdateAsync(asset);
        }

        return Result.Ok("Repair completed");
    }
}

