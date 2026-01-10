namespace EbikeRental.Application.Common.Filters;

public class WarehouseFilterParameters : FilterParameters
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
}
