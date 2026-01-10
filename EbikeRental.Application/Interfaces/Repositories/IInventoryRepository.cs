using EbikeRental.Application.Common.Filters;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IInventoryRepository : IRepository<Item>
{
    Task<int> GetStockQuantityAsync(int itemId, int warehouseId);
    Task<PagedResult<Item>> GetPagedItemsAsync(ItemFilterParameters filter);
}
