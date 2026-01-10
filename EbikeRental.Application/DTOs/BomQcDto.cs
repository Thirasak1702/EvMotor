namespace EbikeRental.Application.DTOs;

public class BomQcDto
{
    public int Id { get; set; }
    public int BillOfMaterialId { get; set; }
    public int Sequence { get; set; }
    public string QcCode { get; set; } = string.Empty;
    public string QcName { get; set; } = string.Empty;
    public string QcValues { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

