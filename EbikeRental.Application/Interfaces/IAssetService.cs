using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces;

public interface IAssetService
{
    Task<Result<List<AssetDto>>> GetAllAsync();
    Task<Result<PagedResult<AssetDto>>> GetPagedAsync(AssetFilterParameters filter);
    Task<Result<AssetDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(AssetDto assetDto);
    Task<Result> UpdateAsync(AssetDto assetDto);
    Task<Result> DeleteAsync(int id);
    Task<Result<List<AssetDto>>> GetAvailableAssetsAsync();
}
