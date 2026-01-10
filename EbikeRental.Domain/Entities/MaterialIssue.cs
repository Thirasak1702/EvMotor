namespace EbikeRental.Domain.Entities;

public class MaterialIssue
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    
    // Reference to production order
    public int? ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    
    public string IssuedBy { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, Posted, Cancelled
    public string? Notes { get; set; }
    
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<MaterialIssueItem> Items { get; set; } = new List<MaterialIssueItem>();
}
