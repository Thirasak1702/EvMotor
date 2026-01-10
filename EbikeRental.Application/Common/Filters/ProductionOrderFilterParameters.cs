namespace EbikeRental.Application.Common.Filters;

public class ProductionOrderFilterParameters : FilterParameters
{
    public string? OrderNumber { get; set; }
    public string? ItemCode { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
}
