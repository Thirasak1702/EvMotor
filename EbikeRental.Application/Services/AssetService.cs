using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;

    public AssetService(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<List<AssetDto>>> GetAllAsync()
    {
        var assets = await _assetRepository.GetAllAsync();
        // In a real app, use AutoMapper
        var dtos = assets.Select(a => new AssetDto
        {
            Id = a.Id,
            AssetCode = a.AssetCode,
            SerialNumber = a.SerialNumber,
            ItemId = a.ItemId,
            ItemName = a.Item?.Name ?? string.Empty,
            ItemCategory = a.Item?.Category ?? string.Empty,
            Status = a.Status,
            CurrentWarehouseId = a.CurrentWarehouseId,
            PurchaseCost = a.PurchaseCost,
            PurchaseDate = a.PurchaseDate,
            Notes = a.Notes,
            IsActive = a.IsActive
        }).ToList();

        return Result<List<AssetDto>>.Ok(dtos);
    }

    public async Task<Result<AssetDto>> GetByIdAsync(int id)
    {
        var asset = await _assetRepository.GetByIdAsync(id);
        if (asset == null) return Result<AssetDto>.Fail("Asset not found");

        var dto = new AssetDto
        {
            Id = asset.Id,
            AssetCode = asset.AssetCode,
            SerialNumber = asset.SerialNumber,
            ItemId = asset.ItemId,
            ItemName = asset.Item?.Name ?? string.Empty,
            ItemCategory = asset.Item?.Category ?? string.Empty,
            Status = asset.Status,
            CurrentWarehouseId = asset.CurrentWarehouseId,
            PurchaseCost = asset.PurchaseCost,
            PurchaseDate = asset.PurchaseDate,
            Notes = asset.Notes,
            IsActive = asset.IsActive
        };

        return Result<AssetDto>.Ok(dto);
    }

    public async Task<Result<int>> CreateAsync(AssetDto assetDto)
    {
        var asset = new Asset
        {
            AssetCode = assetDto.AssetCode,
            SerialNumber = assetDto.SerialNumber,
            ItemId = assetDto.ItemId,
            Status = assetDto.Status,
            CurrentWarehouseId = assetDto.CurrentWarehouseId,
            PurchaseCost = assetDto.PurchaseCost,
            PurchaseDate = assetDto.PurchaseDate,
            Notes = assetDto.Notes,
            IsActive = true
        };

        await _assetRepository.AddAsync(asset);
        return Result<int>.Ok(asset.Id, "Asset created successfully");
    }

    public async Task<Result> UpdateAsync(AssetDto assetDto)
    {
        var asset = await _assetRepository.GetByIdAsync(assetDto.Id);
        if (asset == null) return Result.Fail("Asset not found");

        asset.AssetCode = assetDto.AssetCode;
        asset.SerialNumber = assetDto.SerialNumber;
        asset.ItemId = assetDto.ItemId;
        asset.Status = assetDto.Status;
        asset.CurrentWarehouseId = assetDto.CurrentWarehouseId;
        asset.PurchaseCost = assetDto.PurchaseCost;
        asset.PurchaseDate = assetDto.PurchaseDate;
        asset.Notes = assetDto.Notes;
        asset.IsActive = assetDto.IsActive;

        await _assetRepository.UpdateAsync(asset);
        return Result.Ok("Asset updated successfully");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var asset = await _assetRepository.GetByIdAsync(id);
        if (asset == null) return Result.Fail("Asset not found");

        await _assetRepository.DeleteAsync(asset);
        return Result.Ok("Asset deleted successfully");
    }

    public async Task<Result<List<AssetDto>>> GetAvailableAssetsAsync()
    {
        var assets = await _assetRepository.GetAvailableAssetsAsync();
        var dtos = assets.Select(a => new AssetDto
        {
            Id = a.Id,
            AssetCode = a.AssetCode,
            SerialNumber = a.SerialNumber,
            ItemId = a.ItemId,
            ItemName = a.Item?.Name ?? string.Empty,
            ItemCategory = a.Item?.Category ?? string.Empty,
            Status = a.Status,
            CurrentWarehouseId = a.CurrentWarehouseId,
            CurrentWarehouseName = a.CurrentWarehouse?.Name,
            PurchaseCost = a.PurchaseCost,
            PurchaseDate = a.PurchaseDate,
            Notes = a.Notes,
            IsActive = a.IsActive
        }).ToList();

        return Result<List<AssetDto>>.Ok(dtos);
    }

    public async Task<Result<PagedResult<AssetDto>>> GetPagedAsync(AssetFilterParameters filter)
    {
        var pagedAssets = await _assetRepository.GetPagedAssetsAsync(filter);
        
        var dtos = pagedAssets.Items.Select(a => new AssetDto
        {
            Id = a.Id,
            AssetCode = a.AssetCode,
            SerialNumber = a.SerialNumber,
            ItemId = a.ItemId,
            ItemName = a.Item?.Name ?? string.Empty,
            ItemCategory = a.Item?.Category ?? string.Empty,
            Status = a.Status,
            CurrentWarehouseId = a.CurrentWarehouseId,
            CurrentWarehouseName = a.CurrentWarehouse?.Name,
            PurchaseCost = a.PurchaseCost,
            PurchaseDate = a.PurchaseDate,
            Notes = a.Notes,
            IsActive = a.IsActive
        }).ToList();

        var result = new PagedResult<AssetDto>(dtos, pagedAssets.TotalCount, pagedAssets.PageNumber, pagedAssets.PageSize);
        return Result<PagedResult<AssetDto>>.Ok(result);
    }
}
