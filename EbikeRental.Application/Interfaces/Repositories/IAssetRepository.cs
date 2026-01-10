using EbikeRental.Application.Common.Filters;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IAssetRepository : IRepository<Asset>
{
    Task<List<Asset>> GetAvailableAssetsAsync();
    Task<PagedResult<Asset>> GetPagedAssetsAsync(AssetFilterParameters filter);
}
