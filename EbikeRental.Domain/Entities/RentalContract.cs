using EbikeRental.Domain.Enums;

namespace EbikeRental.Domain.Entities;

public class RentalContract
{
    public int Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public DateTime RentalStartDate { get; set; }
    public DateTime RentalEndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Draft;
    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
