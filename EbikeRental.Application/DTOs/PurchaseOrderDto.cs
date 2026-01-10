namespace EbikeRental.Application.DTOs;

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int? PurchaseRequisitionId { get; set; }
    public string PurchaseRequisitionNumber { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorContact { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTime ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Sent, Confirmed, PartialReceived, Received, Cancelled
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Items
    public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
}

public class PurchaseOrderItemDto
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal LineTotal => Quantity * UnitPrice * (1 - DiscountPercent / 100) * (1 + TaxPercent / 100);
    public string? Notes { get; set; }
}
