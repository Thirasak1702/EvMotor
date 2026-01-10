using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces;

public interface IUserService
{
    Task<Result<List<UserDto>>> GetAllAsync();
    Task<Result<PagedResult<UserDto>>> GetPagedAsync(UserFilterParameters filter);
    Task<Result<UserDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(UserDto userDto, string password);
    Task<Result> UpdateAsync(UserDto userDto);
    Task<Result> DeleteAsync(int id);
    Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}
