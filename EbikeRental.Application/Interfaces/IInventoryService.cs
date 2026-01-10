using EbikeRental.Application.DTOs;
using EbikeRental.Shared;

namespace EbikeRental.Application.Interfaces;

public interface IInventoryService
{
    Task<Result<List<StockDto>>> GetStockOverviewAsync();
    Task<Result> TransferStockAsync(int itemId, int fromWarehouseId, int toWarehouseId, int quantity);

    // Stock Inquiry
    Task<Result<decimal>> GetAvailableQuantityAsync(int itemId, int warehouseId, string? batchNumber = null);
    Task<Result<List<StockBalanceDto>>> GetStockBalancesByItemAsync(int itemId);
    Task<Result<List<StockBalanceDto>>> GetStockBalancesByWarehouseAsync(int warehouseId);
    Task<Result<List<InventoryTransactionDto>>> GetTransactionHistoryAsync(int itemId, int warehouseId, DateTime? fromDate = null, DateTime? toDate = null);
    
    // Stock Movements (Core Methods)
    Task<Result<int>> AddStockAsync(AddStockRequest request, int userId);
    Task<Result<int>> DeductStockAsync(DeductStockRequest request, int userId);
    Task<Result<int>> AdjustStockAsync(AdjustStockRequest request, int userId);
    Task<Result<int>> TransferStockAsync(TransferStockRequest request, int userId);
    
    // Validation
    Task<Result> ValidateStockAvailabilityAsync(int itemId, int warehouseId, decimal requiredQuantity, string? batchNumber = null);
}

// DTOs for new Inventory Transaction System
public class StockBalanceDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable { get; set; }
    public decimal AverageCost { get; set; }
    public decimal TotalValue { get; set; }
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class InventoryTransactionDto
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal BalanceQuantity { get; set; }
    public decimal BalanceValue { get; set; }
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Request DTOs
public class AddStockRequest
{
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public string TransactionType { get; set; } = "GoodsReceipt"; // GoodsReceipt, ProductionReceipt, Adjustment, Return
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
}

public class DeductStockRequest
{
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string TransactionType { get; set; } = "MaterialIssue"; // MaterialIssue, Adjustment, Transfer
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? Notes { get; set; }
}

public class AdjustStockRequest
{
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public decimal AdjustmentQuantity { get; set; } // Positive or Negative
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal NewUnitCost { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}

public class TransferStockRequest
{
    public int ItemId { get; set; }
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}
