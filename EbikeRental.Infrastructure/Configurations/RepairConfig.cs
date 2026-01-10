using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class RepairConfig : IEntityTypeConfiguration<RepairOrder>
{
    public void Configure(EntityTypeBuilder<RepairOrder> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(x => x.OrderNumber).IsUnique();

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.RepairOrders)
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
