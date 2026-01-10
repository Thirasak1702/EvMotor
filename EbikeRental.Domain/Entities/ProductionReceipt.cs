namespace EbikeRental.Domain.Entities;

/// <summary>
/// Production Receipt - ??????????????????????????????? (Finished Goods Receipt)
/// ??????????????????????????????? ????????????????????
/// </summary>
public class ProductionReceipt
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; } = DateTime.Today;
    
    // Reference to production order (optional)
    public int? ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    
    public string ReceivedBy { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, Posted, Cancelled
    public string? Notes { get; set; }
    
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<ProductionReceiptItem> Items { get; set; } = new List<ProductionReceiptItem>();
}
