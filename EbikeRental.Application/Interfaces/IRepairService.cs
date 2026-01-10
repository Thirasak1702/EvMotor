using EbikeRental.Application.Common.Filters;
using EbikeRental.Application.DTOs;
using EbikeRental.Shared;
using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Interfaces;

public interface IRepairService
{
    Task<Result<List<RepairDto>>> GetAllAsync();
    Task<Result<PagedResult<RepairDto>>> GetPagedAsync(RepairOrderFilterParameters filter);
    Task<Result<RepairDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateRequestAsync(RepairDto repairDto);
    Task<Result> StartRepairAsync(int repairId, int technicianId);
    Task<Result> CompleteRepairAsync(int repairId, decimal cost, string notes);
}
