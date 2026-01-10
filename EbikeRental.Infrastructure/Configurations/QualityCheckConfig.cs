using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class QualityCheckConfig : IEntityTypeConfiguration<QualityCheck>
{
    public void Configure(EntityTypeBuilder<QualityCheck> builder)
    {
        builder.HasKey(qc => qc.Id);

        builder.Property(qc => qc.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(qc => qc.InspectedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(qc => qc.ApprovedBy)
            .HasMaxLength(100);

        builder.HasOne(qc => qc.GoodsReceipt)
            .WithMany()
            .HasForeignKey(qc => qc.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(qc => qc.ProductionOrder)
            .WithMany()
            .HasForeignKey(qc => qc.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(qc => qc.Items)
            .WithOne(i => i.QualityCheck)
            .HasForeignKey(i => i.QualityCheckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class QualityCheckItemConfig : IEntityTypeConfiguration<QualityCheckItem>
{
    public void Configure(EntityTypeBuilder<QualityCheckItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InspectedQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.PassedQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.RejectedQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.BatchNumber)
            .HasMaxLength(50);

        builder.HasOne(i => i.Item)
            .WithMany()
            .HasForeignKey(i => i.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
