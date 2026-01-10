using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class QualityCheckService : IQualityCheckService
{
    private readonly IQualityCheckRepository _qcRepository;
    private readonly IRepository<Item> _itemRepository;
    private readonly IGoodsReceiptRepository _grRepository;
    private readonly IRepository<ProductionOrder> _poRepository;

    public QualityCheckService(
        IQualityCheckRepository qcRepository,
        IRepository<Item> itemRepository,
        IGoodsReceiptRepository grRepository,
        IRepository<ProductionOrder> poRepository)
    {
        _qcRepository = qcRepository;
        _itemRepository = itemRepository;
        _grRepository = grRepository;
        _poRepository = poRepository;
    }

    public async Task<Result<List<QualityCheckDto>>> GetAllAsync()
    {
        try
        {
            var qcs = await _qcRepository.GetAllWithDetailsAsync();
            var dtos = qcs.Select(MapToDto).ToList();
            return Result<List<QualityCheckDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<QualityCheckDto>>.Fail($"Error retrieving quality checks: {ex.Message}");
        }
    }

    public async Task<Result<QualityCheckDto>> GetByIdAsync(int id)
    {
        try
        {
            var qc = await _qcRepository.GetByIdWithDetailsAsync(id);
            if (qc == null)
                return Result<QualityCheckDto>.Fail("Quality check not found");

            return Result<QualityCheckDto>.Ok(MapToDto(qc));
        }
        catch (Exception ex)
        {
            return Result<QualityCheckDto>.Fail($"Error retrieving quality check: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(QualityCheckDto dto, int userId)
    {
        try
        {
            var documentNumber = await _qcRepository.GenerateDocumentNumberAsync();

            var qc = new QualityCheck
            {
                DocumentNumber = documentNumber,
                CheckType = dto.CheckType,
                Status = dto.Status, // Use status from DTO instead of hardcoded Pending
                InspectionDate = dto.InspectionDate,
                InspectedBy = dto.InspectedBy,
                GoodsReceiptId = dto.GoodsReceiptId,
                ProductionOrderId = dto.ProductionOrderId,
                Remarks = dto.Remarks,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var itemDto in dto.Items)
            {
                qc.Items.Add(new QualityCheckItem
                {
                    ItemId = itemDto.ItemId,
                    InspectedQuantity = itemDto.InspectedQuantity,
                    PassedQuantity = itemDto.PassedQuantity,
                    RejectedQuantity = itemDto.RejectedQuantity,
                    ItemStatus = itemDto.ItemStatus,
                    DefectDetails = itemDto.DefectDetails,
                    BatchNumber = itemDto.BatchNumber,
                    Notes = itemDto.Notes
                });
            }

            await _qcRepository.AddAsync(qc);

            return Result<int>.Ok(qc.Id, "Quality check created successfully");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating quality check: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(int id, QualityCheckDto dto, int userId)
    {
        try
        {
            var qc = await _qcRepository.GetByIdWithDetailsAsync(id);
            if (qc == null)
                return Result.Fail("Quality check not found");

            qc.InspectionDate = dto.InspectionDate;
            qc.InspectedBy = dto.InspectedBy;
            qc.Status = dto.Status;
            qc.DefectDescription = dto.DefectDescription;
            qc.CorrectionAction = dto.CorrectionAction;
            qc.Remarks = dto.Remarks;
            qc.UpdatedAt = DateTime.UtcNow;

            // Update items
            qc.Items.Clear();
            foreach (var itemDto in dto.Items)
            {
                qc.Items.Add(new QualityCheckItem
                {
                    ItemId = itemDto.ItemId,
                    InspectedQuantity = itemDto.InspectedQuantity,
                    PassedQuantity = itemDto.PassedQuantity,
                    RejectedQuantity = itemDto.RejectedQuantity,
                    ItemStatus = itemDto.ItemStatus,
                    DefectDetails = itemDto.DefectDetails,
                    BatchNumber = itemDto.BatchNumber,
                    Notes = itemDto.Notes
                });
            }

            await _qcRepository.UpdateAsync(qc);

            return Result.Ok("Quality check updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating quality check: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var qc = await _qcRepository.GetByIdAsync(id);
            if (qc == null)
                return Result.Fail("Quality check not found");

            if (qc.Status == QualityCheckStatus.Passed || qc.Status == QualityCheckStatus.Failed)
                return Result.Fail("Cannot delete an approved or rejected quality check");

            await _qcRepository.DeleteAsync(qc);

            return Result.Ok("Quality check deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting quality check: {ex.Message}");
        }
    }

    public async Task<Result> ApproveAsync(int id, string approvedBy)
    {
        try
        {
            var qc = await _qcRepository.GetByIdWithDetailsAsync(id);
            if (qc == null)
                return Result.Fail("Quality check not found");

            if (qc.Status == QualityCheckStatus.Passed)
                return Result.Fail("Quality check is already approved");

            // Check if all items passed
            bool allPassed = qc.Items.All(i => i.ItemStatus == QualityCheckStatus.Passed);
            
            qc.Status = allPassed ? QualityCheckStatus.Passed : QualityCheckStatus.Conditional;
            qc.ApprovedBy = approvedBy;
            qc.ApprovedDate = DateTime.UtcNow;
            qc.UpdatedAt = DateTime.UtcNow;

            // Update related document QC status
            if (qc.GoodsReceiptId.HasValue)
            {
                var gr = await _grRepository.GetByIdWithItemsAsync(qc.GoodsReceiptId.Value);
                if (gr != null)
                {
                    foreach (var grItem in gr.Items)
                    {
                        var qcItem = qc.Items.FirstOrDefault(i => i.ItemId == grItem.ItemId);
                        if (qcItem != null)
                        {
                            grItem.QCStatus = qcItem.ItemStatus.ToString();
                            grItem.QualityCheckId = qc.Id;
                        }
                    }
                }
            }

            if (qc.ProductionOrderId.HasValue)
            {
                var po = await _poRepository.GetByIdAsync(qc.ProductionOrderId.Value);
                if (po != null)
                {
                    if (qc.CheckType == QualityCheckType.InProcess)
                        po.InProcessQCId = qc.Id;
                    else if (qc.CheckType == QualityCheckType.Final)
                        po.FinalQCId = qc.Id;
                    
                    po.QCStatus = qc.Status.ToString();
                }
            }

            await _qcRepository.UpdateAsync(qc);

            return Result.Ok("Quality check approved successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error approving quality check: {ex.Message}");
        }
    }

    public async Task<Result> RejectAsync(int id, string rejectedBy, string reason)
    {
        try
        {
            var qc = await _qcRepository.GetByIdWithDetailsAsync(id);
            if (qc == null)
                return Result.Fail("Quality check not found");

            qc.Status = QualityCheckStatus.Failed;
            qc.ApprovedBy = rejectedBy;
            qc.ApprovedDate = DateTime.UtcNow;
            qc.DefectDescription = reason;
            qc.UpdatedAt = DateTime.UtcNow;

            // Update related document QC status
            if (qc.GoodsReceiptId.HasValue)
            {
                var gr = await _grRepository.GetByIdWithItemsAsync(qc.GoodsReceiptId.Value);
                if (gr != null)
                {
                    foreach (var grItem in gr.Items)
                    {
                        var qcItem = qc.Items.FirstOrDefault(i => i.ItemId == grItem.ItemId);
                        if (qcItem != null)
                        {
                            grItem.QCStatus = "Failed";
                            grItem.QualityCheckId = qc.Id;
                        }
                    }
                }
            }

            if (qc.ProductionOrderId.HasValue)
            {
                var po = await _poRepository.GetByIdAsync(qc.ProductionOrderId.Value);
                if (po != null)
                {
                    po.QCStatus = "Failed";
                }
            }

            await _qcRepository.UpdateAsync(qc);

            return Result.Ok("Quality check rejected");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error rejecting quality check: {ex.Message}");
        }
    }

    public async Task<Result<List<QualityCheckDto>>> GetByGoodsReceiptIdAsync(int grId)
    {
        try
        {
            var qcs = await _qcRepository.GetByGoodsReceiptIdAsync(grId);
            var dtos = qcs.Select(MapToDto).ToList();
            return Result<List<QualityCheckDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<QualityCheckDto>>.Fail($"Error retrieving quality checks: {ex.Message}");
        }
    }

    public async Task<Result<List<QualityCheckDto>>> GetByProductionOrderIdAsync(int poId)
    {
        try
        {
            var qcs = await _qcRepository.GetByProductionOrderIdAsync(poId);
            var dtos = qcs.Select(MapToDto).ToList();
            return Result<List<QualityCheckDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<QualityCheckDto>>.Fail($"Error retrieving quality checks: {ex.Message}");
        }
    }

    private QualityCheckDto MapToDto(QualityCheck qc)
    {
        return new QualityCheckDto
        {
            Id = qc.Id,
            DocumentNumber = qc.DocumentNumber,
            CheckType = qc.CheckType,
            Status = qc.Status,
            InspectionDate = qc.InspectionDate,
            InspectedBy = qc.InspectedBy,
            GoodsReceiptId = qc.GoodsReceiptId,
            GoodsReceiptNumber = qc.GoodsReceipt?.DocumentNumber,
            ProductionOrderId = qc.ProductionOrderId,
            ProductionOrderNumber = qc.ProductionOrder?.OrderNumber,
            DefectDescription = qc.DefectDescription,
            CorrectionAction = qc.CorrectionAction,
            Remarks = qc.Remarks,
            ApprovedDate = qc.ApprovedDate,
            ApprovedBy = qc.ApprovedBy,
            CreatedAt = qc.CreatedAt,
            Items = qc.Items.Select(i => new QualityCheckItemDto
            {
                Id = i.Id,
                QualityCheckId = i.QualityCheckId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.Code,
                ItemName = i.Item?.Name,
                InspectedQuantity = i.InspectedQuantity,
                PassedQuantity = i.PassedQuantity,
                RejectedQuantity = i.RejectedQuantity,
                ItemStatus = i.ItemStatus,
                DefectDetails = i.DefectDetails,
                BatchNumber = i.BatchNumber,
                Notes = i.Notes
            }).ToList()
        };
    }
}
