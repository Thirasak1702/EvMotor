using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IBomService
{
    Task<Result<List<BomDto>>> GetAllAsync();
    Task<Result<BomDto>> GetByIdAsync(int id);
    Task<Result<BomDto>> GetByCodeAsync(string bomCode);
    Task<Result<int>> CreateAsync(BomDto bomDto);
    Task<Result> UpdateAsync(BomDto bomDto);
    Task<Result> DeleteAsync(int id);
    Task<Result<List<BomDto>>> GetByParentItemIdAsync(int parentItemId);
}
