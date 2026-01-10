using EbikeRental.Domain.Entities;
using EbikeRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EbikeRental.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<RentalContract> RentalContracts { get; set; }
    public DbSet<RepairOrder> RepairOrders { get; set; }
    public DbSet<ProductionOrder> ProductionOrders { get; set; }
    public DbSet<ProductionOrderItem> ProductionOrderItems { get; set; }
    public DbSet<ProductionOrderProcesses> ProductionOrderProcesses { get; set; }
    public DbSet<ProductionOrderQcs> ProductionOrderQcs { get; set; }
    public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; }
    public DbSet<PurchaseRequisitionItem> PurchaseRequisitionItems { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
    public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }
    public DbSet<BillOfMaterial> BillOfMaterials { get; set; }
    public DbSet<BomItem> BomItems { get; set; }
    public DbSet<BomProcess> BomProcesses { get; set; }
    public DbSet<BomQc> BomQcs { get; set; }
    public DbSet<MaterialIssue> MaterialIssues { get; set; }
    public DbSet<MaterialIssueItem> MaterialIssueItems { get; set; }
    public DbSet<QualityCheck> QualityChecks { get; set; }
    public DbSet<QualityCheckItem> QualityCheckItems { get; set; }
    
    // Production Receipt (Finished Goods Receipt)
    public DbSet<ProductionReceipt> ProductionReceipts { get; set; }
    public DbSet<ProductionReceiptItem> ProductionReceiptItems { get; set; }
    
    // Inventory Management
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<StockBalance> StockBalances { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
