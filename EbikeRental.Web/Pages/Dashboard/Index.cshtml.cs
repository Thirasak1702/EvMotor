using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EbikeRental.Web.Pages.Dashboard;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IAssetRepository _assetRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IRepairRepository _repairRepository;
    private readonly IUserService _userService;

    public IndexModel(
        IAssetRepository assetRepository,
        IRentalRepository rentalRepository,
        IRepairRepository repairRepository,
        IUserService userService)
    {
        _assetRepository = assetRepository;
        _rentalRepository = rentalRepository;
        _repairRepository = repairRepository;
        _userService = userService;
    }

    public int AvailableAssetsCount { get; set; }
    public int ActiveRentalsCount { get; set; }
    public int PendingRepairsCount { get; set; }
    public int TotalUsersCount { get; set; }

    public async Task OnGetAsync()
    {
        var availableAssets = await _assetRepository.GetAvailableAssetsAsync();
        AvailableAssetsCount = availableAssets.Count;

        var activeRentals = await _rentalRepository.GetActiveContractsAsync();
        ActiveRentalsCount = activeRentals.Count;

        var pendingRepairs = await _repairRepository.GetPendingRepairsAsync();
        PendingRepairsCount = pendingRepairs.Count;

        var usersResult = await _userService.GetAllAsync();
        if (usersResult.Success && usersResult.Data != null)
        {
            TotalUsersCount = usersResult.Data.Count;
        }
    }
}
