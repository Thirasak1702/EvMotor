using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class BillOfMaterialConfig : IEntityTypeConfiguration<BillOfMaterial>
{
    public void Configure(EntityTypeBuilder<BillOfMaterial> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BomCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.BomCode).IsUnique();

        builder.HasOne(x => x.ParentItem)
            .WithMany()
            .HasForeignKey(x => x.ParentItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.BomItems)
            .WithOne(x => x.BillOfMaterial)
            .HasForeignKey(x => x.BillOfMaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BomProcesses)
            .WithOne(x => x.BillOfMaterial)
            .HasForeignKey(x => x.BillOfMaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BomQcs)
            .WithOne(x => x.BillOfMaterial)
            .HasForeignKey(x => x.BillOfMaterialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

