namespace EbikeRental.Domain.Entities;

/// <summary>
/// Inventory Transaction - ?????????????????????????????????
/// </summary>
public class InventoryTransaction
{
    public int Id { get; set; }
    
    // Transaction Info
    public string TransactionNumber { get; set; } = string.Empty; // Auto-generated: IT-YYYY-####
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    // Item & Location
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    
    // Transaction Type
    public string TransactionType { get; set; } = string.Empty; 
    // Values: "GoodsReceipt", "MaterialIssue", "ProductionReceipt", "Adjustment", "Transfer", "Return"
    
    // Reference to source document
    public string? ReferenceType { get; set; } // "GR", "MI", "PO", "ADJ", "TRF"
    public int? ReferenceId { get; set; } // ID of source document
    public string? ReferenceNumber { get; set; } // Document number for display
    
    // Quantity Movement
    public decimal Quantity { get; set; } // Positive = In, Negative = Out
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Costing (for inventory valuation)
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; } // Quantity * UnitCost
    
    // Running Balance (after this transaction)
    public decimal BalanceQuantity { get; set; }
    public decimal BalanceValue { get; set; }
    
    // Batch/Serial Tracking (optional)
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    // Additional Info
    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
