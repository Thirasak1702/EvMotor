using EbikeRental.Domain.Enums;

namespace EbikeRental.Domain.Entities;

public class RepairOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public string Description { get; set; } = string.Empty;
    public RepairStatus Status { get; set; } = RepairStatus.Requested;
    public int? AssignedTechnicianId { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public string? RepairNotes { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
