namespace EbikeRental.Application.DTOs;

public class MaterialIssueDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    
    public int? ProductionOrderId { get; set; }
    public string? ProductionOrderNumber { get; set; }
    
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    
    public string IssuedBy { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public string? Notes { get; set; }
    
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<MaterialIssueItemDto> Items { get; set; } = new();
}

public class MaterialIssueItemDto
{
    public int Id { get; set; }
    public int MaterialIssueId { get; set; }
    public int ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    
    // Reference to ProductionOrderItem (optional)
    public int? ProductionOrderItemId { get; set; }
    
    public decimal RequiredQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Phase 1: Batch, Serial, Expiry Tracking
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }      // NEW: Expiry Date tracking
    public string? SerialNumber { get; set; }      // NEW: Serial Number tracking
    
    public string? Notes { get; set; }
}
