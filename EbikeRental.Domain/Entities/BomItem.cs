namespace EbikeRental.Domain.Entities;

public class BomItem
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public BillOfMaterial? BillOfMaterial { get; set; }
    public int ComponentItemId { get; set; }
    public Item? ComponentItem { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal ScrapPercentage { get; set; } = 0;
    public int Sequence { get; set; }
    public string? Notes { get; set; }
}
