using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using EbikeRental.Shared;

namespace EbikeRental.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryTransactionRepository _transactionRepo;
    private readonly IStockBalanceRepository _balanceRepo;
    private readonly IRepository<Item> _itemRepo;
    private readonly IRepository<Warehouse> _warehouseRepo;

    public InventoryService(
        IInventoryTransactionRepository transactionRepo,
        IStockBalanceRepository balanceRepo,
        IRepository<Item> itemRepo,
        IRepository<Warehouse> warehouseRepo)
    {
        _transactionRepo = transactionRepo;
        _balanceRepo = balanceRepo;
        _itemRepo = itemRepo;
        _warehouseRepo = warehouseRepo;
    }

    #region Old Methods (Backward Compatibility)

    public async Task<Result<List<StockDto>>> GetStockOverviewAsync()
    {
        try
        {
            var balances = await _balanceRepo.GetAllAsync();
            var stockDtos = balances.Select(b => new StockDto
            {
                ItemId = b.ItemId,
                ItemCode = b.Item?.Code ?? string.Empty,
                ItemName = b.Item?.Name ?? string.Empty,
                WarehouseId = b.WarehouseId,
                WarehouseName = b.Warehouse?.Name ?? string.Empty,
                Quantity = (int)b.QuantityOnHand,
                AvailableQuantity = (int)b.QuantityAvailable,
                RentedQuantity = 0, // Not tracked yet
                MaintenanceQuantity = 0 // Not tracked yet
            }).ToList();

            return Result<List<StockDto>>.Ok(stockDtos);
        }
        catch (Exception ex)
        {
            return Result<List<StockDto>>.Fail($"Error getting stock overview: {ex.Message}");
        }
    }

    public async Task<Result> TransferStockAsync(int itemId, int fromWarehouseId, int toWarehouseId, int quantity)
    {
        try
        {
            var request = new TransferStockRequest
            {
                ItemId = itemId,
                FromWarehouseId = fromWarehouseId,
                ToWarehouseId = toWarehouseId,
                Quantity = quantity,
                UnitOfMeasure = "PCS", // Default
                Notes = "Stock transfer (legacy method)"
            };

            var result = await TransferStockAsync(request, 1); // Default userId = 1
            
            if (result.Success)
                return Result.Ok(result.Message);
            else
                return Result.Fail(result.Message);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error transferring stock: {ex.Message}");
        }
    }

    #endregion

    #region Stock Inquiry

    public async Task<Result<decimal>> GetAvailableQuantityAsync(int itemId, int warehouseId, string? batchNumber = null)
    {
        try
        {
            var quantity = await _balanceRepo.GetAvailableQuantityAsync(itemId, warehouseId, batchNumber);
            return Result<decimal>.Ok(quantity);
        }
        catch (Exception ex)
        {
            return Result<decimal>.Fail($"Error getting available quantity: {ex.Message}");
        }
    }

    public async Task<Result<List<StockBalanceDto>>> GetStockBalancesByItemAsync(int itemId)
    {
        try
        {
            var balances = await _balanceRepo.GetByItemAsync(itemId);
            var dtos = balances.Select(MapToStockBalanceDto).ToList();
            return Result<List<StockBalanceDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<StockBalanceDto>>.Fail($"Error getting stock balances: {ex.Message}");
        }
    }

    public async Task<Result<List<StockBalanceDto>>> GetStockBalancesByWarehouseAsync(int warehouseId)
    {
        try
        {
            var balances = await _balanceRepo.GetByWarehouseAsync(warehouseId);
            var dtos = balances.Select(MapToStockBalanceDto).ToList();
            return Result<List<StockBalanceDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<StockBalanceDto>>.Fail($"Error getting stock balances: {ex.Message}");
        }
    }

    public async Task<Result<List<InventoryTransactionDto>>> GetTransactionHistoryAsync(int itemId, int warehouseId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            var transactions = await _transactionRepo.GetByItemAndWarehouseAsync(itemId, warehouseId);
            
            if (fromDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate >= fromDate.Value);
            
            if (toDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate <= toDate.Value);
            
            var dtos = transactions.Select(MapToTransactionDto).ToList();
            return Result<List<InventoryTransactionDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<InventoryTransactionDto>>.Fail($"Error getting transaction history: {ex.Message}");
        }
    }

    #endregion

    #region Stock Movements

    public async Task<Result<int>> AddStockAsync(AddStockRequest request, int userId)
    {
        try
        {
            Console.WriteLine($"   [INVENTORY] AddStockAsync started");
            Console.WriteLine($"               ItemId: {request.ItemId}");
            Console.WriteLine($"               WarehouseId: {request.WarehouseId}");
            Console.WriteLine($"               Quantity: {request.Quantity}");
            Console.WriteLine($"               UnitCost: {request.UnitCost}");

            // Validation
            var validationResult = await ValidateStockRequest(request.ItemId, request.WarehouseId, request.Quantity);
            if (!validationResult.Success)
            {
                Console.WriteLine($"   [INVENTORY] ? Validation failed: {validationResult.Message}");
                return Result<int>.Fail(validationResult.Message);
            }

            Console.WriteLine($"   [INVENTORY] ? Validation passed");

            // Get or Create Stock Balance
            var balance = await _balanceRepo.GetByItemAndWarehouseAsync(
                request.ItemId, 
                request.WarehouseId, 
                request.BatchNumber, 
                request.SerialNumber);

            if (balance == null)
            {
                Console.WriteLine($"   [INVENTORY] Creating new StockBalance...");
                
                // Create new balance
                balance = new StockBalance
                {
                    ItemId = request.ItemId,
                    WarehouseId = request.WarehouseId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0,
                    AverageCost = 0,
                    BatchNumber = request.BatchNumber,
                    SerialNumber = request.SerialNumber,
                    ExpiryDate = request.ExpiryDate,
                    LastUpdated = DateTime.UtcNow
                };
                await _balanceRepo.AddAsync(balance);
                
                Console.WriteLine($"   [INVENTORY] StockBalance created");
                
                // Reload to get Id
                balance = await _balanceRepo.GetByItemAndWarehouseAsync(
                    request.ItemId, 
                    request.WarehouseId, 
                    request.BatchNumber, 
                    request.SerialNumber);
                
                Console.WriteLine($"   [INVENTORY] StockBalance reloaded, Id: {balance?.Id}");
            }
            else
            {
                Console.WriteLine($"   [INVENTORY] StockBalance found, Id: {balance.Id}");
                Console.WriteLine($"               Current Qty: {balance.QuantityOnHand}");
                Console.WriteLine($"               Current Cost: {balance.AverageCost}");
            }

            // Calculate new average cost (Weighted Average Method)
            var oldTotalValue = balance!.QuantityOnHand * balance.AverageCost;
            var newTotalValue = request.Quantity * request.UnitCost;
            var newQuantity = balance.QuantityOnHand + request.Quantity;
            
            balance.AverageCost = newQuantity > 0 ? (oldTotalValue + newTotalValue) / newQuantity : request.UnitCost;
            balance.QuantityOnHand = newQuantity;
            balance.LastUpdated = DateTime.UtcNow;

            Console.WriteLine($"   [INVENTORY] Updating StockBalance...");
            Console.WriteLine($"               New Qty: {balance.QuantityOnHand}");
            Console.WriteLine($"               New Avg Cost: {balance.AverageCost}");

            await _balanceRepo.UpdateAsync(balance);
            
            Console.WriteLine($"   [INVENTORY] ? StockBalance updated");

            // Create Transaction Record
            var transactionNumber = await _transactionRepo.GenerateTransactionNumberAsync();
            Console.WriteLine($"   [INVENTORY] Transaction Number: {transactionNumber}");
            
            var transaction = new InventoryTransaction
            {
                TransactionNumber = transactionNumber,
                TransactionDate = DateTime.UtcNow,
                ItemId = request.ItemId,
                WarehouseId = request.WarehouseId,
                TransactionType = request.TransactionType,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId,
                ReferenceNumber = request.ReferenceNumber,
                Quantity = request.Quantity, // Positive
                UnitOfMeasure = request.UnitOfMeasure,
                UnitCost = request.UnitCost,
                TotalCost = request.Quantity * request.UnitCost,
                BalanceQuantity = balance.QuantityOnHand,
                BalanceValue = balance.QuantityOnHand * balance.AverageCost,
                BatchNumber = request.BatchNumber,
                SerialNumber = request.SerialNumber,
                ExpiryDate = request.ExpiryDate,
                Notes = request.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            Console.WriteLine($"   [INVENTORY] Adding InventoryTransaction...");
            await _transactionRepo.AddAsync(transaction);
            
            Console.WriteLine($"   [INVENTORY] ? Transaction created, Id: {transaction.Id}");
            Console.WriteLine($"   [INVENTORY] AddStockAsync completed successfully");

            return Result<int>.Ok(transaction.Id, $"Stock added successfully. Balance: {balance.QuantityOnHand:N2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   [INVENTORY] ? EXCEPTION: {ex.Message}");
            Console.WriteLine($"   [INVENTORY] Stack: {ex.StackTrace}");
            return Result<int>.Fail($"Error adding stock: {ex.Message}");
        }
    }

    public async Task<Result<int>> DeductStockAsync(DeductStockRequest request, int userId)
    {
        try
        {
            // Validation
            var validationResult = await ValidateStockAvailabilityAsync(
                request.ItemId, 
                request.WarehouseId, 
                request.Quantity, 
                request.BatchNumber);
            
            if (!validationResult.Success)
                return Result<int>.Fail(validationResult.Message);

            // Get Stock Balance
            var balance = await _balanceRepo.GetByItemAndWarehouseAsync(
                request.ItemId, 
                request.WarehouseId, 
                request.BatchNumber, 
                request.SerialNumber);

            if (balance == null)
                return Result<int>.Fail("Stock balance not found");

            // Check sufficient quantity
            if (balance.QuantityAvailable < request.Quantity)
                return Result<int>.Fail($"Insufficient stock. Available: {balance.QuantityAvailable:N2}, Required: {request.Quantity:N2}");

            // Deduct quantity
            balance.QuantityOnHand -= request.Quantity;
            balance.LastUpdated = DateTime.UtcNow;

            await _balanceRepo.UpdateAsync(balance);

            // Create Transaction Record (Negative quantity)
            var transactionNumber = await _transactionRepo.GenerateTransactionNumberAsync();
            var transaction = new InventoryTransaction
            {
                TransactionNumber = transactionNumber,
                TransactionDate = DateTime.UtcNow,
                ItemId = request.ItemId,
                WarehouseId = request.WarehouseId,
                TransactionType = request.TransactionType,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId,
                ReferenceNumber = request.ReferenceNumber,
                Quantity = -request.Quantity, // Negative
                UnitOfMeasure = request.UnitOfMeasure,
                UnitCost = balance.AverageCost,
                TotalCost = -request.Quantity * balance.AverageCost,
                BalanceQuantity = balance.QuantityOnHand,
                BalanceValue = balance.QuantityOnHand * balance.AverageCost,
                BatchNumber = request.BatchNumber,
                SerialNumber = request.SerialNumber,
                Notes = request.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepo.AddAsync(transaction);

            return Result<int>.Ok(transaction.Id, $"Stock deducted successfully. Balance: {balance.QuantityOnHand:N2}");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error deducting stock: {ex.Message}");
        }
    }

    public async Task<Result<int>> AdjustStockAsync(AdjustStockRequest request, int userId)
    {
        try
        {
            // Validation
            var validationResult = await ValidateStockRequest(request.ItemId, request.WarehouseId, Math.Abs(request.AdjustmentQuantity));
            if (!validationResult.Success)
                return Result<int>.Fail(validationResult.Message);

            // Get or Create Stock Balance
            var balance = await _balanceRepo.GetByItemAndWarehouseAsync(
                request.ItemId, 
                request.WarehouseId, 
                request.BatchNumber);

            if (balance == null && request.AdjustmentQuantity < 0)
                return Result<int>.Fail("Cannot adjust down non-existent stock");

            if (balance == null)
            {
                balance = new StockBalance
                {
                    ItemId = request.ItemId,
                    WarehouseId = request.WarehouseId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0,
                    AverageCost = request.NewUnitCost,
                    BatchNumber = request.BatchNumber,
                    LastUpdated = DateTime.UtcNow
                };
                await _balanceRepo.AddAsync(balance);
                
                balance = await _balanceRepo.GetByItemAndWarehouseAsync(request.ItemId, request.WarehouseId, request.BatchNumber);
            }

            // Check if adjustment would result in negative stock
            var newQuantity = balance!.QuantityOnHand + request.AdjustmentQuantity;
            if (newQuantity < 0)
                return Result<int>.Fail($"Adjustment would result in negative stock. Current: {balance.QuantityOnHand:N2}, Adjustment: {request.AdjustmentQuantity:N2}");

            // Update balance
            balance.QuantityOnHand = newQuantity;
            
            // Update cost if positive adjustment
            if (request.AdjustmentQuantity > 0)
            {
                var oldTotalValue = (balance.QuantityOnHand - request.AdjustmentQuantity) * balance.AverageCost;
                var newTotalValue = request.AdjustmentQuantity * request.NewUnitCost;
                balance.AverageCost = balance.QuantityOnHand > 0 
                    ? (oldTotalValue + newTotalValue) / balance.QuantityOnHand 
                    : request.NewUnitCost;
            }
            
            balance.LastUpdated = DateTime.UtcNow;

            await _balanceRepo.UpdateAsync(balance);

            // Create Transaction Record
            var transactionNumber = await _transactionRepo.GenerateTransactionNumberAsync();
            var transaction = new InventoryTransaction
            {
                TransactionNumber = transactionNumber,
                TransactionDate = DateTime.UtcNow,
                ItemId = request.ItemId,
                WarehouseId = request.WarehouseId,
                TransactionType = "Adjustment",
                ReferenceType = "ADJ",
                Quantity = request.AdjustmentQuantity,
                UnitOfMeasure = request.UnitOfMeasure,
                UnitCost = request.NewUnitCost,
                TotalCost = request.AdjustmentQuantity * request.NewUnitCost,
                BalanceQuantity = balance.QuantityOnHand,
                BalanceValue = balance.QuantityOnHand * balance.AverageCost,
                BatchNumber = request.BatchNumber,
                Notes = $"Adjustment: {request.Reason}. {request.Notes}",
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepo.AddAsync(transaction);

            return Result<int>.Ok(transaction.Id, $"Stock adjusted successfully. New balance: {balance.QuantityOnHand:N2}");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error adjusting stock: {ex.Message}");
        }
    }

    public async Task<Result<int>> TransferStockAsync(TransferStockRequest request, int userId)
    {
        try
        {
            // Validation
            if (request.FromWarehouseId == request.ToWarehouseId)
                return Result<int>.Fail("Cannot transfer to the same warehouse");

            var validationResult = await ValidateStockAvailabilityAsync(
                request.ItemId, 
                request.FromWarehouseId, 
                request.Quantity, 
                request.BatchNumber);
            
            if (!validationResult.Success)
                return Result<int>.Fail(validationResult.Message);

            // Deduct from source warehouse
            var deductRequest = new DeductStockRequest
            {
                ItemId = request.ItemId,
                WarehouseId = request.FromWarehouseId,
                Quantity = request.Quantity,
                UnitOfMeasure = request.UnitOfMeasure,
                TransactionType = "Transfer",
                ReferenceType = "TRF",
                BatchNumber = request.BatchNumber,
                Notes = $"Transfer to Warehouse {request.ToWarehouseId}. {request.Notes}"
            };

            var deductResult = await DeductStockAsync(deductRequest, userId);
            if (!deductResult.Success)
                return Result<int>.Fail($"Transfer failed: {deductResult.Message}");

            // Get cost from source
            var sourceBalance = await _balanceRepo.GetByItemAndWarehouseAsync(request.ItemId, request.FromWarehouseId, request.BatchNumber);
            var transferCost = sourceBalance?.AverageCost ?? 0;

            // Add to destination warehouse
            var addRequest = new AddStockRequest
            {
                ItemId = request.ItemId,
                WarehouseId = request.ToWarehouseId,
                Quantity = request.Quantity,
                UnitOfMeasure = request.UnitOfMeasure,
                UnitCost = transferCost,
                TransactionType = "Transfer",
                ReferenceType = "TRF",
                ReferenceId = deductResult.Data,
                BatchNumber = request.BatchNumber,
                Notes = $"Transfer from Warehouse {request.FromWarehouseId}. {request.Notes}"
            };

            var addResult = await AddStockAsync(addRequest, userId);
            if (!addResult.Success)
                return Result<int>.Fail($"Transfer failed at destination: {addResult.Message}");

            return Result<int>.Ok(addResult.Data, $"Stock transferred successfully. Quantity: {request.Quantity:N2}");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail($"Error transferring stock: {ex.Message}");
        }
    }

    #endregion

    #region Validation

    public async Task<Result> ValidateStockAvailabilityAsync(int itemId, int warehouseId, decimal requiredQuantity, string? batchNumber = null)
    {
        try
        {
            var availableQty = await _balanceRepo.GetAvailableQuantityAsync(itemId, warehouseId, batchNumber);
            
            if (availableQty < requiredQuantity)
            {
                var item = await _itemRepo.GetByIdAsync(itemId);
                var warehouse = await _warehouseRepo.GetByIdAsync(warehouseId);
                
                return Result.Fail(
                    $"Insufficient stock for {item?.Code ?? "Item"}. " +
                    $"Available: {availableQty:N2}, Required: {requiredQuantity:N2} " +
                    $"at {warehouse?.Code ?? "Warehouse"}");
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error validating stock: {ex.Message}");
        }
    }

    private async Task<Result> ValidateStockRequest(int itemId, int warehouseId, decimal quantity)
    {
        if (quantity <= 0)
            return Result.Fail("Quantity must be greater than zero");

        var item = await _itemRepo.GetByIdAsync(itemId);
        if (item == null)
            return Result.Fail("Item not found");

        var warehouse = await _warehouseRepo.GetByIdAsync(warehouseId);
        if (warehouse == null)
            return Result.Fail("Warehouse not found");

        if (!warehouse.IsActive)
            return Result.Fail("Warehouse is not active");

        return Result.Ok();
    }

    #endregion

    #region Mapping

    private StockBalanceDto MapToStockBalanceDto(StockBalance balance)
    {
        return new StockBalanceDto
        {
            Id = balance.Id,
            ItemId = balance.ItemId,
            ItemCode = balance.Item?.Code ?? string.Empty,
            ItemName = balance.Item?.Name ?? string.Empty,
            WarehouseId = balance.WarehouseId,
            WarehouseCode = balance.Warehouse?.Code ?? string.Empty,
            WarehouseName = balance.Warehouse?.Name ?? string.Empty,
            QuantityOnHand = balance.QuantityOnHand,
            QuantityReserved = balance.QuantityReserved,
            QuantityAvailable = balance.QuantityAvailable,
            AverageCost = balance.AverageCost,
            TotalValue = balance.TotalValue,
            BatchNumber = balance.BatchNumber,
            SerialNumber = balance.SerialNumber,
            ExpiryDate = balance.ExpiryDate,
            LastUpdated = balance.LastUpdated
        };
    }

    private InventoryTransactionDto MapToTransactionDto(InventoryTransaction transaction)
    {
        return new InventoryTransactionDto
        {
            Id = transaction.Id,
            TransactionNumber = transaction.TransactionNumber,
            TransactionDate = transaction.TransactionDate,
            ItemId = transaction.ItemId,
            ItemCode = transaction.Item?.Code ?? string.Empty,
            ItemName = transaction.Item?.Name ?? string.Empty,
            WarehouseId = transaction.WarehouseId,
            WarehouseCode = transaction.Warehouse?.Code ?? string.Empty,
            WarehouseName = transaction.Warehouse?.Name ?? string.Empty,
            TransactionType = transaction.TransactionType,
            ReferenceType = transaction.ReferenceType,
            ReferenceId = transaction.ReferenceId,
            ReferenceNumber = transaction.ReferenceNumber,
            Quantity = transaction.Quantity,
            UnitOfMeasure = transaction.UnitOfMeasure,
            UnitCost = transaction.UnitCost,
            TotalCost = transaction.TotalCost,
            BalanceQuantity = transaction.BalanceQuantity,
            BalanceValue = transaction.BalanceValue,
            BatchNumber = transaction.BatchNumber,
            SerialNumber = transaction.SerialNumber,
            Notes = transaction.Notes,
            CreatedAt = transaction.CreatedAt
        };
    }

    #endregion
}
