using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IProductionService
{
    Task<Result<List<ProductionOrderDto>>> GetAllOrdersAsync();
    Task<Result<ProductionOrderDto>> GetOrderByIdAsync(int id);
    Task<Result<int>> CreateOrderAsync(ProductionOrderDto orderDto);
    Task<Result> UpdateOrderAsync(ProductionOrderDto orderDto);
    Task<Result> DeleteOrderAsync(int id);
    Task<Result> StartProductionAsync(int id);
    Task<Result> CompleteProductionAsync(int id);
    Task<Result> CancelProductionAsync(int id);
}
