using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;

    public UserService(UserManager<AppUser> userManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserDto>>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Roles = roles.ToList(),
                IsActive = user.IsActive
            });
        }

        return Result<List<UserDto>>.Ok(userDtos);
    }

    public async Task<Result<PagedResult<UserDto>>> GetPagedAsync(UserFilterParameters filter)
    {
        var pagedUsers = await _userRepository.GetPagedUsersAsync(filter);
        var userDtos = new List<UserDto>();

        foreach (var user in pagedUsers.Items)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Roles = roles.ToList(),
                IsActive = user.IsActive
            });
        }

        var result = new PagedResult<UserDto>(userDtos, pagedUsers.TotalCount, pagedUsers.PageNumber, pagedUsers.PageSize);
        return Result<PagedResult<UserDto>>.Ok(result);
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return Result<UserDto>.Fail("User not found");

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

        return Result<UserDto>.Ok(userDto);
    }

    public async Task<Result<int>> CreateAsync(UserDto userDto, string password)
    {
        var user = new AppUser
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            IsActive = true,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return Result<int>.Fail("Failed to create user", result.Errors.Select(e => e.Description).ToArray());
        }

        if (userDto.Roles != null && userDto.Roles.Any())
        {
            await _userManager.AddToRolesAsync(user, userDto.Roles);
        }

        return Result<int>.Ok(user.Id, "User created successfully");
    }

    public async Task<Result> UpdateAsync(UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
        if (user == null) return Result.Fail("User not found");

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.IsActive = userDto.IsActive;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result.Fail("Failed to update user", result.Errors.Select(e => e.Description).ToArray());
        }

        // Update roles if needed
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(userDto.Roles).ToList();
        var rolesToAdd = userDto.Roles.Except(currentRoles).ToList();

        if (rolesToRemove.Any()) await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (rolesToAdd.Any()) await _userManager.AddToRolesAsync(user, rolesToAdd);

        return Result.Ok("User updated successfully");
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return Result.Fail("User not found");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return Result.Fail("Failed to delete user", result.Errors.Select(e => e.Description).ToArray());
        }

        return Result.Ok("User deleted successfully");
    }

    public async Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Fail("User not found");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return Result.Fail("Failed to change password", result.Errors.Select(e => e.Description).ToArray());
        }

        return Result.Ok("Password changed successfully");
    }
}
