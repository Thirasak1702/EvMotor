namespace EbikeRental.Domain.Entities;

public class PurchaseRequisitionItem
{
    public int Id { get; set; }
    public int PurchaseRequisitionId { get; set; }
    public PurchaseRequisition? PurchaseRequisition { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal EstimatedUnitPrice { get; set; }
    public DateTime RequiredDate { get; set; }
    public string? Notes { get; set; }
}
