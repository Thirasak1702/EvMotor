namespace EbikeRental.Domain.Entities;

public class BomProcess
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public BillOfMaterial? BillOfMaterial { get; set; }
    public int Sequence { get; set; }
    
    // Work/Process (ใช้ Text Fields - ไม่ต้องมี Master)
    public string WorkCode { get; set; } = string.Empty;
    public string WorkName { get; set; } = string.Empty;
    
    // Process Details
    public decimal NumberOfPersons { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    public string? Notes { get; set; }
}

