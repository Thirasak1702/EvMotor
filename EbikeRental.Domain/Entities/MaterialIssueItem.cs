namespace EbikeRental.Domain.Entities;

public class MaterialIssueItem
{
    public int Id { get; set; }
    public int MaterialIssueId { get; set; }
    public MaterialIssue? MaterialIssue { get; set; }
    
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    
    // Reference to ProductionOrderItem (optional)
    public int? ProductionOrderItemId { get; set; }
    public ProductionOrderItem? ProductionOrderItem { get; set; }
    
    public decimal RequiredQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Phase 1: Batch, Serial, Expiry Tracking
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }      // NEW: Expiry Date tracking
    public string? SerialNumber { get; set; }      // NEW: Serial Number tracking
    
    public string? Notes { get; set; }
}
