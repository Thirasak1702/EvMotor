namespace EbikeRental.Domain.Entities;

public class ProductionOrderItem
{
    public int Id { get; set; }
    public int ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    
    // Reference to BOM Item
    public int? BomItemId { get; set; }
    public BomItem? BomItem { get; set; }
    
    // Item Details
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    
    public int Sequence { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Reference quantity from BOM (for calculation)
    public decimal BomQuantity { get; set; }
    
    public string? Notes { get; set; }
    
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

