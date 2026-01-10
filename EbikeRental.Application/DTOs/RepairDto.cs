using EbikeRental.Domain.Enums;

namespace EbikeRental.Application.DTOs;

public class RepairDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RepairStatus Status { get; set; }
    public int? AssignedTechnicianId { get; set; }
    public string? AssignedTechnicianName { get; set; }
    public DateTime RequestedDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public string? RepairNotes { get; set; }
}
