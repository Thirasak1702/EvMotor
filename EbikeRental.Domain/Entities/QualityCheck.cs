using EbikeRental.Domain.Enums;

namespace EbikeRental.Domain.Entities;

public class QualityCheck
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public QualityCheckType CheckType { get; set; }
    public QualityCheckStatus Status { get; set; } = QualityCheckStatus.Pending;
    public DateTime InspectionDate { get; set; } = DateTime.UtcNow;
    public string InspectedBy { get; set; } = string.Empty;
    
    // Reference to source document
    public int? GoodsReceiptId { get; set; }
    public GoodsReceipt? GoodsReceipt { get; set; }
    
    public int? ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    
    // Inspection details
    public string? DefectDescription { get; set; }
    public string? CorrectionAction { get; set; }
    public string? Remarks { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<QualityCheckItem> Items { get; set; } = new List<QualityCheckItem>();
}
