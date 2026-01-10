namespace EbikeRental.Application.DTOs;

public class PurchaseRequisitionDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string RequestorName { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, Pending, Approved, Rejected
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Items
    public List<PurchaseRequisitionItemDto> Items { get; set; } = new List<PurchaseRequisitionItemDto>();
}

public class PurchaseRequisitionItemDto
{
    public int Id { get; set; }
    public int PurchaseRequisitionId { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal EstimatedUnitPrice { get; set; }
    public decimal TotalAmount => Quantity * EstimatedUnitPrice;
    public DateTime RequiredDate { get; set; }
    public string? Notes { get; set; }
}
