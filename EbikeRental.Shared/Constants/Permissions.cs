namespace EbikeRental.Shared.Constants;

public static class Permissions
{
    // Master Data
    public const string ViewItems = "Permissions.Items.View";
    public const string CreateItems = "Permissions.Items.Create";
    public const string EditItems = "Permissions.Items.Edit";
    public const string DeleteItems = "Permissions.Items.Delete";

    public const string ViewAssets = "Permissions.Assets.View";
    public const string CreateAssets = "Permissions.Assets.Create";
    public const string EditAssets = "Permissions.Assets.Edit";
    public const string DeleteAssets = "Permissions.Assets.Delete";

    // Purchasing
    public const string ViewPurchasing = "Permissions.Purchasing.View";
    public const string CreatePurchasing = "Permissions.Purchasing.Create";
    public const string EditPurchasing = "Permissions.Purchasing.Edit";
    public const string ApprovePurchasing = "Permissions.Purchasing.Approve";

    // Rental
    public const string ViewRental = "Permissions.Rental.View";
    public const string CreateRental = "Permissions.Rental.Create";
    public const string EditRental = "Permissions.Rental.Edit";
    public const string CompleteRental = "Permissions.Rental.Complete";

    // Maintenance
    public const string ViewMaintenance = "Permissions.Maintenance.View";
    public const string CreateMaintenance = "Permissions.Maintenance.Create";
    public const string AssignMaintenance = "Permissions.Maintenance.Assign";
    public const string CompleteMaintenance = "Permissions.Maintenance.Complete";

    // Administration
    public const string ViewUsers = "Permissions.Users.View";
    public const string CreateUsers = "Permissions.Users.Create";
    public const string EditUsers = "Permissions.Users.Edit";
    public const string DeleteUsers = "Permissions.Users.Delete";
}
