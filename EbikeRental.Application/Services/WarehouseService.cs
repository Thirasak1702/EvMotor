using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IRepository<Warehouse> _warehouseRepository;

    public WarehouseService(IRepository<Warehouse> warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<Result<List<WarehouseDto>>> GetAllAsync()
    {
        var warehouses = await _warehouseRepository.GetAllAsync();
        var warehouseDtos = warehouses.Select(w => new WarehouseDto
        {
            Id = w.Id,
            Code = w.Code,
            Name = w.Name,
            Location = w.Location,
            ContactPerson = w.ContactPerson,
            PhoneNumber = w.PhoneNumber,
            IsActive = w.IsActive
        }).ToList();

        return Result<List<WarehouseDto>>.Ok(warehouseDtos);
    }

    public async Task<Result<WarehouseDto>> GetByIdAsync(int id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null)
            return Result<WarehouseDto>.Fail("Warehouse not found");

        var warehouseDto = new WarehouseDto
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            Location = warehouse.Location,
            ContactPerson = warehouse.ContactPerson,
            PhoneNumber = warehouse.PhoneNumber,
            IsActive = warehouse.IsActive
        };

        return Result<WarehouseDto>.Ok(warehouseDto);
    }

    public async Task<Result<int>> CreateAsync(WarehouseDto warehouseDto)
    {
        var warehouse = new Warehouse
        {
            Code = warehouseDto.Code,
            Name = warehouseDto.Name,
            Location = warehouseDto.Location,
            ContactPerson = warehouseDto.ContactPerson,
            PhoneNumber = warehouseDto.PhoneNumber,
            IsActive = warehouseDto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _warehouseRepository.AddAsync(warehouse);
        return Result<int>.Ok(warehouse.Id);
    }

    public async Task<Result> UpdateAsync(WarehouseDto warehouseDto)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseDto.Id);
        if (warehouse == null)
            return Result.Fail("Warehouse not found");

        warehouse.Code = warehouseDto.Code;
        warehouse.Name = warehouseDto.Name;
        warehouse.Location = warehouseDto.Location;
        warehouse.ContactPerson = warehouseDto.ContactPerson;
        warehouse.PhoneNumber = warehouseDto.PhoneNumber;
        warehouse.IsActive = warehouseDto.IsActive;
        warehouse.UpdatedAt = DateTime.UtcNow;

        await _warehouseRepository.UpdateAsync(warehouse);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null)
            return Result.Fail("Warehouse not found");

        await _warehouseRepository.DeleteAsync(warehouse);
        return Result.Ok();
    }

    public async Task<Result<PagedResult<WarehouseDto>>> GetPagedAsync(WarehouseFilterParameters filter)
    {
        var pagedWarehouses = await _warehouseRepository.GetPagedAsync(
            filter.PageNumber,
            filter.PageSize,
            w => (string.IsNullOrWhiteSpace(filter.Code) || w.Code.Contains(filter.Code)) &&
                 (string.IsNullOrWhiteSpace(filter.Name) || w.Name.Contains(filter.Name)) &&
                 (string.IsNullOrWhiteSpace(filter.Location) || w.Location!.Contains(filter.Location)) &&
                 (string.IsNullOrWhiteSpace(filter.SearchTerm) || 
                  w.Code.Contains(filter.SearchTerm) || 
                  w.Name.Contains(filter.SearchTerm) || 
                  (w.Location != null && w.Location.Contains(filter.SearchTerm))),
            q => q.OrderBy(w => w.Code));
        
        var dtos = pagedWarehouses.Items.Select(w => new WarehouseDto
        {
            Id = w.Id,
            Code = w.Code,
            Name = w.Name,
            Location = w.Location,
            ContactPerson = w.ContactPerson,
            PhoneNumber = w.PhoneNumber,
            IsActive = w.IsActive
        }).ToList();

        var result = new PagedResult<WarehouseDto>(dtos, pagedWarehouses.TotalCount, pagedWarehouses.PageNumber, pagedWarehouses.PageSize);
        return Result<PagedResult<WarehouseDto>>.Ok(result);
    }
}

