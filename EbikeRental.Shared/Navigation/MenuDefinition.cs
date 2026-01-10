using EbikeRental.Shared.Constants;

namespace EbikeRental.Shared.Navigation;

public class MenuItem
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string[]? Roles { get; set; }
    public List<MenuItem>? SubItems { get; set; }
}

public static class MenuDefinition
{
    public static List<MenuItem> GetMenuItems()
    {
        return new List<MenuItem>
        {
            new MenuItem
            {
                Name = "Dashboard",
                Icon = "bi-speedometer2",
                Url = "/Dashboard",
                Roles = Roles.All
            },
            new MenuItem
            {
                Name = "Master Data",
                Icon = "bi-database",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.Purchasing, Roles.Warehouse },
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Items", Url = "/Masters/Items", Icon = "bi-box" },
                    new MenuItem { Name = "BOM", Url = "/Masters/BOM", Icon = "bi-diagram-3" },
                    new MenuItem { Name = "Assets", Url = "/Masters/Assets", Icon = "bi-bicycle" },
                    new MenuItem { Name = "Warehouses", Url = "/Masters/Warehouses", Icon = "bi-building" },
                    new MenuItem { Name = "Users", Url = "/Masters/Users", Icon = "bi-people" }
                }
            },
            new MenuItem
            {
                Name = "Purchasing",
                Icon = "bi-cart",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.Purchasing },
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Purchase Requisitions", Url = "/Purchasing/PR", Icon = "bi-file-text" },
                    new MenuItem { Name = "Purchase Orders", Url = "/Purchasing/PO", Icon = "bi-receipt" },
                    new MenuItem { Name = "Goods Receipt", Url = "/Purchasing/GR", Icon = "bi-inbox" }
                }
            },
            new MenuItem
            {
                Name = "Quality",
                Icon = "bi-shield-lock",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin  , Roles.Purchasing},
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "QC", Url = "/Quality/QC", Icon = "bi-person-badge" },
                }
            },
            new MenuItem
            {
                Name = "Inventory",
                Icon = "bi-boxes",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.Warehouse },
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Stock Overview", Url = "/Inventory/Stock", Icon = "bi-stack" },
                    new MenuItem { Name = "Transfers", Url = "/Inventory/Transfers", Icon = "bi-arrow-left-right" }
                }
            },
            new MenuItem
            {
                Name = "Production",
                Icon = "bi-gear",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.Production },
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Production Orders", Url = "/Production/ProductionOrders", Icon = "bi-clipboard-check" },
                    new MenuItem { Name = "Material Issue", Url = "/Production/MaterialIssue", Icon = "bi-box-arrow-right" },
                    new MenuItem { Name = "Production Receipt", Url = "/Production/ProductionReceipt", Icon = "bi-box-arrow-in-down" }
                }
            },
            new MenuItem
            {
                Name = "Rental",
                Icon = "bi-calendar-check",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.RentalStaff },
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Contracts", Url = "/Rental/Contracts", Icon = "bi-file-earmark-text" },
                    new MenuItem { Name = "Returns", Url = "/Rental/Returns", Icon = "bi-arrow-return-left" }
                }
            },
            new MenuItem
            {
                Name = "Maintenance",
                Icon = "bi-tools",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin, Roles.Technician },
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Repair Requests", Url = "/Maintenance/Requests", Icon = "bi-wrench" },
                    new MenuItem { Name = "Repair Orders", Url = "/Maintenance/RepairOrders", Icon = "bi-hammer" }
                }
            },
            new MenuItem
            {
                Name = "Reports",
                Icon = "bi-graph-up",
                Url = "/Reports",
                Roles = Roles.All
            },
            new MenuItem
            {
                Name = "Administration",
                Icon = "bi-shield-lock",
                Roles = new[] { Roles.SuperAdmin, Roles.Admin },
                SubItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Users", Url = "/Admin/Users", Icon = "bi-person-badge" },
                    new MenuItem { Name = "Roles", Url = "/Admin/Roles", Icon = "bi-key" }
                }
            }
        };
    }
}
