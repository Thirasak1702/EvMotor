namespace EbikeRental.Application.DTOs;

public class BomDto
{
    public int Id { get; set; }
    public string BomCode { get; set; } = string.Empty;
    public int ParentItemId { get; set; }
    public string ParentItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public List<BomItemDto> BomItems { get; set; } = new List<BomItemDto>();
    public List<BomProcessDto> BomProcesses { get; set; } = new List<BomProcessDto>();
    public List<BomQcDto> BomQcs { get; set; } = new List<BomQcDto>();
}

public class BomItemDto
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public int ComponentItemId { get; set; }
    public string ComponentItemCode { get; set; } = string.Empty;
    public string ComponentItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal ScrapPercentage { get; set; }
    public int Sequence { get; set; }
    public string? Notes { get; set; }
}
