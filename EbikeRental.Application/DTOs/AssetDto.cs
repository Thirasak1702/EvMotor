using EbikeRental.Domain.Enums;

namespace EbikeRental.Application.DTOs;

public class AssetDto
{
    public int Id { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCategory { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public int? CurrentWarehouseId { get; set; }
    public string? CurrentWarehouseName { get; set; }
    public decimal PurchaseCost { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}
