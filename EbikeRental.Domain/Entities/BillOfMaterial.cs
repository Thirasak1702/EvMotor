namespace EbikeRental.Domain.Entities;

public class BillOfMaterial
{
    public int Id { get; set; }
    public string BomCode { get; set; } = string.Empty;
    public int ParentItemId { get; set; }
    public Item? ParentItem { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<BomItem> BomItems { get; set; } = new List<BomItem>();
    public ICollection<BomProcess> BomProcesses { get; set; } = new List<BomProcess>();
    public ICollection<BomQc> BomQcs { get; set; } = new List<BomQc>();
}
