using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IGoodsReceiptService
{
    Task<Result<List<GoodsReceiptDto>>> GetAllAsync();
    Task<Result<GoodsReceiptDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(GoodsReceiptDto dto, int userId);
    Task<Result> UpdateAsync(int id, GoodsReceiptDto dto, int userId);
    Task<Result> DeleteAsync(int id);
    Task<Result> PostAsync(int id, int userId);
    Task<Result> CancelAsync(int id, int userId, string reason);
}
