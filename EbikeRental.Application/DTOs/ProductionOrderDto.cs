namespace EbikeRental.Application.DTOs;

public class ProductionOrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Released, InProgress, Completed, Cancelled
    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // BOM Reference
    public int? BillOfMaterialId { get; set; }
    public string? BomCode { get; set; }
    public string? BomName { get; set; }
    
    // Progress tracking
    public int CompletedQuantity { get; set; }
    public decimal ProgressPercentage => Quantity > 0 ? (decimal)CompletedQuantity / Quantity * 100 : 0;
    
    // Navigation
    public List<ProductionOrderItemDto> Items { get; set; } = new List<ProductionOrderItemDto>();
    public List<ProductionOrderProcessesDto> Processes { get; set; } = new List<ProductionOrderProcessesDto>();
    public List<ProductionOrderQcsDto> QcSteps { get; set; } = new List<ProductionOrderQcsDto>();
}
