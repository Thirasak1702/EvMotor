using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class MaterialIssueService : IMaterialIssueService
{
    private readonly IMaterialIssueRepository _miRepository;
    private readonly IRepository<Item> _itemRepository;
    private readonly IRepository<Warehouse> _warehouseRepository;
    private readonly IRepository<ProductionOrder> _poRepository;
    private readonly IRepository<ProductionOrderItem> _poItemRepository;
    private readonly IBomService _bomService;
    private readonly IInventoryService _inventoryService;

    public MaterialIssueService(
        IMaterialIssueRepository miRepository,
        IRepository<Item> itemRepository,
        IRepository<Warehouse> warehouseRepository,
        IRepository<ProductionOrder> poRepository,
        IRepository<ProductionOrderItem> poItemRepository,
        IBomService bomService,
        IInventoryService inventoryService)
    {
        _miRepository = miRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _poRepository = poRepository;
        _poItemRepository = poItemRepository;
        _bomService = bomService;
        _inventoryService = inventoryService;
    }

    public async Task<Result<List<MaterialIssueDto>>> GetAllAsync()
    {
        try
        {
            var mis = await _miRepository.GetAllWithDetailsAsync();
            var dtos = mis.Select(MapToDto).ToList();
            return Result<List<MaterialIssueDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<MaterialIssueDto>>.Fail($"Error retrieving material issues: {ex.Message}");
        }
    }

    public async Task<Result<MaterialIssueDto>> GetByIdAsync(int id)
    {
        try
        {
            var mi = await _miRepository.GetByIdWithDetailsAsync(id);
            if (mi == null)
                return Result<MaterialIssueDto>.Fail("Material issue not found");

            return Result<MaterialIssueDto>.Ok(MapToDto(mi));
        }
        catch (Exception ex)
        {
            return Result<MaterialIssueDto>.Fail($"Error retrieving material issue: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(MaterialIssueDto dto, int userId)
    {
        try
        {
            var documentNumber = await _miRepository.GenerateDocumentNumberAsync();

            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                return Result<int>.Fail("Warehouse not found");

            var mi = new MaterialIssue
            {
                DocumentNumber = documentNumber,
                IssueDate = dto.IssueDate,
                ProductionOrderId = dto.ProductionOrderId,
                WarehouseId = dto.WarehouseId,
                IssuedBy = dto.IssuedBy,
                Status = "Draft",
                Notes = dto.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var itemDto in dto.Items)
            {
                var item = await _itemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null)
                    return Result<int>.Fail($"Item with ID {itemDto.ItemId} not found");

                mi.Items.Add(new MaterialIssueItem
                {
                    ItemId = itemDto.ItemId,
                    RequiredQuantity = itemDto.RequiredQuantity,
                    IssuedQuantity = itemDto.IssuedQuantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    SerialNumber = itemDto.SerialNumber,
                    Notes = itemDto.Notes
                });
            }

            await _miRepository.AddAsync(mi);
            return Result<int>.Ok(mi.Id, "Material issue created successfully");
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message;
            var innerMsg = string.IsNullOrWhiteSpace(inner) ? string.Empty : $" | Inner: {inner}";
            return Result<int>.Fail($"Error creating material issue: {ex.Message}{innerMsg}");
        }
    }

    public async Task<Result> UpdateAsync(int id, MaterialIssueDto dto, int userId)
    {
        try
        {
            var mi = await _miRepository.GetByIdWithDetailsAsync(id);
            if (mi == null)
                return Result.Fail("Material issue not found");

            if (mi.Status == "Posted")
                return Result.Fail("Cannot update posted material issue");

            mi.IssueDate = dto.IssueDate;
            mi.IssuedBy = dto.IssuedBy;
            mi.Notes = dto.Notes;
            mi.UpdatedAt = DateTime.UtcNow;

            // Update items
            mi.Items.Clear();
            foreach (var itemDto in dto.Items)
            {
                mi.Items.Add(new MaterialIssueItem
                {
                    ItemId = itemDto.ItemId,
                    RequiredQuantity = itemDto.RequiredQuantity,
                    IssuedQuantity = itemDto.IssuedQuantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    SerialNumber = itemDto.SerialNumber,
                    Notes = itemDto.Notes
                });
            }

            await _miRepository.UpdateAsync(mi);
            return Result.Ok("Material issue updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating material issue: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var mi = await _miRepository.GetByIdAsync(id);
            if (mi == null)
                return Result.Fail("Material issue not found");

            if (mi.Status == "Posted")
                return Result.Fail("Cannot delete posted material issue");

            await _miRepository.DeleteAsync(mi);

            return Result.Ok("Material issue deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting material issue: {ex.Message}");
        }
    }

    public async Task<Result> PostAsync(int id, int userId)
    {
        try
        {
            var mi = await _miRepository.GetByIdWithDetailsAsync(id);
            if (mi == null)
                return Result.Fail("Material issue not found");

            if (mi.Status == "Posted")
                return Result.Fail("Material issue is already posted");

            // Validate stock availability for all items before posting
            foreach (var item in mi.Items)
            {
                var validationResult = await _inventoryService.ValidateStockAvailabilityAsync(
                    item.ItemId,
                    mi.WarehouseId,
                    item.IssuedQuantity,
                    item.BatchNumber);

                if (!validationResult.Success)
                {
                    return Result.Fail($"Cannot post: {validationResult.Message}");
                }
            }

            // Deduct stock for each item
            foreach (var item in mi.Items)
            {
                var deductRequest = new DeductStockRequest
                {
                    ItemId = item.ItemId,
                    WarehouseId = mi.WarehouseId,
                    Quantity = item.IssuedQuantity,
                    UnitOfMeasure = item.UnitOfMeasure,
                    TransactionType = "MaterialIssue",
                    ReferenceType = "MI",
                    ReferenceId = mi.Id,
                    ReferenceNumber = mi.DocumentNumber,
                    BatchNumber = item.BatchNumber,
                    SerialNumber = item.SerialNumber,
                    Notes = $"Material Issue: {mi.DocumentNumber}{(!string.IsNullOrWhiteSpace(item.Notes) ? $" - {item.Notes}" : "")}"
                };

                var deductResult = await _inventoryService.DeductStockAsync(deductRequest, userId);
                
                if (!deductResult.Success)
                {
                    return Result.Fail($"Failed to deduct stock for item {item.Item?.Code}: {deductResult.Message}");
                }
            }

            mi.Status = "Posted";
            mi.UpdatedAt = DateTime.UtcNow;

            await _miRepository.UpdateAsync(mi);

            return Result.Ok("Material issue posted successfully. Stock deducted from inventory.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error posting material issue: {ex.Message}");
        }
    }

    public async Task<Result<MaterialIssueDto>> GenerateFromProductionOrderAsync(int productionOrderId)
    {
        try
        {
            var po = await _poRepository.GetByIdAsync(productionOrderId);
            if (po == null)
                return Result<MaterialIssueDto>.Fail("Production order not found");

            // Load ProductionOrder Items
            var orderItems = await _poItemRepository.FindAsync(oi => oi.ProductionOrderId == productionOrderId);

            if (orderItems == null || !orderItems.Any())
                return Result<MaterialIssueDto>.Fail("Production order has no items. Please ensure BOM is loaded.");

            var dto = new MaterialIssueDto
            {
                IssueDate = DateTime.Today,
                ProductionOrderId = productionOrderId,
                ProductionOrderNumber = po.OrderNumber,
                IssuedBy = string.Empty,
                Status = "Draft",
                Items = new List<MaterialIssueItemDto>()
            };

            // Create Material Issue Items from Production Order Items
            foreach (var orderItem in orderItems.OrderBy(oi => oi.Sequence))
            {
                var item = await _itemRepository.GetByIdAsync(orderItem.ItemId);
                if (item == null)
                {
                    // Skip items that are not found
                    continue;
                }

                dto.Items.Add(new MaterialIssueItemDto
                {
                    ProductionOrderItemId = orderItem.Id,
                    ItemId = orderItem.ItemId,
                    ItemCode = item.Code,
                    ItemName = item.Name,
                    RequiredQuantity = orderItem.Quantity,
                    IssuedQuantity = orderItem.Quantity, // Default issued quantity to required quantity
                    UnitOfMeasure = orderItem.UnitOfMeasure,
                    Notes = orderItem.Notes
                });
            }

            if (dto.Items.Count == 0)
                return Result<MaterialIssueDto>.Fail("No valid items found in production order");

            return Result<MaterialIssueDto>.Ok(dto, "Materials generated successfully");
        }
        catch (Exception ex)
        {
            return Result<MaterialIssueDto>.Fail($"Error generating material issue: {ex.Message}");
        }
    }

    public async Task<Result<List<MaterialIssueDto>>> GetByProductionOrderIdAsync(int poId)
    {
        try
        {
            var mis = await _miRepository.GetByProductionOrderIdAsync(poId);
            var dtos = mis.Select(MapToDto).ToList();
            return Result<List<MaterialIssueDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<MaterialIssueDto>>.Fail($"Error retrieving material issues: {ex.Message}");
        }
    }

    private MaterialIssueDto MapToDto(MaterialIssue mi)
    {
        return new MaterialIssueDto
        {
            Id = mi.Id,
            DocumentNumber = mi.DocumentNumber,
            IssueDate = mi.IssueDate,
            ProductionOrderId = mi.ProductionOrderId,
            ProductionOrderNumber = mi.ProductionOrder?.OrderNumber,
            WarehouseId = mi.WarehouseId,
            WarehouseName = mi.Warehouse?.Name,
            IssuedBy = mi.IssuedBy,
            Status = mi.Status,
            Notes = mi.Notes,
            CreatedAt = mi.CreatedAt,
            Items = mi.Items.Select(i => new MaterialIssueItemDto
            {
                Id = i.Id,
                MaterialIssueId = i.MaterialIssueId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.Code,
                ProductionOrderItemId = i.ProductionOrderItemId,
                ItemName = i.Item?.Name,
                RequiredQuantity = i.RequiredQuantity,
                IssuedQuantity = i.IssuedQuantity,
                UnitOfMeasure = i.UnitOfMeasure,
                BatchNumber = i.BatchNumber,
                ExpiryDate = i.ExpiryDate,
                SerialNumber = i.SerialNumber,
                Notes = i.Notes
            }).ToList()
        };
    }
}
