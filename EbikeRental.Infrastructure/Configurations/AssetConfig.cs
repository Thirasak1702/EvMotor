using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.AssetCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.AssetCode).IsUnique();
        builder.HasIndex(x => x.SerialNumber).IsUnique();

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentWarehouse)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.CurrentWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
