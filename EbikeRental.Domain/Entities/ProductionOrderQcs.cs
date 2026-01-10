namespace EbikeRental.Domain.Entities;

public class ProductionOrderQcs
{
    public int Id { get; set; }
    public int ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    
    // Reference to BOM QC
    public int? BomQcId { get; set; }
    
    public int Sequence { get; set; }
    public string QcCode { get; set; } = string.Empty;
    public string QcName { get; set; } = string.Empty;
    public string QcValues { get; set; } = string.Empty;
    
    // Status: Pending, InProgress, Passed, Failed
    public string Status { get; set; } = "Pending";
    
    public DateTime? CheckedAt { get; set; }
    public int? CheckedByUserId { get; set; }
    public string? ActualValues { get; set; }
    public string? Notes { get; set; }
}
