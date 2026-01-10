using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Services;

public class ItemService : IItemService
{
    private readonly IRepository<Item> _itemRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public ItemService(IRepository<Item> itemRepository, IInventoryRepository inventoryRepository)
    {
        _itemRepository = itemRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<List<ItemDto>>> GetAllAsync()
    {
        var items = await _itemRepository.GetAllAsync();
        var itemDtos = items.Select(i => new ItemDto
        {
            Id = i.Id,
            Code = i.Code,
            Name = i.Name,
            Description = i.Description,
            Category = i.Category,
            UnitOfMeasure = i.UnitOfMeasure,
            StandardCost = i.StandardCost,
            IsActive = i.IsActive,
            IsBarcode = i.IsBarcode,
            IsSerial = i.IsSerial,
            IsBatch = i.IsBatch,
            IsExpiry = i.IsExpiry,
            Barcode = i.Barcode,
            TrackingMethod = i.TrackingMethod
        }).ToList();

        return Result<List<ItemDto>>.Ok(itemDtos);
    }

    public async Task<Result<ItemDto>> GetByIdAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
            return Result<ItemDto>.Fail("Item not found");

        var itemDto = new ItemDto
        {
            Id = item.Id,
            Code = item.Code,
            Name = item.Name,
            Description = item.Description,
            Category = item.Category,
            UnitOfMeasure = item.UnitOfMeasure,
            StandardCost = item.StandardCost,
            IsActive = item.IsActive,
            IsBarcode = item.IsBarcode,
            IsSerial = item.IsSerial,
            IsBatch = item.IsBatch,
            IsExpiry = item.IsExpiry,
            Barcode = item.Barcode,
            TrackingMethod = item.TrackingMethod
        };

        return Result<ItemDto>.Ok(itemDto);
    }

    public async Task<Result<int>> CreateAsync(ItemDto itemDto)
    {
        var item = new Item
        {
            Code = itemDto.Code,
            Name = itemDto.Name,
            Description = itemDto.Description,
            Category = itemDto.Category,
            UnitOfMeasure = itemDto.UnitOfMeasure,
            StandardCost = itemDto.StandardCost,
            IsActive = itemDto.IsActive,
            IsBarcode = itemDto.IsBarcode,
            IsSerial = itemDto.IsSerial,
            IsBatch = itemDto.IsBatch,
            IsExpiry = itemDto.IsExpiry,
            Barcode = itemDto.Barcode,
            TrackingMethod = itemDto.TrackingMethod,
            CreatedAt = DateTime.UtcNow
        };

        await _itemRepository.AddAsync(item);
        return Result<int>.Ok(item.Id);
    }

    public async Task<Result> UpdateAsync(ItemDto itemDto)
    {
        var item = await _itemRepository.GetByIdAsync(itemDto.Id);
        if (item == null)
            return Result.Fail("Item not found");

        item.Code = itemDto.Code;
        item.Name = itemDto.Name;
        item.Description = itemDto.Description;
        item.Category = itemDto.Category;
        item.UnitOfMeasure = itemDto.UnitOfMeasure;
        item.StandardCost = itemDto.StandardCost;
        item.IsActive = itemDto.IsActive;
        item.IsBarcode = itemDto.IsBarcode;
        item.IsSerial = itemDto.IsSerial;
        item.IsBatch = itemDto.IsBatch;
        item.IsExpiry = itemDto.IsExpiry;
        item.Barcode = itemDto.Barcode;
        item.TrackingMethod = itemDto.TrackingMethod;
        item.UpdatedAt = DateTime.UtcNow;

        await _itemRepository.UpdateAsync(item);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
            return Result.Fail("Item not found");

        await _itemRepository.DeleteAsync(item);
        return Result.Ok();
    }

    public async Task<Result<PagedResult<ItemDto>>> GetPagedAsync(ItemFilterParameters filter)
    {
        var pagedItems = await _inventoryRepository.GetPagedItemsAsync(filter);
        
        var dtos = pagedItems.Items.Select(i => new ItemDto
        {
            Id = i.Id,
            Code = i.Code,
            Name = i.Name,
            Description = i.Description,
            Category = i.Category,
            UnitOfMeasure = i.UnitOfMeasure,
            StandardCost = i.StandardCost,
            IsActive = i.IsActive,
            IsBarcode = i.IsBarcode,
            IsSerial = i.IsSerial,
            IsBatch = i.IsBatch,
            IsExpiry = i.IsExpiry,
            Barcode = i.Barcode,
            TrackingMethod = i.TrackingMethod
        }).ToList();

        var result = new PagedResult<ItemDto>(dtos, pagedItems.TotalCount, pagedItems.PageNumber, pagedItems.PageSize);
        return Result<PagedResult<ItemDto>>.Ok(result);
    }
}

