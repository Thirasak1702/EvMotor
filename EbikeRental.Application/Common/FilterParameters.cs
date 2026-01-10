using EbikeRental.Shared.Models;

namespace EbikeRental.Application.Common;

public class FilterParameters : PagingParameters
{
    public string? SearchTerm { get; set; }
}
