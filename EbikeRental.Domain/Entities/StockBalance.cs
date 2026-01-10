namespace EbikeRental.Domain.Entities;

/// <summary>
/// Stock Balance - ???????????????????? (Summary view)
/// </summary>
public class StockBalance
{
    public int Id { get; set; }
    
    // Item & Location
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    
    // Current Balance
    public decimal QuantityOnHand { get; set; } // ?????????????
    public decimal QuantityReserved { get; set; } = 0; // ?????? (???????????)
    public decimal QuantityAvailable => QuantityOnHand - QuantityReserved; // ???????????
    
    // Valuation
    public decimal AverageCost { get; set; } // ????????????
    public decimal TotalValue => QuantityOnHand * AverageCost; // ?????????
    
    // Batch/Serial Tracking (optional - for quick lookup)
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    // Audit
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public int LastTransactionId { get; set; } // Link to last transaction
}
