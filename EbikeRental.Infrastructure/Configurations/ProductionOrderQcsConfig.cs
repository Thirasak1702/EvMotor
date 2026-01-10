using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class ProductionOrderQcsConfig : IEntityTypeConfiguration<ProductionOrderQcs>
{
    public void Configure(EntityTypeBuilder<ProductionOrderQcs> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QcCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.QcName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.QcValues)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ActualValues)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.QcSteps)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ProductionOrderId, x.Sequence });
    }
}
