namespace EbikeRental.Shared.Constants;

public static class Roles
{
    public const string SuperAdmin = "SUPER_ADMIN";
    public const string Admin = "ADMIN";
    public const string Purchasing = "PURCHASING";
    public const string Warehouse = "WAREHOUSE";
    public const string Production = "PRODUCTION";
    public const string RentalStaff = "RENTAL_STAFF";
    public const string Technician = "TECHNICIAN";
    public const string Viewer = "VIEWER";

    public static readonly string[] All = 
    {
        SuperAdmin,
        Admin,
        Purchasing,
        Warehouse,
        Production,
        RentalStaff,
        Technician,
        Viewer
    };
}
