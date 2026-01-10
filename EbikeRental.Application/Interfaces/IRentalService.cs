using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces;

public interface IRentalService
{
    Task<Result<List<RentalDto>>> GetAllAsync();
    Task<Result<PagedResult<RentalDto>>> GetPagedAsync(RentalContractFilterParameters filter);
    Task<Result<RentalDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateContractAsync(RentalDto rentalDto);
    Task<Result> ReturnAssetAsync(int contractId, DateTime returnDate, string? notes);
    Task<Result> CancelContractAsync(int contractId);
}
