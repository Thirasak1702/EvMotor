using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class ProductionOrderItemConfig : IEntityTypeConfiguration<ProductionOrderItem>
{
    public void Configure(EntityTypeBuilder<ProductionOrderItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.BomQuantity)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BomItem)
            .WithMany()
            .HasForeignKey(x => x.BomItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ProductionOrderId, x.Sequence }).IsUnique();
    }
}

