using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Domain.Enums;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IAssetRepository _assetRepository;

    public RentalService(IRentalRepository rentalRepository, IAssetRepository assetRepository)
    {
        _rentalRepository = rentalRepository;
        _assetRepository = assetRepository;
    }

    public async Task<Result<List<RentalDto>>> GetAllAsync()
    {
        var rentals = await _rentalRepository.GetAllAsync();
        var dtos = rentals.Select(r => new RentalDto
        {
            Id = r.Id,
            ContractNumber = r.ContractNumber,
            AssetId = r.AssetId,
            CustomerName = r.CustomerName,
            CustomerPhone = r.CustomerPhone,
            RentalStartDate = r.RentalStartDate,
            RentalEndDate = r.RentalEndDate,
            ActualReturnDate = r.ActualReturnDate,
            TotalAmount = r.TotalAmount,
            Status = r.Status
        }).ToList();

        return Result<List<RentalDto>>.Ok(dtos);
    }

    public async Task<Result<RentalDto>> GetByIdAsync(int id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null) return Result<RentalDto>.Fail("Rental contract not found");

        var dto = new RentalDto
        {
            Id = rental.Id,
            ContractNumber = rental.ContractNumber,
            AssetId = rental.AssetId,
            CustomerName = rental.CustomerName,
            CustomerPhone = rental.CustomerPhone,
            RentalStartDate = rental.RentalStartDate,
            RentalEndDate = rental.RentalEndDate,
            ActualReturnDate = rental.ActualReturnDate,
            TotalAmount = rental.TotalAmount,
            Status = rental.Status
        };

        return Result<RentalDto>.Ok(dto);
    }

    public async Task<Result<int>> CreateContractAsync(RentalDto rentalDto)
    {
        var asset = await _assetRepository.GetByIdAsync(rentalDto.AssetId);
        if (asset == null) return Result<int>.Fail("Asset not found");
        if (asset.Status != AssetStatus.Available) return Result<int>.Fail("Asset is not available for rental");

        var contract = new RentalContract
        {
            ContractNumber = rentalDto.ContractNumber,
            AssetId = rentalDto.AssetId,
            CustomerName = rentalDto.CustomerName,
            CustomerPhone = rentalDto.CustomerPhone,
            RentalStartDate = rentalDto.RentalStartDate,
            RentalEndDate = rentalDto.RentalEndDate,
            TotalAmount = rentalDto.TotalAmount,
            Status = RentalStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _rentalRepository.AddAsync(contract);

        // Update asset status
        asset.Status = AssetStatus.Rented;
        await _assetRepository.UpdateAsync(asset);

        return Result<int>.Ok(contract.Id, "Rental contract created successfully");
    }

    public async Task<Result> ReturnAssetAsync(int contractId, DateTime returnDate, string? notes)
    {
        var contract = await _rentalRepository.GetByIdAsync(contractId);
        if (contract == null) return Result.Fail("Contract not found");

        contract.ActualReturnDate = returnDate;
        contract.Status = RentalStatus.Completed;
        contract.Notes = notes;

        await _rentalRepository.UpdateAsync(contract);

        // Update asset status
        var asset = await _assetRepository.GetByIdAsync(contract.AssetId);
        if (asset != null)
        {
            asset.Status = AssetStatus.Available; // Or NeedsInspection
            await _assetRepository.UpdateAsync(asset);
        }

        return Result.Ok("Asset returned successfully");
    }

    public async Task<Result> CancelContractAsync(int contractId)
    {
        var contract = await _rentalRepository.GetByIdAsync(contractId);
        if (contract == null) return Result.Fail("Contract not found");

        contract.Status = RentalStatus.Cancelled;
        await _rentalRepository.UpdateAsync(contract);

        // Update asset status
        var asset = await _assetRepository.GetByIdAsync(contract.AssetId);
        if (asset != null)
        {
            asset.Status = AssetStatus.Available;
            await _assetRepository.UpdateAsync(asset);
        }

        return Result.Ok("Contract cancelled successfully");
    }

    public async Task<Result<PagedResult<RentalDto>>> GetPagedAsync(RentalContractFilterParameters filter)
    {
        var pagedRentals = await _rentalRepository.GetPagedRentalContractsAsync(filter);
        
        var dtos = pagedRentals.Items.Select(r => new RentalDto
        {
            Id = r.Id,
            ContractNumber = r.ContractNumber,
            AssetId = r.AssetId,
            AssetCode = r.Asset?.AssetCode ?? string.Empty,
            CustomerName = r.CustomerName,
            CustomerPhone = r.CustomerPhone,
            RentalStartDate = r.RentalStartDate,
            RentalEndDate = r.RentalEndDate,
            ActualReturnDate = r.ActualReturnDate,
            TotalAmount = r.TotalAmount,
            Status = r.Status
        }).ToList();

        var result = new PagedResult<RentalDto>(dtos, pagedRentals.TotalCount, pagedRentals.PageNumber, pagedRentals.PageSize);
        return Result<PagedResult<RentalDto>>.Ok(result);
    }
}

