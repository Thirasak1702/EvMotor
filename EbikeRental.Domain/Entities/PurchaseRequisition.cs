namespace EbikeRental.Domain.Entities;

public class PurchaseRequisition
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string RequestorName { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, Pending, Approved, Rejected
    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<PurchaseRequisitionItem> Items { get; set; } = new List<PurchaseRequisitionItem>();
}
