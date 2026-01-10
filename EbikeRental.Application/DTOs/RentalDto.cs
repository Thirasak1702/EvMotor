using EbikeRental.Domain.Enums;

namespace EbikeRental.Application.DTOs;

public class RentalDto
{
    public int Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
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
    public RentalStatus Status { get; set; }
    public string? Notes { get; set; }
}
