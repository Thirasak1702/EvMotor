using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces;

public interface IWarehouseService
{
    Task<Result<List<WarehouseDto>>> GetAllAsync();
    Task<Result<PagedResult<WarehouseDto>>> GetPagedAsync(WarehouseFilterParameters filter);
    Task<Result<WarehouseDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(WarehouseDto warehouseDto);
    Task<Result> UpdateAsync(WarehouseDto warehouseDto);
    Task<Result> DeleteAsync(int id);
}
