namespace EbikeRental.Application.Common.Filters;

public class RentalContractFilterParameters : FilterParameters
{
    public string? ContractNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}
