namespace EbikeRental.Domain.Entities;

public class ProductionOrderProcesses
{
    public int Id { get; set; }
    public int ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    
    // Reference to BOM Process
    public int? BomProcessId { get; set; }
    
    public int Sequence { get; set; }
    public string WorkCode { get; set; } = string.Empty;
    public string WorkName { get; set; } = string.Empty;
    public decimal NumberOfPersons { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    // Status: Pending, InProgress, Completed
    public string Status { get; set; } = "Pending";
    
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}
