namespace EbikeRental.Application.DTOs;

public class ProductionOrderItemDto
{
    public int Id { get; set; }
    public int ProductionOrderId { get; set; }
    
    // Reference to BOM Item
    public int? BomItemId { get; set; }
    
    // Item Details
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    
    public int Sequence { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Reference quantity from BOM (for calculation)
    public decimal BomQuantity { get; set; }
    
    public string? Notes { get; set; }
}

