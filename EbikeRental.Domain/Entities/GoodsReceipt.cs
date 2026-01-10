namespace EbikeRental.Domain.Entities;

public class GoodsReceipt
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public int? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Posted, Cancelled
    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
}
