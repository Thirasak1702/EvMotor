using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class BomItemConfig : IEntityTypeConfiguration<BomItem>
{
    public void Configure(EntityTypeBuilder<BomItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ScrapPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.ComponentItem)
            .WithMany()
            .HasForeignKey(x => x.ComponentItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

