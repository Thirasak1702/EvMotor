using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class ProductionReceiptService : IProductionReceiptService
{
    private readonly IProductionReceiptRepository _prRepository;
    private readonly IRepository<Item> _itemRepository;
    private readonly IRepository<Warehouse> _warehouseRepository;
    private readonly IRepository<ProductionOrder> _poRepository;
    private readonly IInventoryService _inventoryService;

    public ProductionReceiptService(
        IProductionReceiptRepository prRepository,
        IRepository<Item> itemRepository,
        IRepository<Warehouse> warehouseRepository,
        IRepository<ProductionOrder> poRepository,
        IInventoryService inventoryService)
    {
        _prRepository = prRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _poRepository = poRepository;
        _inventoryService = inventoryService;
    }

    public async Task<Result<List<ProductionReceiptDto>>> GetAllAsync()
    {
        try
        {
            var prs = await _prRepository.GetAllWithItemsAsync();
            var dtos = prs.Select(MapToDto).ToList();
            return Result<List<ProductionReceiptDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<ProductionReceiptDto>>.Fail($"Error retrieving production receipts: {ex.Message}");
        }
    }

    public async Task<Result<ProductionReceiptDto>> GetByIdAsync(int id)
    {
        try
        {
            var pr = await _prRepository.GetByIdWithItemsAsync(id);
            if (pr == null)
                return Result<ProductionReceiptDto>.Fail("Production receipt not found");

            return Result<ProductionReceiptDto>.Ok(MapToDto(pr));
        }
        catch (Exception ex)
        {
            return Result<ProductionReceiptDto>.Fail($"Error retrieving production receipt: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(ProductionReceiptDto dto, int userId)
    {
        try
        {
            var documentNumber = await _prRepository.GenerateDocumentNumberAsync();

            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                return Result<int>.Fail("Warehouse not found");

            // Validate Production Order if provided
            if (dto.ProductionOrderId.HasValue)
            {
                var po = await _poRepository.GetByIdAsync(dto.ProductionOrderId.Value);
                if (po == null)
                    return Result<int>.Fail("Production order not found");
                
                if (po.Status != "Completed")
                    return Result<int>.Fail("Can only create receipt for completed production orders");
            }

            var pr = new ProductionReceipt
            {
                DocumentNumber = documentNumber,
                ReceiptDate = dto.ReceiptDate,
                ProductionOrderId = dto.ProductionOrderId,
                ReceivedBy = dto.ReceivedBy,
                WarehouseId = dto.WarehouseId,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var itemDto in dto.Items)
            {
                var item = await _itemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null)
                    return Result<int>.Fail($"Item with ID {itemDto.ItemId} not found");

                pr.Items.Add(new ProductionReceiptItem
                {
                    ItemId = itemDto.ItemId,
                    PlannedQuantity = itemDto.PlannedQuantity,
                    ReceivedQuantity = itemDto.ReceivedQuantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    SerialNumber = itemDto.SerialNumber,
                    Notes = itemDto.Notes
                });
            }

            await _prRepository.AddAsync(pr);
            return Result<int>.Ok(pr.Id, "Production receipt created successfully");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating production receipt: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(int id, ProductionReceiptDto dto, int userId)
    {
        try
        {
            var pr = await _prRepository.GetByIdWithItemsAsync(id);
            if (pr == null)
                return Result.Fail("Production receipt not found");

            if (pr.Status != "Draft")
                return Result.Fail("Only draft production receipts can be updated");

            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                return Result.Fail("Warehouse not found");

            // Validate Production Order if provided
            if (dto.ProductionOrderId.HasValue)
            {
                var po = await _poRepository.GetByIdAsync(dto.ProductionOrderId.Value);
                if (po == null)
                    return Result.Fail("Production order not found");
            }

            pr.ReceiptDate = dto.ReceiptDate;
            pr.ProductionOrderId = dto.ProductionOrderId;
            pr.ReceivedBy = dto.ReceivedBy;
            pr.WarehouseId = dto.WarehouseId;
            pr.Notes = dto.Notes;
            pr.Status = dto.Status;
            pr.UpdatedAt = DateTime.UtcNow;

            pr.Items.Clear();
            foreach (var itemDto in dto.Items)
            {
                var item = await _itemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null)
                    return Result.Fail($"Item with ID {itemDto.ItemId} not found");

                pr.Items.Add(new ProductionReceiptItem
                {
                    ItemId = itemDto.ItemId,
                    PlannedQuantity = itemDto.PlannedQuantity,
                    ReceivedQuantity = itemDto.ReceivedQuantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    SerialNumber = itemDto.SerialNumber,
                    Notes = itemDto.Notes
                });
            }

            await _prRepository.UpdateAsync(pr);
            return Result.Ok("Production receipt updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating production receipt: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var pr = await _prRepository.GetByIdAsync(id);
            if (pr == null)
                return Result.Fail("Production receipt not found");

            if (pr.Status != "Draft")
                return Result.Fail("Only draft production receipts can be deleted");

            await _prRepository.DeleteAsync(pr);
            return Result.Ok("Production receipt deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting production receipt: {ex.Message}");
        }
    }

    public async Task<Result> PostAsync(int id, int userId)
    {
        try
        {
            Console.WriteLine($"???????????????????????????????????????");
            Console.WriteLine($"[PR POST] Called for Production Receipt ID: {id}, UserId: {userId}");
            Console.WriteLine($"???????????????????????????????????????");
            
            var pr = await _prRepository.GetByIdWithItemsAsync(id);
            if (pr == null)
            {
                Console.WriteLine($"[ERROR] Production Receipt not found: {id}");
                return Result.Fail("Production receipt not found");
            }

            Console.WriteLine($"[INFO] PR found: {pr.DocumentNumber}");
            Console.WriteLine($"       Status: {pr.Status}");
            Console.WriteLine($"       Warehouse: {pr.WarehouseId}");
            Console.WriteLine($"       Production Order: {pr.ProductionOrderId}");
            Console.WriteLine($"       Items Count: {pr.Items.Count}");

            if (pr.Status != "Draft")
            {
                Console.WriteLine($"[ERROR] PR status is not Draft: {pr.Status}");
                return Result.Fail("Only draft production receipts can be posted");
            }

            if (!pr.Items.Any())
            {
                Console.WriteLine($"[ERROR] PR has no items");
                return Result.Fail("Cannot post production receipt with no items");
            }

            var errors = new List<string>();
            var processedCount = 0;

            foreach (var prItem in pr.Items)
            {
                Console.WriteLine($"\n[ITEM {++processedCount}] Processing:");
                Console.WriteLine($"   ItemId: {prItem.ItemId}");
                Console.WriteLine($"   Planned Qty: {prItem.PlannedQuantity}");
                Console.WriteLine($"   Received Qty: {prItem.ReceivedQuantity}");
                
                if (prItem.ReceivedQuantity <= 0)
                {
                    Console.WriteLine($"   ? SKIPPED (Zero quantity)");
                    continue;
                }

                var item = await _itemRepository.GetByIdAsync(prItem.ItemId);
                if (item == null)
                {
                    Console.WriteLine($"   ? ERROR: Item not found!");
                    errors.Add($"Item ID {prItem.ItemId} not found");
                    continue;
                }

                Console.WriteLine($"   Item Code: {item.Code}");
                Console.WriteLine($"   Item Name: {item.Name}");
                Console.WriteLine($"   Standard Cost: {item.StandardCost}");

                var unitCost = item.StandardCost;

                var addStockRequest = new AddStockRequest
                {
                    ItemId = prItem.ItemId,
                    WarehouseId = pr.WarehouseId,
                    Quantity = prItem.ReceivedQuantity,
                    UnitOfMeasure = prItem.UnitOfMeasure,
                    UnitCost = unitCost,
                    TransactionType = "ProductionReceipt",
                    ReferenceType = "PR",
                    ReferenceId = pr.Id,
                    ReferenceNumber = pr.DocumentNumber,
                    BatchNumber = prItem.BatchNumber,
                    ExpiryDate = prItem.ExpiryDate,
                    Notes = $"Production Receipt: {pr.DocumentNumber}" + 
                            (pr.ProductionOrderId.HasValue ? $" - PO: {pr.ProductionOrder?.OrderNumber}" : "")
                };

                Console.WriteLine($"   ? Calling InventoryService.AddStockAsync...");

                var addResult = await _inventoryService.AddStockAsync(addStockRequest, userId);

                if (addResult.Success)
                {
                    Console.WriteLine($"   ? SUCCESS: {addResult.Message}");
                }
                else
                {
                    Console.WriteLine($"   ? FAILED: {addResult.Message}");
                    errors.Add($"Item {item.Code}: {addResult.Message}");
                }
            }

            Console.WriteLine($"\n???????????????????????????????????????");
            Console.WriteLine($"[SUMMARY] Processed: {processedCount} items");
            Console.WriteLine($"          Errors: {errors.Count}");
            Console.WriteLine($"???????????????????????????????????????");

            if (errors.Any())
            {
                Console.WriteLine($"[RESULT] ? POST FAILED");
                Console.WriteLine($"Errors:\n{string.Join("\n", errors)}");
                return Result.Fail($"Failed to post production receipt:\n{string.Join("\n", errors)}");
            }

            pr.Status = "Posted";
            pr.UpdatedAt = DateTime.UtcNow;
            pr.Notes = $"{pr.Notes}\n[Posted on {DateTime.UtcNow:yyyy-MM-dd HH:mm} by User {userId}]";

            Console.WriteLine($"[INFO] Updating PR status to Posted...");
            await _prRepository.UpdateAsync(pr);

            Console.WriteLine($"[RESULT] ? POST SUCCESSFUL");
            Console.WriteLine($"???????????????????????????????????????\n");

            return Result.Ok($"Production receipt posted successfully. {pr.Items.Count(i => i.ReceivedQuantity > 0)} finished goods added to inventory.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n? EXCEPTION IN PostAsync ?");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
            Console.WriteLine($"???????????????????????????????????????\n");
            return Result.Fail($"Error posting production receipt: {ex.Message}");
        }
    }

    public async Task<Result> CancelAsync(int id, int userId, string reason)
    {
        try
        {
            var pr = await _prRepository.GetByIdAsync(id);
            if (pr == null)
                return Result.Fail("Production receipt not found");

            if (pr.Status == "Cancelled")
                return Result.Fail("Production receipt is already cancelled");

            pr.Status = "Cancelled";
            pr.Notes = $"{pr.Notes}\nCancellation reason: {reason}";
            pr.UpdatedAt = DateTime.UtcNow;

            await _prRepository.UpdateAsync(pr);
            return Result.Ok("Production receipt cancelled");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error cancelling production receipt: {ex.Message}");
        }
    }

    public async Task<Result<List<ProductionReceiptDto>>> GetByProductionOrderIdAsync(int productionOrderId)
    {
        try
        {
            var prs = await _prRepository.GetByProductionOrderIdAsync(productionOrderId);
            var dtos = prs.Select(MapToDto).ToList();
            return Result<List<ProductionReceiptDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<ProductionReceiptDto>>.Fail($"Error retrieving production receipts: {ex.Message}");
        }
    }

    private ProductionReceiptDto MapToDto(ProductionReceipt pr)
    {
        return new ProductionReceiptDto
        {
            Id = pr.Id,
            DocumentNumber = pr.DocumentNumber,
            ReceiptDate = pr.ReceiptDate,
            ProductionOrderId = pr.ProductionOrderId,
            ProductionOrderNumber = pr.ProductionOrder?.OrderNumber ?? "",
            ReceivedBy = pr.ReceivedBy,
            WarehouseId = pr.WarehouseId,
            WarehouseName = pr.Warehouse?.Name ?? "",
            Status = pr.Status,
            Notes = pr.Notes,
            CreatedByUserId = pr.CreatedByUserId,
            CreatedAt = pr.CreatedAt,
            Items = pr.Items.Select(i => new ProductionReceiptItemDto
            {
                Id = i.Id,
                ProductionReceiptId = i.ProductionReceiptId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.Code ?? "",
                ItemName = i.Item?.Name ?? "",
                PlannedQuantity = i.PlannedQuantity,
                ReceivedQuantity = i.ReceivedQuantity,
                UnitOfMeasure = i.UnitOfMeasure,
                BatchNumber = i.BatchNumber,
                ExpiryDate = i.ExpiryDate,
                SerialNumber = i.SerialNumber,
                Notes = i.Notes
            }).ToList()
        };
    }
}
