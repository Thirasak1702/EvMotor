namespace EbikeRental.Application.DTOs;

public class BomProcessDto
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public int Sequence { get; set; }
    public string WorkCode { get; set; } = string.Empty;
    public string WorkName { get; set; } = string.Empty;
    public decimal NumberOfPersons { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

