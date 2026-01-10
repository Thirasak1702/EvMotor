namespace EbikeRental.Domain.Entities;

public class PurchaseOrder
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int? PurchaseRequisitionId { get; set; }
    public PurchaseRequisition? PurchaseRequisition { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string VendorContact { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTime ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Sent, Confirmed, PartialReceived, Received, Cancelled
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
