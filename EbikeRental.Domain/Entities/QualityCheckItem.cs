using EbikeRental.Domain.Enums;

namespace EbikeRental.Domain.Entities;

public class QualityCheckItem
{
    public int Id { get; set; }
    public int QualityCheckId { get; set; }
    public QualityCheck? QualityCheck { get; set; }
    
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    
    public decimal InspectedQuantity { get; set; }
    public decimal PassedQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    
    public QualityCheckStatus ItemStatus { get; set; } = QualityCheckStatus.Pending;
    public string? DefectDetails { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}
