using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class PurchaseRequisitionItemConfig : IEntityTypeConfiguration<PurchaseRequisitionItem>
{
    public void Configure(EntityTypeBuilder<PurchaseRequisitionItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.EstimatedUnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
