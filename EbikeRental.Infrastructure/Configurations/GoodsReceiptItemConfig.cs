using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class GoodsReceiptItemConfig : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderedQuantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.ReceivedQuantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
