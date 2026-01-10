using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class ProductionOrderConfig : IEntityTypeConfiguration<ProductionOrder>
{
    public void Configure(EntityTypeBuilder<ProductionOrder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.OrderNumber).IsUnique();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        // builder.Property(x => x.QCStatus) - Ignored, column not in database yet
        //     .HasMaxLength(20);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.BomCode)
            .HasMaxLength(50);

        builder.Property(x => x.BomName)
            .HasMaxLength(500);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BillOfMaterial)
            .WithMany()
            .HasForeignKey(x => x.BillOfMaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.ProductionOrder)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CreatedByUserId).IsRequired();

        // Ignore properties that don't exist in database yet
        builder.Ignore(x => x.CompletedQuantity);
        builder.Ignore(x => x.RejectedQuantity);
        builder.Ignore(x => x.InProcessQCId);
        builder.Ignore(x => x.FinalQCId);
        builder.Ignore(x => x.QCStatus);
    }
}

