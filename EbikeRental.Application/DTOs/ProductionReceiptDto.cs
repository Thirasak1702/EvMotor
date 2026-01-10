namespace EbikeRental.Application.DTOs;

public class ProductionReceiptDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    
    public int? ProductionOrderId { get; set; }
    public string? ProductionOrderNumber { get; set; }
    
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    
    public string ReceivedBy { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, Posted, Cancelled
    public string? Notes { get; set; }
    
    public int CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<ProductionReceiptItemDto> Items { get; set; } = new();
}

public class ProductionReceiptItemDto
{
    public int Id { get; set; }
    public int ProductionReceiptId { get; set; }
    public int ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    
    public decimal PlannedQuantity { get; set; }  // ??????????????
    public decimal ReceivedQuantity { get; set; } // ???????????????
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Batch, Serial, Expiry Tracking for Finished Goods
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? SerialNumber { get; set; }
    
    public string? Notes { get; set; }
}
