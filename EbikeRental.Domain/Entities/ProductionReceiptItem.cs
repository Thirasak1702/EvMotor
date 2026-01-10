namespace EbikeRental.Domain.Entities;

/// <summary>
/// Production Receipt Item - ???????????????????????????????????
/// </summary>
public class ProductionReceiptItem
{
    public int Id { get; set; }
    public int ProductionReceiptId { get; set; }
    public ProductionReceipt? ProductionReceipt { get; set; }
    
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    
    public decimal PlannedQuantity { get; set; }  // ??????????????????
    public decimal ReceivedQuantity { get; set; } // ???????????????
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Batch, Serial, Expiry Tracking for Finished Goods
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? SerialNumber { get; set; }
    
    public string? Notes { get; set; }
}
