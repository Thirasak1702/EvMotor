namespace EbikeRental.Application.Common.Filters;

public class RepairOrderFilterParameters : FilterParameters
{
    public string? OrderNumber { get; set; }
    public string? AssetCode { get; set; }
    public string? Status { get; set; }
    public DateTime? RequestDateFrom { get; set; }
    public DateTime? RequestDateTo { get; set; }
}
