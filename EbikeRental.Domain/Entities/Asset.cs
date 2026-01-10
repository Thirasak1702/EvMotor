using EbikeRental.Domain.Enums;

namespace EbikeRental.Domain.Entities;

public class Asset
{
    public int Id { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public AssetStatus Status { get; set; } = AssetStatus.InStock;
    public int? CurrentWarehouseId { get; set; }
    public Warehouse? CurrentWarehouse { get; set; }
    public decimal PurchaseCost { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? ActivationDate { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
    public ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
}
