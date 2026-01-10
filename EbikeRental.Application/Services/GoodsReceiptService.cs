using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly IGoodsReceiptRepository _grRepository;
    private readonly IRepository<Item> _itemRepository;
    private readonly IRepository<Warehouse> _warehouseRepository;
    private readonly IPurchaseOrderRepository _poRepository;
    private readonly IInventoryService _inventoryService;

    public GoodsReceiptService(
        IGoodsReceiptRepository grRepository,
        IRepository<Item> itemRepository,
        IRepository<Warehouse> warehouseRepository,
        IPurchaseOrderRepository poRepository,
        IInventoryService inventoryService)
    {
        _grRepository = grRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _poRepository = poRepository;
        _inventoryService = inventoryService;
    }

    public async Task<Result<List<GoodsReceiptDto>>> GetAllAsync()
    {
        try
        {
            var grs = await _grRepository.GetAllWithItemsAsync();
            var dtos = grs.Select(MapToDto).ToList();
            return Result<List<GoodsReceiptDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<GoodsReceiptDto>>.Fail($"Error retrieving goods receipts: {ex.Message}");
        }
    }

    public async Task<Result<GoodsReceiptDto>> GetByIdAsync(int id)
    {
        try
        {
            var gr = await _grRepository.GetByIdWithItemsAsync(id);
            if (gr == null)
                return Result<GoodsReceiptDto>.Fail("Goods receipt not found");

            return Result<GoodsReceiptDto>.Ok(MapToDto(gr));
        }
        catch (Exception ex)
        {
            return Result<GoodsReceiptDto>.Fail($"Error retrieving goods receipt: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(GoodsReceiptDto dto, int userId)
    {
        try
        {
            var documentNumber = await _grRepository.GenerateDocumentNumberAsync();

            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                return Result<int>.Fail("Warehouse not found");

            var gr = new GoodsReceipt
            {
                DocumentNumber = documentNumber,
                ReceiptDate = dto.ReceiptDate,
                PurchaseOrderId = dto.PurchaseOrderId,
                VendorName = dto.VendorName,
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

                gr.Items.Add(new GoodsReceiptItem
                {
                    ItemId = itemDto.ItemId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = itemDto.ReceivedQuantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    Notes = itemDto.Notes,
                    IsAccepted = itemDto.IsAccepted,
                    Barcode = itemDto.Barcode,
                    SerialNumber = itemDto.SerialNumber
                });
            }

            await _grRepository.AddAsync(gr);
            return Result<int>.Ok(gr.Id, "Goods receipt created successfully");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating goods receipt: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(int id, GoodsReceiptDto dto, int userId)
    {
        try
        {
            var gr = await _grRepository.GetByIdWithItemsAsync(id);
            if (gr == null)
                return Result.Fail("Goods receipt not found");

            if (gr.Status != "Draft")
                return Result.Fail("Only draft goods receipts can be updated");

            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                return Result.Fail("Warehouse not found");

            gr.ReceiptDate = dto.ReceiptDate;
            gr.PurchaseOrderId = dto.PurchaseOrderId;
            gr.VendorName = dto.VendorName;
            gr.ReceivedBy = dto.ReceivedBy;
            gr.WarehouseId = dto.WarehouseId;
            gr.Notes = dto.Notes;
            gr.Status = dto.Status;
            gr.UpdatedAt = DateTime.UtcNow;

            gr.Items.Clear();
            foreach (var itemDto in dto.Items)
            {
                var item = await _itemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null)
                    return Result.Fail($"Item with ID {itemDto.ItemId} not found");

                gr.Items.Add(new GoodsReceiptItem
                {
                    ItemId = itemDto.ItemId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = itemDto.ReceivedQuantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    Notes = itemDto.Notes,
                    IsAccepted = itemDto.IsAccepted,
                    Barcode = itemDto.Barcode,
                    SerialNumber = itemDto.SerialNumber
                });
            }

            await _grRepository.UpdateAsync(gr);
            return Result.Ok("Goods receipt updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating goods receipt: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var gr = await _grRepository.GetByIdAsync(id);
            if (gr == null)
                return Result.Fail("Goods receipt not found");

            if (gr.Status != "Draft")
                return Result.Fail("Only draft goods receipts can be deleted");

            await _grRepository.DeleteAsync(gr);
            return Result.Ok("Goods receipt deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting goods receipt: {ex.Message}");
        }
    }

    public async Task<Result> PostAsync(int id, int userId)
    {
        try
        {
            // ?? DEBUG: Log entry point
            Console.WriteLine($"???????????????????????????????????????????");
            Console.WriteLine($"[GR POST] Called for GR ID: {id}, UserId: {userId}");
            Console.WriteLine($"???????????????????????????????????????????");
            
            var gr = await _grRepository.GetByIdWithItemsAsync(id);
            if (gr == null)
            {
                Console.WriteLine($"[ERROR] GR not found: {id}");
                return Result.Fail("Goods receipt not found");
            }

            Console.WriteLine($"[INFO] GR found: {gr.DocumentNumber}");
            Console.WriteLine($"       Status: {gr.Status}");
            Console.WriteLine($"       Warehouse: {gr.WarehouseId}");
            Console.WriteLine($"       Items Count: {gr.Items.Count}");

            if (gr.Status != "Draft")
            {
                Console.WriteLine($"[ERROR] GR status is not Draft: {gr.Status}");
                return Result.Fail("Only draft goods receipts can be posted");
            }

            if (!gr.Items.Any())
            {
                Console.WriteLine($"[ERROR] GR has no items");
                return Result.Fail("Cannot post goods receipt with no items");
            }

            var errors = new List<string>();
            var processedCount = 0;

            foreach (var grItem in gr.Items)
            {
                Console.WriteLine($"\n[ITEM {++processedCount}] Processing:");
                Console.WriteLine($"   ItemId: {grItem.ItemId}");
                Console.WriteLine($"   Quantity: {grItem.ReceivedQuantity}");
                Console.WriteLine($"   IsAccepted: {grItem.IsAccepted}");
                
                if (!grItem.IsAccepted)
                {
                    Console.WriteLine($"   ? SKIPPED (Not accepted)");
                    continue;
                }

                var item = await _itemRepository.GetByIdAsync(grItem.ItemId);
                if (item == null)
                {
                    Console.WriteLine($"   ? ERROR: Item not found!");
                    errors.Add($"Item ID {grItem.ItemId} not found");
                    continue;
                }

                Console.WriteLine($"   Item Code: {item.Code}");
                Console.WriteLine($"   Item Name: {item.Name}");
                Console.WriteLine($"   Standard Cost: {item.StandardCost}");

                var unitCost = item.StandardCost;

                var addStockRequest = new AddStockRequest
                {
                    ItemId = grItem.ItemId,
                    WarehouseId = gr.WarehouseId,
                    Quantity = grItem.ReceivedQuantity,
                    UnitOfMeasure = grItem.UnitOfMeasure,
                    UnitCost = unitCost,
                    TransactionType = "GoodsReceipt",
                    ReferenceType = "GR",
                    ReferenceId = gr.Id,
                    ReferenceNumber = gr.DocumentNumber,
                    BatchNumber = grItem.BatchNumber,
                    SerialNumber = grItem.SerialNumber,
                    ExpiryDate = grItem.ExpiryDate,
                    Notes = $"GR: {gr.DocumentNumber} - {gr.VendorName}"
                };

                Console.WriteLine($"   ?? Calling InventoryService.AddStockAsync...");

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

            Console.WriteLine($"\n???????????????????????????????????????????");
            Console.WriteLine($"[SUMMARY] Processed: {processedCount} items");
            Console.WriteLine($"          Errors: {errors.Count}");
            Console.WriteLine($"???????????????????????????????????????????");

            if (errors.Any())
            {
                Console.WriteLine($"[RESULT] ? POST FAILED");
                Console.WriteLine($"Errors:\n{string.Join("\n", errors)}");
                return Result.Fail($"Failed to post goods receipt:\n{string.Join("\n", errors)}");
            }

            gr.Status = "Posted";
            gr.UpdatedAt = DateTime.UtcNow;
            gr.Notes = $"{gr.Notes}\n[Posted on {DateTime.UtcNow:yyyy-MM-dd HH:mm} by User {userId}]";

            Console.WriteLine($"[INFO] Updating GR status to Posted...");
            await _grRepository.UpdateAsync(gr);

            Console.WriteLine($"[RESULT] ? POST SUCCESSFUL");
            Console.WriteLine($"???????????????????????????????????????????\n");

            return Result.Ok($"Goods receipt posted successfully. {gr.Items.Count(i => i.IsAccepted)} items added to inventory.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n??? EXCEPTION IN PostAsync ???");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
            Console.WriteLine($"???????????????????????????????????????????\n");
            return Result.Fail($"Error posting goods receipt: {ex.Message}");
        }
    }

    public async Task<Result> CancelAsync(int id, int userId, string reason)
    {
        try
        {
            var gr = await _grRepository.GetByIdAsync(id);
            if (gr == null)
                return Result.Fail("Goods receipt not found");

            if (gr.Status == "Cancelled")
                return Result.Fail("Goods receipt is already cancelled");

            gr.Status = "Cancelled";
            gr.Notes = $"{gr.Notes}\nCancellation reason: {reason}";
            gr.UpdatedAt = DateTime.UtcNow;

            await _grRepository.UpdateAsync(gr);
            return Result.Ok("Goods receipt cancelled");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error cancelling goods receipt: {ex.Message}");
        }
    }

    private GoodsReceiptDto MapToDto(GoodsReceipt gr)
    {
        return new GoodsReceiptDto
        {
            Id = gr.Id,
            DocumentNumber = gr.DocumentNumber,
            ReceiptDate = gr.ReceiptDate,
            PurchaseOrderId = gr.PurchaseOrderId,
            PurchaseOrderNumber = gr.PurchaseOrder?.DocumentNumber ?? "",
            VendorName = gr.VendorName,
            ReceivedBy = gr.ReceivedBy,
            WarehouseId = gr.WarehouseId,
            WarehouseName = gr.Warehouse?.Name ?? "",
            Status = gr.Status,
            Notes = gr.Notes,
            CreatedAt = gr.CreatedAt,
            Items = gr.Items.Select(i => new GoodsReceiptItemDto
            {
                Id = i.Id,
                GoodsReceiptId = i.GoodsReceiptId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.Code ?? "",
                ItemName = i.Item?.Name ?? "",
                OrderedQuantity = i.OrderedQuantity,
                ReceivedQuantity = i.ReceivedQuantity,
                UnitOfMeasure = i.UnitOfMeasure,
                BatchNumber = i.BatchNumber,
                ExpiryDate = i.ExpiryDate,
                Notes = i.Notes,
                IsAccepted = i.IsAccepted,
                Barcode = i.Barcode,
                SerialNumber = i.SerialNumber
            }).ToList()
        };
    }
}
