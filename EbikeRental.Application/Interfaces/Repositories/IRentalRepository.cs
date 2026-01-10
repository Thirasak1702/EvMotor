using EbikeRental.Application.Common.Filters;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IRentalRepository : IRepository<RentalContract>
{
    Task<List<RentalContract>> GetActiveContractsAsync();
    Task<PagedResult<RentalContract>> GetPagedRentalContractsAsync(RentalContractFilterParameters filter);
}
