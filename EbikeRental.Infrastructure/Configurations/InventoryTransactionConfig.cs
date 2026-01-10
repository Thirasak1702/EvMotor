using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class InventoryTransactionConfig : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");
        
        builder.HasKey(x => x.Id);
        
        // Transaction Number - Unique
        builder.Property(x => x.TransactionNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(x => x.TransactionNumber)
            .IsUnique();
        
        // Transaction Type
        builder.Property(x => x.TransactionType)
            .IsRequired()
            .HasMaxLength(50);
        
        // Reference
        builder.Property(x => x.ReferenceType)
            .HasMaxLength(10);
        
        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(50);
        
        // Quantity & Costing
        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4);
        
        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 4);
        
        builder.Property(x => x.TotalCost)
            .HasPrecision(18, 4);
        
        builder.Property(x => x.BalanceQuantity)
            .HasPrecision(18, 4);
        
        builder.Property(x => x.BalanceValue)
            .HasPrecision(18, 4);
        
        builder.Property(x => x.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(20);
        
        // Batch/Serial
        builder.Property(x => x.BatchNumber)
            .HasMaxLength(50);
        
        builder.Property(x => x.SerialNumber)
            .HasMaxLength(100);
        
        // Notes
        builder.Property(x => x.Notes)
            .HasMaxLength(500);
        
        // Relationships
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes for performance
        builder.HasIndex(x => x.TransactionDate);
        builder.HasIndex(x => x.ItemId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => new { x.ItemId, x.WarehouseId, x.TransactionDate });
        builder.HasIndex(x => new { x.ReferenceType, x.ReferenceId });
    }
}
