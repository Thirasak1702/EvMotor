using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IPurchaseOrderRepository _poRepository;
    private readonly IRepository<Item> _itemRepository;

    public PurchaseOrderService(
        IPurchaseOrderRepository poRepository,
        IRepository<Item> itemRepository)
    {
        _poRepository = poRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Result<List<PurchaseOrderDto>>> GetAllAsync()
    {
        try
        {
            var pos = await _poRepository.GetAllWithItemsAsync();
            var dtos = pos.Select(MapToDto).ToList();
            return Result<List<PurchaseOrderDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<PurchaseOrderDto>>.Fail($"Error retrieving purchase orders: {ex.Message}");
        }
    }

    public async Task<Result<PurchaseOrderDto>> GetByIdAsync(int id)
    {
        try
        {
            var po = await _poRepository.GetByIdWithItemsAsync(id);
            if (po == null)
                return Result<PurchaseOrderDto>.Fail("Purchase order not found");

            return Result<PurchaseOrderDto>.Ok(MapToDto(po));
        }
        catch (Exception ex)
        {
            return Result<PurchaseOrderDto>.Fail($"Error retrieving purchase order: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(PurchaseOrderDto dto, int userId)
    {
        try
        {
            var documentNumber = await _poRepository.GenerateDocumentNumberAsync();

            var po = new PurchaseOrder
            {
                DocumentNumber = documentNumber,
                OrderDate = dto.OrderDate,
                PurchaseRequisitionId = dto.PurchaseRequisitionId,
                VendorName = dto.VendorName,
                VendorContact = dto.VendorContact,
                DeliveryAddress = dto.DeliveryAddress,
                ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                Status = dto.Status,
                Notes = dto.Notes,
                TotalAmount = 0,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            foreach (var itemDto in dto.Items)
            {
                var item = await _itemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null)
                    return Result<int>.Fail($"Item with ID {itemDto.ItemId} not found");

                var lineTotal = itemDto.Quantity * itemDto.UnitPrice * 
                               (1 - itemDto.DiscountPercent / 100) * 
                               (1 + itemDto.TaxPercent / 100);
                totalAmount += lineTotal;

                po.Items.Add(new PurchaseOrderItem
                {
                    ItemId = itemDto.ItemId,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    UnitPrice = itemDto.UnitPrice,
                    DiscountPercent = itemDto.DiscountPercent,
                    TaxPercent = itemDto.TaxPercent,
                    Notes = itemDto.Notes
                });
            }

            po.TotalAmount = totalAmount;

            await _poRepository.AddAsync(po);
            return Result<int>.Ok(po.Id, "Purchase order created successfully");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating purchase order: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(int id, PurchaseOrderDto dto, int userId)
    {
        try
        {
            var po = await _poRepository.GetByIdWithItemsAsync(id);
            if (po == null)
                return Result.Fail("Purchase order not found");

            if (po.Status != "Draft")
                return Result.Fail("Only draft purchase orders can be updated");

            po.OrderDate = dto.OrderDate;
            po.PurchaseRequisitionId = dto.PurchaseRequisitionId;
            po.VendorName = dto.VendorName;
            po.Status = dto.Status;
            po.VendorContact = dto.VendorContact;
            po.DeliveryAddress = dto.DeliveryAddress;
            po.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
            po.Notes = dto.Notes;
            po.UpdatedAt = DateTime.UtcNow;

            po.Items.Clear();
            decimal totalAmount = 0;
            foreach (var itemDto in dto.Items)
            {
                var item = await _itemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null)
                    return Result.Fail($"Item with ID {itemDto.ItemId} not found");

                var lineTotal = itemDto.Quantity * itemDto.UnitPrice * 
                               (1 - itemDto.DiscountPercent / 100) * 
                               (1 + itemDto.TaxPercent / 100);
                totalAmount += lineTotal;

                po.Items.Add(new PurchaseOrderItem
                {
                    ItemId = itemDto.ItemId,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    UnitPrice = itemDto.UnitPrice,
                    DiscountPercent = itemDto.DiscountPercent,
                    TaxPercent = itemDto.TaxPercent,
                    Notes = itemDto.Notes
                });
            }

            po.TotalAmount = totalAmount;

            await _poRepository.UpdateAsync(po);
            return Result.Ok("Purchase order updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating purchase order: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var po = await _poRepository.GetByIdAsync(id);
            if (po == null)
                return Result.Fail("Purchase order not found");

            if (po.Status != "Draft")
                return Result.Fail("Only draft purchase orders can be deleted");

            await _poRepository.DeleteAsync(po);
            return Result.Ok("Purchase order deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting purchase order: {ex.Message}");
        }
    }

    public async Task<Result> ConfirmAsync(int id, int userId)
    {
        try
        {
            var po = await _poRepository.GetByIdAsync(id);
            if (po == null)
                return Result.Fail("Purchase order not found");

            if (po.Status != "Sent" && po.Status != "Draft")
                return Result.Fail("Purchase order cannot be confirmed");

            po.Status = "Confirmed";
            po.UpdatedAt = DateTime.UtcNow;

            await _poRepository.UpdateAsync(po);
            return Result.Ok("Purchase order confirmed successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error confirming purchase order: {ex.Message}");
        }
    }

    public async Task<Result> CancelAsync(int id, int userId, string reason)
    {
        try
        {
            var po = await _poRepository.GetByIdAsync(id);
            if (po == null)
                return Result.Fail("Purchase order not found");

            if (po.Status == "Cancelled" || po.Status == "Received")
                return Result.Fail("Purchase order cannot be cancelled");

            po.Status = "Cancelled";
            po.Notes = $"{po.Notes}\nCancellation reason: {reason}";
            po.UpdatedAt = DateTime.UtcNow;

            await _poRepository.UpdateAsync(po);
            return Result.Ok("Purchase order cancelled");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error cancelling purchase order: {ex.Message}");
        }
    }

    public async Task<Result<List<PurchaseOrderDto>>> GetApprovedPurchaseOrdersAsync()
    {
        try
        {
            var pos = await _poRepository.GetAllWithItemsAsync();
            var approvedPos = pos.Where(x => x.Status == "Confirmed" || x.Status == "Sent").ToList();
            var dtos = approvedPos.Select(MapToDto).ToList();
            return Result<List<PurchaseOrderDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<PurchaseOrderDto>>.Fail($"Error retrieving approved purchase orders: {ex.Message}");
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            DocumentNumber = po.DocumentNumber,
            OrderDate = po.OrderDate,
            PurchaseRequisitionId = po.PurchaseRequisitionId,
            PurchaseRequisitionNumber = po.PurchaseRequisition?.DocumentNumber ?? "",
            VendorName = po.VendorName,
            VendorContact = po.VendorContact,
            DeliveryAddress = po.DeliveryAddress,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            Status = po.Status,
            Notes = po.Notes,
            TotalAmount = po.TotalAmount,
            CreatedAt = po.CreatedAt,
            Items = po.Items.Select(i => new PurchaseOrderItemDto
            {
                Id = i.Id,
                PurchaseOrderId = i.PurchaseOrderId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.Code ?? "",
                ItemName = i.Item?.Name ?? "",
                Description = i.Description,
                Quantity = i.Quantity,
                UnitOfMeasure = i.UnitOfMeasure,
                UnitPrice = i.UnitPrice,
                DiscountPercent = i.DiscountPercent,
                TaxPercent = i.TaxPercent,
                Notes = i.Notes
            }).ToList()
        };
    }
}
