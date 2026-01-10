using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class PurchaseRequisitionService : IPurchaseRequisitionService
{
    private readonly IPurchaseRequisitionRepository _prRepository;
    private readonly IRepository<Item> _itemRepository;

    public PurchaseRequisitionService(
        IPurchaseRequisitionRepository prRepository,
        IRepository<Item> itemRepository)
    {
        _prRepository = prRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Result<List<PurchaseRequisitionDto>>> GetAllAsync()
    {
        try
        {
            var prs = await _prRepository.GetAllWithItemsAsync();
            var dtos = prs.Select(MapToDto).ToList();
            return Result<List<PurchaseRequisitionDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<PurchaseRequisitionDto>>.Fail($"Error retrieving purchase requisitions: {ex.Message}");
        }
    }

    public async Task<Result<PurchaseRequisitionDto>> GetByIdAsync(int id)
    {
        try
        {
            var pr = await _prRepository.GetByIdWithItemsAsync(id);
            if (pr == null)
                return Result<PurchaseRequisitionDto>.Fail("Purchase requisition not found");

            return Result<PurchaseRequisitionDto>.Ok(MapToDto(pr));
        }
        catch (Exception ex)
        {
            return Result<PurchaseRequisitionDto>.Fail($"Error retrieving purchase requisition: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(PurchaseRequisitionDto dto, int userId)
    {
        try
        {
            var documentNumber = await _prRepository.GenerateDocumentNumberAsync();

            var pr = new PurchaseRequisition
            {
                DocumentNumber = documentNumber,
                Date = dto.Date,
                DepartmentName = dto.DepartmentName,
                RequestorName = dto.RequestorName,
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

                pr.Items.Add(new PurchaseRequisitionItem
                {
                    ItemId = itemDto.ItemId,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    EstimatedUnitPrice = itemDto.EstimatedUnitPrice,
                    RequiredDate = itemDto.RequiredDate,
                    Notes = itemDto.Notes
                });
            }

            await _prRepository.AddAsync(pr);
            return Result<int>.Ok(pr.Id, "Purchase requisition created successfully");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating purchase requisition: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(int id, PurchaseRequisitionDto dto, int userId)
    {
        try
        {
            var pr = await _prRepository.GetByIdWithItemsAsync(id);
            if (pr == null)
                return Result.Fail("Purchase requisition not found");

            if (pr.Status != "Draft" && pr.Status != dto.Status)
                return Result.Fail("Only draft purchase requisitions can be updated");

            pr.Date = dto.Date;
            pr.DepartmentName = dto.DepartmentName;
            pr.RequestorName = dto.RequestorName;
            pr.Status = dto.Status;
            pr.Notes = dto.Notes;
            pr.UpdatedAt = DateTime.UtcNow;

            pr.Items.Clear();
            foreach (var itemDto in dto.Items)
            {
                var item = await _itemRepository.GetByIdAsync(itemDto.ItemId);
                if (item == null)
                    return Result.Fail($"Item with ID {itemDto.ItemId} not found");

                pr.Items.Add(new PurchaseRequisitionItem
                {
                    ItemId = itemDto.ItemId,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitOfMeasure = itemDto.UnitOfMeasure,
                    EstimatedUnitPrice = itemDto.EstimatedUnitPrice,
                    RequiredDate = itemDto.RequiredDate,
                    Notes = itemDto.Notes
                });
            }

            await _prRepository.UpdateAsync(pr);
            return Result.Ok("Purchase requisition updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating purchase requisition: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var pr = await _prRepository.GetByIdAsync(id);
            if (pr == null)
                return Result.Fail("Purchase requisition not found");

            if (pr.Status != "Draft")
                return Result.Fail("Only draft purchase requisitions can be deleted");

            await _prRepository.DeleteAsync(pr);
            return Result.Ok("Purchase requisition deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting purchase requisition: {ex.Message}");
        }
    }

    public async Task<Result> ApproveAsync(int id, int userId)
    {
        try
        {
            var pr = await _prRepository.GetByIdAsync(id);
            if (pr == null)
                return Result.Fail("Purchase requisition not found");

            if (pr.Status != "Pending" && pr.Status != "Draft")
                return Result.Fail("Purchase requisition cannot be approved");

            pr.Status = "Approved";
            pr.UpdatedAt = DateTime.UtcNow;

            await _prRepository.UpdateAsync(pr);
            return Result.Ok("Purchase requisition approved successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error approving purchase requisition: {ex.Message}");
        }
    }

    public async Task<Result> RejectAsync(int id, int userId, string reason)
    {
        try
        {
            var pr = await _prRepository.GetByIdAsync(id);
            if (pr == null)
                return Result.Fail("Purchase requisition not found");

            if (pr.Status != "Pending" && pr.Status != "Draft")
                return Result.Fail("Purchase requisition cannot be rejected");

            pr.Status = "Rejected";
            pr.Notes = $"{pr.Notes}\nRejection reason: {reason}";
            pr.UpdatedAt = DateTime.UtcNow;

            await _prRepository.UpdateAsync(pr);
            return Result.Ok("Purchase requisition rejected");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error rejecting purchase requisition: {ex.Message}");
        }
    }

    public async Task<Result<List<PurchaseRequisitionDto>>> GetApprovedPurchaseRequisitionsAsync()
    {
        try
        {
            var prs = await _prRepository.GetAllWithItemsAsync();
            var approvedPrs = prs.Where(x => x.Status == "Approved").ToList();
            var dtos = approvedPrs.Select(MapToDto).ToList();
            return Result<List<PurchaseRequisitionDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<PurchaseRequisitionDto>>.Fail($"Error retrieving approved purchase requisitions: {ex.Message}");
        }
    }

    private PurchaseRequisitionDto MapToDto(PurchaseRequisition pr)
    {
        return new PurchaseRequisitionDto
        {
            Id = pr.Id,
            DocumentNumber = pr.DocumentNumber,
            Date = pr.Date,
            DepartmentName = pr.DepartmentName,
            RequestorName = pr.RequestorName,
            Status = pr.Status,
            Notes = pr.Notes,
            CreatedAt = pr.CreatedAt,
            Items = pr.Items.Select(i => new PurchaseRequisitionItemDto
            {
                Id = i.Id,
                PurchaseRequisitionId = i.PurchaseRequisitionId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.Code ?? "",
                ItemName = i.Item?.Name ?? "",
                Description = i.Description,
                Quantity = i.Quantity,
                UnitOfMeasure = i.UnitOfMeasure,
                EstimatedUnitPrice = i.EstimatedUnitPrice,
                RequiredDate = i.RequiredDate,
                Notes = i.Notes
            }).ToList()
        };
    }
}
