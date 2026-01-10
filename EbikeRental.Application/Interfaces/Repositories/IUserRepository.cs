using EbikeRental.Application.Common.Filters;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces.Repositories;

public interface IUserRepository
{
    // Identity handles most user operations, but we might need custom queries
    Task<List<AppUser>> GetUsersByRoleAsync(string role);
    Task<PagedResult<AppUser>> GetPagedUsersAsync(UserFilterParameters filter);
}
