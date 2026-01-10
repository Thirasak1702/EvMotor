namespace EbikeRental.Application.Common.Filters;

public class AssetFilterParameters : FilterParameters
{
    public string? AssetCode { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
}
