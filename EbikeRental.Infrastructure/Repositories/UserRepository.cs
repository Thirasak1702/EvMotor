using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Data;
using EbikeRental.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    public UserRepository(UserManager<AppUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<List<AppUser>> GetUsersByRoleAsync(string role)
    {
        return (await _userManager.GetUsersInRoleAsync(role)).ToList();
    }

    public async Task<PagedResult<AppUser>> GetPagedUsersAsync(UserFilterParameters filter)
    {
        var query = _context.Users.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.UserName))
        {
            query = query.Where(u => u.UserName!.Contains(filter.UserName));
        }

        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            query = query.Where(u => u.Email!.Contains(filter.Email));
        }

        if (!string.IsNullOrWhiteSpace(filter.FirstName))
        {
            query = query.Where(u => u.FirstName.Contains(filter.FirstName));
        }

        if (!string.IsNullOrWhiteSpace(filter.LastName))
        {
            query = query.Where(u => u.LastName.Contains(filter.LastName));
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(u =>
                u.UserName!.Contains(filter.SearchTerm) ||
                u.Email!.Contains(filter.SearchTerm) ||
                u.FirstName.Contains(filter.SearchTerm) ||
                u.LastName.Contains(filter.SearchTerm));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.UserName)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<AppUser>(items, totalCount, filter.PageNumber, filter.PageSize);
    }
}
