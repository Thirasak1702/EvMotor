using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class BomService : IBomService
{
    private readonly IBomRepository _bomRepository;
    private readonly IRepository<Item> _itemRepository;

    public BomService(
        IBomRepository bomRepository,
        IRepository<Item> itemRepository)
    {
        _bomRepository = bomRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Result<List<BomDto>>> GetAllAsync()
    {
        try
        {
            var boms = await _bomRepository.GetAllWithDetailsAsync();
            var bomDtos = boms.Select(b => MapToDto(b)).ToList();
            return Result<List<BomDto>>.Ok(bomDtos);
        }
        catch (Exception ex)
        {
            return Result<List<BomDto>>.Fail($"Error retrieving BOMs: {ex.Message}");
        }
    }

    public async Task<Result<BomDto>> GetByIdAsync(int id)
    {
        try
        {
            var bom = await _bomRepository.GetByIdWithDetailsAsync(id);
            if (bom == null)
                return Result<BomDto>.Fail("BOM not found");

            var bomDto = MapToDto(bom);
            return Result<BomDto>.Ok(bomDto);
        }
        catch (Exception ex)
        {
            return Result<BomDto>.Fail($"Error retrieving BOM: {ex.Message}");
        }
    }

    public async Task<Result<BomDto>> GetByCodeAsync(string bomCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(bomCode))
                return Result<BomDto>.Fail("BOM Code is required");

            var bom = await _bomRepository.GetByCodeWithDetailsAsync(bomCode);
            if (bom == null)
                return Result<BomDto>.Fail("BOM not found");

            var bomDto = MapToDto(bom);
            return Result<BomDto>.Ok(bomDto);
        }
        catch (Exception ex)
        {
            return Result<BomDto>.Fail($"Error retrieving BOM: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(BomDto bomDto)
    {
        try
        {
            // Validate Parent Item exists
            var parentItem = await _itemRepository.GetByIdAsync(bomDto.ParentItemId);
            if (parentItem == null)
                return Result<int>.Fail("Parent item not found");

            // Generate BOM Code if empty or "Auto-generated"
            var bomCode = (string.IsNullOrWhiteSpace(bomDto.BomCode) || bomDto.BomCode.Equals("Auto-generated", StringComparison.OrdinalIgnoreCase))
                ? await _bomRepository.GenerateBomCodeAsync() 
                : bomDto.BomCode;

            // Check if BomCode already exists
            if (await _bomRepository.BomCodeExistsAsync(bomCode))
                return Result<int>.Fail("BOM Code already exists");

        var bom = new BillOfMaterial
        {
            BomCode = bomCode,
            ParentItemId = bomDto.ParentItemId,
            Description = bomDto.Description,
            Version = bomDto.Version,
            IsActive = bomDto.IsActive,
            EffectiveDate = bomDto.EffectiveDate,
            ExpiryDate = bomDto.ExpiryDate,
            Notes = bomDto.Notes,
            CreatedByUserId = 1, // TODO: Get from current user
            CreatedAt = DateTime.UtcNow
        };

        // Add BomItems
        foreach (var itemDto in bomDto.BomItems)
        {
            var componentItem = await _itemRepository.GetByIdAsync(itemDto.ComponentItemId);
            if (componentItem == null)
                return Result<int>.Fail($"Component item {itemDto.ComponentItemId} not found");

            bom.BomItems.Add(new BomItem
            {
                ComponentItemId = itemDto.ComponentItemId,
                Quantity = itemDto.Quantity,
                UnitOfMeasure = itemDto.UnitOfMeasure,
                ScrapPercentage = itemDto.ScrapPercentage,
                Sequence = itemDto.Sequence,
                Notes = itemDto.Notes
            });
        }

        // Add BomProcesses
        foreach (var processDto in bomDto.BomProcesses)
        {
            bom.BomProcesses.Add(new BomProcess
            {
                Sequence = processDto.Sequence,
                WorkCode = processDto.WorkCode,
                WorkName = processDto.WorkName,
                NumberOfPersons = processDto.NumberOfPersons,
                Quantity = processDto.Quantity,
                UnitOfMeasure = processDto.UnitOfMeasure,
                Notes = processDto.Notes
            });
        }

        // Add BomQcs
        foreach (var qcDto in bomDto.BomQcs)
        {
            bom.BomQcs.Add(new BomQc
            {
                Sequence = qcDto.Sequence,
                QcCode = qcDto.QcCode,
                QcName = qcDto.QcName,
                QcValues = qcDto.QcValues,
                Notes = qcDto.Notes
            });
        }

            await _bomRepository.AddAsync(bom);
            return Result<int>.Ok(bom.Id, "BOM created successfully");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error creating BOM: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(BomDto bomDto)
    {
        try
        {
            var bom = await _bomRepository.GetByIdWithDetailsAsync(bomDto.Id);
            if (bom == null)
                return Result.Fail("BOM not found");

            // Check if BomCode already exists (excluding current BOM)
            if (await _bomRepository.BomCodeExistsAsync(bomDto.BomCode, bomDto.Id))
                return Result.Fail("BOM Code already exists");

        // Validate Parent Item exists
        var parentItem = await _itemRepository.GetByIdAsync(bomDto.ParentItemId);
        if (parentItem == null)
            return Result.Fail("Parent item not found");

        // Update Header
        bom.BomCode = bomDto.BomCode;
        bom.ParentItemId = bomDto.ParentItemId;
        bom.Description = bomDto.Description;
        bom.Version = bomDto.Version;
        bom.IsActive = bomDto.IsActive;
        bom.EffectiveDate = bomDto.EffectiveDate;
        bom.ExpiryDate = bomDto.ExpiryDate;
        bom.Notes = bomDto.Notes;
        bom.UpdatedAt = DateTime.UtcNow;

        // Update BomItems (Remove all and add new)
        bom.BomItems.Clear();
        foreach (var itemDto in bomDto.BomItems)
        {
            var componentItem = await _itemRepository.GetByIdAsync(itemDto.ComponentItemId);
            if (componentItem == null)
                return Result.Fail($"Component item {itemDto.ComponentItemId} not found");

            bom.BomItems.Add(new BomItem
            {
                ComponentItemId = itemDto.ComponentItemId,
                Quantity = itemDto.Quantity,
                UnitOfMeasure = itemDto.UnitOfMeasure,
                ScrapPercentage = itemDto.ScrapPercentage,
                Sequence = itemDto.Sequence,
                Notes = itemDto.Notes
            });
        }

        // Update BomProcesses (Remove all and add new)
        bom.BomProcesses.Clear();
        foreach (var processDto in bomDto.BomProcesses)
        {
            bom.BomProcesses.Add(new BomProcess
            {
                Sequence = processDto.Sequence,
                WorkCode = processDto.WorkCode,
                WorkName = processDto.WorkName,
                NumberOfPersons = processDto.NumberOfPersons,
                Quantity = processDto.Quantity,
                UnitOfMeasure = processDto.UnitOfMeasure,
                Notes = processDto.Notes
            });
        }

        // Update BomQcs (Remove all and add new)
        bom.BomQcs.Clear();
        foreach (var qcDto in bomDto.BomQcs)
        {
            bom.BomQcs.Add(new BomQc
            {
                Sequence = qcDto.Sequence,
                QcCode = qcDto.QcCode,
                QcName = qcDto.QcName,
                QcValues = qcDto.QcValues,
                Notes = qcDto.Notes
            });
        }

            await _bomRepository.UpdateAsync(bom);
            return Result.Ok("BOM updated successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating BOM: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var bom = await _bomRepository.GetByIdAsync(id);
            if (bom == null)
                return Result.Fail("BOM not found");

            await _bomRepository.DeleteAsync(bom);
            return Result.Ok("BOM deleted successfully");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting BOM: {ex.Message}");
        }
    }

    public async Task<Result<List<BomDto>>> GetByParentItemIdAsync(int parentItemId)
    {
        try
        {
            var boms = await _bomRepository.GetByParentItemIdWithDetailsAsync(parentItemId);
            var bomDtos = boms.Select(b => MapToDto(b)).ToList();
            return Result<List<BomDto>>.Ok(bomDtos);
        }
        catch (Exception ex)
        {
            return Result<List<BomDto>>.Fail($"Error retrieving BOMs: {ex.Message}");
        }
    }

    private BomDto MapToDto(BillOfMaterial bom)
    {
        return new BomDto
        {
            Id = bom.Id,
            BomCode = bom.BomCode,
            ParentItemId = bom.ParentItemId,
            ParentItemName = bom.ParentItem?.Name ?? string.Empty,
            Description = bom.Description,
            Version = bom.Version,
            IsActive = bom.IsActive,
            EffectiveDate = bom.EffectiveDate,
            ExpiryDate = bom.ExpiryDate,
            Notes = bom.Notes,
            BomItems = bom.BomItems
                .OrderBy(bi => bi.Sequence)
                .Select(bi => new BomItemDto
                {
                    Id = bi.Id,
                    BillOfMaterialId = bi.BillOfMaterialId,
                    ComponentItemId = bi.ComponentItemId,
                    ComponentItemCode = bi.ComponentItem?.Code ?? string.Empty,
                    ComponentItemName = bi.ComponentItem?.Name ?? string.Empty,
                    Quantity = bi.Quantity,
                    UnitOfMeasure = bi.UnitOfMeasure,
                    ScrapPercentage = bi.ScrapPercentage,
                    Sequence = bi.Sequence,
                    Notes = bi.Notes
                }).ToList(),
            BomProcesses = bom.BomProcesses
                .OrderBy(bp => bp.Sequence)
                .Select(bp => new BomProcessDto
                {
                    Id = bp.Id,
                    BillOfMaterialId = bp.BillOfMaterialId,
                    Sequence = bp.Sequence,
                    WorkCode = bp.WorkCode,
                    WorkName = bp.WorkName,
                    NumberOfPersons = bp.NumberOfPersons,
                    Quantity = bp.Quantity,
                    UnitOfMeasure = bp.UnitOfMeasure,
                    Notes = bp.Notes
                }).ToList(),
            BomQcs = bom.BomQcs
                .OrderBy(bq => bq.Sequence)
                .Select(bq => new BomQcDto
                {
                    Id = bq.Id,
                    BillOfMaterialId = bq.BillOfMaterialId,
                    Sequence = bq.Sequence,
                    QcCode = bq.QcCode,
                    QcName = bq.QcName,
                    QcValues = bq.QcValues,
                    Notes = bq.Notes
                }).ToList()
        };
    }
}
