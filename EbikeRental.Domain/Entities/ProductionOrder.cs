namespace EbikeRental.Domain.Entities;

public class ProductionOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int Quantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Released, InProgress, Completed, Cancelled
    public string? Notes { get; set; }
    
    // BOM Reference
    public int? BillOfMaterialId { get; set; }
    public BillOfMaterial? BillOfMaterial { get; set; }
    public string? BomCode { get; set; }
    public string? BomName { get; set; }
    
    // QC Reference
    public int? InProcessQCId { get; set; }
    public int? FinalQCId { get; set; }
    public string? QCStatus { get; set; } // Pending, InProgress, Passed, Failed
    
    // Production tracking
    public int CompletedQuantity { get; set; } = 0;
    public int RejectedQuantity { get; set; } = 0;
    
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<ProductionOrderItem> Items { get; set; } = new List<ProductionOrderItem>();
    public ICollection<ProductionOrderProcesses> Processes { get; set; } = new List<ProductionOrderProcesses>();
    public ICollection<ProductionOrderQcs> QcSteps { get; set; } = new List<ProductionOrderQcs>();
}
