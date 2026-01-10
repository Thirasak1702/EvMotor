using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces;

public interface IItemService
{
    Task<Result<List<ItemDto>>> GetAllAsync();
    Task<Result<PagedResult<ItemDto>>> GetPagedAsync(ItemFilterParameters filter);
    Task<Result<ItemDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(ItemDto itemDto);
    Task<Result> UpdateAsync(ItemDto itemDto);
    Task<Result> DeleteAsync(int id);
}
