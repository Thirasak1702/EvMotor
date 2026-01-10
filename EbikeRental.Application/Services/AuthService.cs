using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;
using Microsoft.AspNetCore.Identity;

namespace EbikeRental.Application.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<UserDto>.Fail("Invalid email or password");
        }

        if (!user.IsActive)
        {
            return Result<UserDto>.Fail("User account is inactive");
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
        
        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Roles = roles.ToList(),
                IsActive = user.IsActive
            };

            return Result<UserDto>.Ok(userDto, "Login successful");
        }

        return Result<UserDto>.Fail("Invalid email or password");
    }

    public async Task<Result> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return Result.Ok("Logout successful");
    }
}
