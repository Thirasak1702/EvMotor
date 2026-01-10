namespace EbikeRental.Domain.Entities;

public class BomQc
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public BillOfMaterial? BillOfMaterial { get; set; }
    public int Sequence { get; set; }
    
    // QC Process (ใช้ Text Fields - ไม่ต้องมี Master)
    public string QcCode { get; set; } = string.Empty;
    public string QcName { get; set; } = string.Empty;
    
    // QC Values (ค่าที่ต้องตรวจสอบ)
    public string QcValues { get; set; } = string.Empty;
    
    public string? Notes { get; set; }
}

