namespace EbikeRental.Application.Common.Filters;

public class ItemFilterParameters : FilterParameters
{
    public string? ItemCode { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
}
