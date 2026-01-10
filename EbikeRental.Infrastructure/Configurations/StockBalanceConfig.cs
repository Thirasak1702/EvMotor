using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class StockBalanceConfig : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.ToTable("StockBalances");
        
        builder.HasKey(x => x.Id);
        
        // Quantities
        builder.Property(x => x.QuantityOnHand)
            .HasPrecision(18, 4);
        
        builder.Property(x => x.QuantityReserved)
            .HasPrecision(18, 4);
        
        // Costing
        builder.Property(x => x.AverageCost)
            .HasPrecision(18, 4);
        
        // Batch/Serial
        builder.Property(x => x.BatchNumber)
            .HasMaxLength(50);
        
        builder.Property(x => x.SerialNumber)
            .HasMaxLength(100);
        
        // Relationships
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Unique constraint: One balance per Item + Warehouse (+ optional Batch/Serial)
        builder.HasIndex(x => new { x.ItemId, x.WarehouseId, x.BatchNumber, x.SerialNumber })
            .IsUnique();
        
        // Additional indexes
        builder.HasIndex(x => x.ItemId);
        builder.HasIndex(x => x.WarehouseId);
    }
}
