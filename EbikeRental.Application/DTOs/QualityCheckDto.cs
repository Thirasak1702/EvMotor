using EbikeRental.Domain.Enums;

namespace EbikeRental.Application.DTOs;

public class QualityCheckDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public QualityCheckType CheckType { get; set; }
    public string CheckTypeName => CheckType.ToString();
    public QualityCheckStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime InspectionDate { get; set; }
    public string InspectedBy { get; set; } = string.Empty;
    
    public int? GoodsReceiptId { get; set; }
    public string? GoodsReceiptNumber { get; set; }
    
    public int? ProductionOrderId { get; set; }
    public string? ProductionOrderNumber { get; set; }
    
    public string? DefectDescription { get; set; }
    public string? CorrectionAction { get; set; }
    public string? Remarks { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<QualityCheckItemDto> Items { get; set; } = new();
}

public class QualityCheckItemDto
{
    public int Id { get; set; }
    public int QualityCheckId { get; set; }
    public int ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public decimal InspectedQuantity { get; set; }
    public decimal PassedQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    public QualityCheckStatus ItemStatus { get; set; }
    public string ItemStatusName => ItemStatus.ToString();
    public string? DefectDetails { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }
}
