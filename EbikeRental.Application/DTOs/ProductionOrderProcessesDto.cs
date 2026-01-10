namespace EbikeRental.Application.DTOs;

public class ProductionOrderProcessesDto
{
    public int Id { get; set; }
    public int ProductionOrderId { get; set; }
    public int? BomProcessId { get; set; }
    public int Sequence { get; set; }
    public string WorkCode { get; set; } = string.Empty;
    public string WorkName { get; set; } = string.Empty;
    public decimal NumberOfPersons { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}
