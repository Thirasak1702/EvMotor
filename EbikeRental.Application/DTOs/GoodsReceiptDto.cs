namespace EbikeRental.Application.DTOs;

public class GoodsReceiptDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public int? PurchaseOrderId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, Posted, Cancelled
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Items
    public List<GoodsReceiptItemDto> Items { get; set; } = new List<GoodsReceiptItemDto>();
}

public class GoodsReceiptItemDto
{
    public int Id { get; set; }
    public int GoodsReceiptId { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public bool IsAccepted { get; set; } = true;
    
    // ? Phase 1: Barcode & Serial Number Tracking
    public string? Barcode { get; set; }        // ? NEW: Barcode per GR line
    public string? SerialNumber { get; set; }   // ? NEW: Serial Number per GR line
}
