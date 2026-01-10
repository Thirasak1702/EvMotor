using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IAuthService
{
    Task<Result<UserDto>> LoginAsync(string email, string password);
    Task<Result> LogoutAsync();
}
