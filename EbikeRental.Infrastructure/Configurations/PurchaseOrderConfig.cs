using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class PurchaseOrderConfig : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.DocumentNumber).IsUnique();

        builder.Property(x => x.VendorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.VendorContact)
            .HasMaxLength(100);

        builder.Property(x => x.DeliveryAddress)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.PurchaseRequisition)
            .WithMany()
            .HasForeignKey(x => x.PurchaseRequisitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.PurchaseOrder)
            .HasForeignKey(x => x.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
