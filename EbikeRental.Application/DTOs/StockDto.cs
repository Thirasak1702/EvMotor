namespace EbikeRental.Application.DTOs;

public class StockDto
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int RentedQuantity { get; set; }
    public int MaintenanceQuantity { get; set; }
}
