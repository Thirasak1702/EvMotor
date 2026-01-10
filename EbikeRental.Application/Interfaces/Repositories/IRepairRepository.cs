using EbikeRental.Application.Common.Filters;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IRepairRepository : IRepository<RepairOrder>
{
    Task<List<RepairOrder>> GetPendingRepairsAsync();
    Task<PagedResult<RepairOrder>> GetPagedRepairOrdersAsync(RepairOrderFilterParameters filter);
}
