namespace EbikeRental.Application.DTOs;

public class ProductionOrderQcsDto
{
    public int Id { get; set; }
    public int ProductionOrderId { get; set; }
    public int? BomQcId { get; set; }
    public int Sequence { get; set; }
    public string QcCode { get; set; } = string.Empty;
    public string QcName { get; set; } = string.Empty;
    public string QcValues { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Passed, Failed
    public DateTime? CheckedAt { get; set; }
    public int? CheckedByUserId { get; set; }
    public string? CheckedByUserName { get; set; }
    public string? ActualValues { get; set; }
    public string? Notes { get; set; }
}
