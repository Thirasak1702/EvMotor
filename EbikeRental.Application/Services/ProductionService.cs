using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class ProductionService : IProductionService
{
    private readonly IRepository<ProductionOrder> _poRepository;
    private readonly IRepository<ProductionOrderItem> _poItemRepository;
    private readonly IRepository<ProductionOrderProcesses> _poProcessRepository;
    private readonly IRepository<ProductionOrderQcs> _poQcRepository;
    private readonly IRepository<Item> _itemRepository;
    private readonly IBomService _bomService;
    private readonly IMaterialIssueService _materialIssueService;
    private readonly IGoodsReceiptService _grService;

    public ProductionService(
        IRepository<ProductionOrder> poRepository,
        IRepository<ProductionOrderItem> poItemRepository,
        IRepository<ProductionOrderProcesses> poProcessRepository,
        IRepository<ProductionOrderQcs> poQcRepository,
        IRepository<Item> itemRepository,
        IBomService bomService,
        IMaterialIssueService materialIssueService,
        IGoodsReceiptService grService)
    {
        _poRepository = poRepository;
        _poItemRepository = poItemRepository;
        _poProcessRepository = poProcessRepository;
        _poQcRepository = poQcRepository;
        _itemRepository = itemRepository;
        _bomService = bomService;
        _materialIssueService = materialIssueService;
        _grService = grService;
    }

    public async Task<Result<List<ProductionOrderDto>>> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _poRepository.GetAllAsync();
            var dtos = new List<ProductionOrderDto>();

            foreach (var order in orders)
            {
                dtos.Add(await MapToDtoAsync(order));
            }

            return Result<List<ProductionOrderDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<ProductionOrderDto>>.Fail($"Error retrieving production orders: {ex.Message}");
        }
    }

    public async Task<Result<ProductionOrderDto>> GetOrderByIdAsync(int id)
    {
        try
        {
            var order = await _poRepository.GetByIdAsync(id);
            if (order == null)
                return Result<ProductionOrderDto>.Fail("Production order not found");

            var dto = await MapToDtoAsync(order);
            return Result<ProductionOrderDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<ProductionOrderDto>.Fail($"Error retrieving production order: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateOrderAsync(ProductionOrderDto orderDto)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(orderDto.ItemId);
            if (item == null)
                return Result<int>.Fail("Item not found");

            // Load BOM - ใช้ BOM Code ถ้ามี หรือใช้ Parent Item Id
            BomDto? bom = null;
            if (!string.IsNullOrWhiteSpace(orderDto.BomCode))
            {
                var bomResult = await _bomService.GetByCodeAsync(orderDto.BomCode);
                if (bomResult.Success && bomResult.Data != null)
                {
                    bom = bomResult.Data;
                }
            }
            
            if (bom == null)
            {
                // ถ้าไม่มี BOM Code ให้หา BOM จาก Parent Item
                var bomResult = await _bomService.GetByParentItemIdAsync(orderDto.ItemId);
                if (!bomResult.Success || bomResult.Data == null || !bomResult.Data.Any())
                    return Result<int>.Fail("Cannot create production order: No BOM found for this item");
                
                // เลือก BOM ที่ Active
                bom = bomResult.Data.FirstOrDefault(b => b.IsActive) ?? bomResult.Data.First();
            }

            // Validate BOM Parent Item matches Order Item
            if (bom.ParentItemId != orderDto.ItemId)
                return Result<int>.Fail("BOM does not match the selected item");

            var order = new ProductionOrder
            {
                OrderNumber = string.IsNullOrWhiteSpace(orderDto.OrderNumber) || orderDto.OrderNumber.Equals("Auto-generated", StringComparison.OrdinalIgnoreCase) 
                    ? await GenerateOrderNumberAsync() 
                    : orderDto.OrderNumber,
                ItemId = orderDto.ItemId,
                Quantity = orderDto.Quantity,
                PlannedStartDate = orderDto.PlannedStartDate,
                PlannedEndDate = orderDto.PlannedEndDate,
                Status = "Draft",
                Notes = orderDto.Notes,
                CreatedByUserId = orderDto.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                // BOM Reference
                BillOfMaterialId = bom.Id,
                BomCode = bom.BomCode,
                BomName = $"{bom.BomCode} - {bom.Description}"
            };

            // Create Order Items from Order.Items (if provided) or from BOM Items
            if (orderDto.Items != null && orderDto.Items.Any())
            {
                // Use Items from form (user may have edited them)
                foreach (var itemDto in orderDto.Items.OrderBy(i => i.Sequence))
                {
                    order.Items.Add(new ProductionOrderItem
                    {
                        BomItemId = itemDto.BomItemId,
                        ItemId = itemDto.ItemId,
                        Sequence = itemDto.Sequence,
                        Quantity = itemDto.Quantity,
                        UnitOfMeasure = itemDto.UnitOfMeasure,
                        BomQuantity = itemDto.BomQuantity > 0 ? itemDto.BomQuantity : itemDto.Quantity,
                        Notes = itemDto.Notes,
                        CreatedByUserId = orderDto.CreatedByUserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            else
            {
                // Fallback: Create Order Items from BOM Items (if no Items provided)
                foreach (var bomItem in bom.BomItems.OrderBy(bi => bi.Sequence))
                {
                    var orderItemQuantity = orderDto.Quantity * bomItem.Quantity;
                    
                    order.Items.Add(new ProductionOrderItem
                    {
                        BomItemId = bomItem.Id,
                        ItemId = bomItem.ComponentItemId,
                        Sequence = bomItem.Sequence,
                        Quantity = orderItemQuantity,
                        UnitOfMeasure = bomItem.UnitOfMeasure,
                        BomQuantity = bomItem.Quantity,
                        Notes = bomItem.Notes,
                        CreatedByUserId = orderDto.CreatedByUserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Create Production Processes from BOM Processes
            foreach (var bomProcess in bom.BomProcesses.OrderBy(bp => bp.Sequence))
            {
                order.Processes.Add(new ProductionOrderProcesses
                {
                    BomProcessId = bomProcess.Id,
                    Sequence = bomProcess.Sequence,
                    WorkCode = bomProcess.WorkCode,
                    WorkName = bomProcess.WorkName,
                    NumberOfPersons = bomProcess.NumberOfPersons,
                    Quantity = bomProcess.Quantity,
                    UnitOfMeasure = bomProcess.UnitOfMeasure,
                    Status = "Pending",
                    Notes = bomProcess.Notes
                });
            }

            // Create QC Steps from BOM QCs
            foreach (var bomQc in bom.BomQcs.OrderBy(bq => bq.Sequence))
            {
                order.QcSteps.Add(new ProductionOrderQcs
                {
                    BomQcId = bomQc.Id,
                    Sequence = bomQc.Sequence,
                    QcCode = bomQc.QcCode,
                    QcName = bomQc.QcName,
                    QcValues = bomQc.QcValues,
                    Status = "Pending",
                    Notes = bomQc.Notes
                });
            }

            await _poRepository.AddAsync(order);

            return Result<int>.Ok(order.Id, "Production order created successfully");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating production order: {ex.Message}");
        }
    }

    public async Task<Result> UpdateOrderAsync(ProductionOrderDto orderDto)
    {
        try
        {
            var order = await _poRepository.GetByIdAsync(orderDto.Id);
            if (order == null)
                return Result.Fail("Production order not found");

            if (order.Status == "Completed" || order.Status == "Cancelled")
                return Result.Fail($"Cannot update {order.Status.ToLower()} production order");

            order.ItemId = orderDto.ItemId;
            order.Quantity = orderDto.Quantity;
            order.PlannedStartDate = orderDto.PlannedStartDate;
            order.PlannedEndDate = orderDto.PlannedEndDate;
            order.Status = orderDto.Status; // Fix: Update status field
            order.Notes = orderDto.Notes;
            order.UpdatedAt = DateTime.UtcNow;

            // Update order items if provided
            if (orderDto.Items != null && orderDto.Items.Any())
            {
                // Get existing order items
                var existingItems = await _poItemRepository.FindAsync(oi => oi.ProductionOrderId == order.Id);
                
                // Remove items that are no longer in the DTO
                var itemsToRemove = existingItems.Where(ei => 
                    !orderDto.Items.Any(dtoItem => dtoItem.Id == ei.Id)).ToList();
                foreach (var itemToRemove in itemsToRemove)
                {
                    await _poItemRepository.DeleteAsync(itemToRemove);
                }

                // Update or add items
                foreach (var itemDto in orderDto.Items)
                {
                    if (itemDto.Id > 0)
                    {
                        // Update existing item
                        var existingItem = existingItems.FirstOrDefault(ei => ei.Id == itemDto.Id);
                        if (existingItem != null)
                        {
                            existingItem.BomItemId = itemDto.BomItemId;
                            existingItem.ItemId = itemDto.ItemId;
                            existingItem.Sequence = itemDto.Sequence;
                            existingItem.Quantity = itemDto.Quantity;
                            existingItem.UnitOfMeasure = itemDto.UnitOfMeasure;
                            existingItem.BomQuantity = itemDto.BomQuantity > 0 ? itemDto.BomQuantity : itemDto.Quantity;
                            existingItem.Notes = itemDto.Notes;
                            existingItem.UpdatedAt = DateTime.UtcNow;
                            await _poItemRepository.UpdateAsync(existingItem);
                        }
                    }
                    else
                    {
                        // Add new item
                        var newItem = new ProductionOrderItem
                        {
                            ProductionOrderId = order.Id,
                            BomItemId = itemDto.BomItemId,
                            ItemId = itemDto.ItemId,
                            Sequence = itemDto.Sequence,
                            Quantity = itemDto.Quantity,
                            UnitOfMeasure = itemDto.UnitOfMeasure,
                            BomQuantity = itemDto.BomQuantity > 0 ? itemDto.BomQuantity : itemDto.Quantity,
                            Notes = itemDto.Notes,
                            CreatedByUserId = order.CreatedByUserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _poItemRepository.AddAsync(newItem);
                    }
                }
            }

            await _poRepository.UpdateAsync(order);

            return Result.Ok("Production order updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating production order: {ex.Message}");
        }
    }

    public async Task<Result> DeleteOrderAsync(int id)
    {
        try
        {
            var order = await _poRepository.GetByIdAsync(id);
            if (order == null)
                return Result.Fail("Production order not found");

            if (order.Status != "Draft")
                return Result.Fail("Can only delete draft production orders");

            await _poRepository.DeleteAsync(order);

            return Result.Ok("Production order deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting production order: {ex.Message}");
        }
    }

    public async Task<Result> StartProductionAsync(int id)
    {
        try
        {
            var order = await _poRepository.GetByIdAsync(id);
            if (order == null)
                return Result.Fail("Production order not found");

            if (order.Status == "InProgress")
                return Result.Fail("Production is already in progress");

            if (order.Status == "Completed")
                return Result.Fail("Production is already completed");

            if (order.Status == "Cancelled")
                return Result.Fail("Cannot start cancelled production order");

            // Check material availability via Material Issue
            var materialIssues = await _materialIssueService.GetByProductionOrderIdAsync(id);
            if (!materialIssues.Success || materialIssues.Data == null || !materialIssues.Data.Any())
            {
                return Result.Fail("Cannot start production: No material issue found. Please issue materials first.");
            }

            var hasPostedMaterialIssue = materialIssues.Data.Any(mi => mi.Status == "Posted");
            if (!hasPostedMaterialIssue)
            {
                return Result.Fail("Cannot start production: Material issue must be posted first.");
            }

            order.Status = "InProgress";
            order.ActualStartDate = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _poRepository.UpdateAsync(order);

            return Result.Ok("Production started successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error starting production: {ex.Message}");
        }
    }

    public async Task<Result> CompleteProductionAsync(int id)
    {
        try
        {
            var order = await _poRepository.GetByIdAsync(id);
            if (order == null)
                return Result.Fail("Production order not found");

            if (order.Status != "InProgress")
                return Result.Fail("Production must be in progress to complete");

            // Check if Final QC is passed (commented out - QCStatus column not in database yet)
            // if (string.IsNullOrEmpty(order.QCStatus) || order.QCStatus == "Pending")
            // {
            //     return Result.Fail("Cannot complete production: Final QC is required");
            // }

            // if (order.QCStatus == "Failed")
            // {
            //     return Result.Fail("Cannot complete production: Final QC failed");
            // }

            // Create Goods Receipt from Production (receive finished goods into inventory)
            var item = await _itemRepository.GetByIdAsync(order.ItemId);
            if (item == null)
                return Result.Fail("Item not found");

            // TODO: Create GR for finished goods
            // This would require creating a GR with source as Production Order
            // For now, just mark as completed

            order.Status = "Completed";
            order.ActualEndDate = DateTime.UtcNow;
            // order.CompletedQuantity = order.Quantity; // Ignored - column not in database yet
            order.UpdatedAt = DateTime.UtcNow;

            await _poRepository.UpdateAsync(order);

            return Result.Ok("Production completed successfully. Finished goods should be received into inventory.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error completing production: {ex.Message}");
        }
    }

    public async Task<Result> CancelProductionAsync(int id)
    {
        try
        {
            var order = await _poRepository.GetByIdAsync(id);
            if (order == null)
                return Result.Fail("Production order not found");

            if (order.Status == "Completed")
                return Result.Fail("Cannot cancel completed production order");

            if (order.Status == "Cancelled")
                return Result.Fail("Production order is already cancelled");

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;

            await _poRepository.UpdateAsync(order);

            return Result.Ok("Production order cancelled successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error cancelling production order: {ex.Message}");
        }
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var orders = await _poRepository.GetAllAsync();
        var lastNumber = orders.Count;
        return $"PO-{DateTime.Now.Year}-{(lastNumber + 1):D4}";
    }

    private async Task<ProductionOrderDto> MapToDtoAsync(ProductionOrder order)
    {
        var item = await _itemRepository.GetByIdAsync(order.ItemId);
        
        // Load Order Items
        var orderItems = await _poItemRepository.FindAsync(oi => oi.ProductionOrderId == order.Id);
        
        // Load Processes
        var processes = await _poProcessRepository.FindAsync(p => p.ProductionOrderId == order.Id);
        
        // Load QC Steps
        var qcSteps = await _poQcRepository.FindAsync(q => q.ProductionOrderId == order.Id);
        
        var dto = new ProductionOrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ItemId = order.ItemId,
            ItemCode = item?.Code ?? string.Empty,
            ItemName = item?.Name ?? string.Empty,
            Quantity = order.Quantity,
            CompletedQuantity = 0, // Default value - column not in database yet
            PlannedStartDate = order.PlannedStartDate,
            PlannedEndDate = order.PlannedEndDate,
            ActualStartDate = order.ActualStartDate,
            ActualEndDate = order.ActualEndDate,
            Status = order.Status,
            Notes = order.Notes,
            CreatedByUserId = order.CreatedByUserId,
            CreatedAt = order.CreatedAt,
            // BOM Reference
            BillOfMaterialId = order.BillOfMaterialId,
            BomCode = order.BomCode,
            BomName = order.BomName,
            // Items
            Items = new List<ProductionOrderItemDto>(),
            // Processes
            Processes = new List<ProductionOrderProcessesDto>(),
            // QC Steps
            QcSteps = new List<ProductionOrderQcsDto>()
        };

        // Map Order Items
        foreach (var orderItem in orderItems.OrderBy(oi => oi.Sequence))
        {
            var itemEntity = await _itemRepository.GetByIdAsync(orderItem.ItemId);
            dto.Items.Add(new ProductionOrderItemDto
            {
                Id = orderItem.Id,
                ProductionOrderId = orderItem.ProductionOrderId,
                BomItemId = orderItem.BomItemId,
                ItemId = orderItem.ItemId,
                ItemCode = itemEntity?.Code ?? string.Empty,
                ItemName = itemEntity?.Name ?? string.Empty,
                Sequence = orderItem.Sequence,
                Quantity = orderItem.Quantity,
                UnitOfMeasure = orderItem.UnitOfMeasure,
                BomQuantity = orderItem.BomQuantity,
                Notes = orderItem.Notes
            });
        }

        // Map Processes
        foreach (var process in processes.OrderBy(p => p.Sequence))
        {
            dto.Processes.Add(new ProductionOrderProcessesDto
            {
                Id = process.Id,
                ProductionOrderId = process.ProductionOrderId,
                BomProcessId = process.BomProcessId,
                Sequence = process.Sequence,
                WorkCode = process.WorkCode,
                WorkName = process.WorkName,
                NumberOfPersons = process.NumberOfPersons,
                Quantity = process.Quantity,
                UnitOfMeasure = process.UnitOfMeasure,
                Status = process.Status,
                StartedAt = process.StartedAt,
                CompletedAt = process.CompletedAt,
                Notes = process.Notes
            });
        }

        // Map QC Steps
        foreach (var qcStep in qcSteps.OrderBy(q => q.Sequence))
        {
            dto.QcSteps.Add(new ProductionOrderQcsDto
            {
                Id = qcStep.Id,
                ProductionOrderId = qcStep.ProductionOrderId,
                BomQcId = qcStep.BomQcId,
                Sequence = qcStep.Sequence,
                QcCode = qcStep.QcCode,
                QcName = qcStep.QcName,
                QcValues = qcStep.QcValues,
                Status = qcStep.Status,
                CheckedAt = qcStep.CheckedAt,
                CheckedByUserId = qcStep.CheckedByUserId,
                ActualValues = qcStep.ActualValues,
                Notes = qcStep.Notes
            });
        }

        return dto;
    }
}
