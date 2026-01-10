using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class MaterialIssueConfig : IEntityTypeConfiguration<MaterialIssue>
{
    public void Configure(EntityTypeBuilder<MaterialIssue> builder)
    {
        builder.HasKey(mi => mi.Id);

        builder.Property(mi => mi.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(mi => mi.IssuedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mi => mi.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(mi => mi.ProductionOrder)
            .WithMany()
            .HasForeignKey(mi => mi.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mi => mi.Warehouse)
            .WithMany()
            .HasForeignKey(mi => mi.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(mi => mi.Items)
            .WithOne(i => i.MaterialIssue)
            .HasForeignKey(i => i.MaterialIssueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MaterialIssueItemConfig : IEntityTypeConfiguration<MaterialIssueItem>
{
    public void Configure(EntityTypeBuilder<MaterialIssueItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.RequiredQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.IssuedQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.BatchNumber)
            .HasMaxLength(50);

        builder.HasOne(i => i.Item)
            .WithMany()
            .HasForeignKey(i => i.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore linking to ProductionOrderItem because the column does not exist in the current DB
        builder.Ignore(i => i.ProductionOrderItemId);
        builder.Ignore(i => i.ProductionOrderItem);
    }
}
