using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class PurchaseRequisitionConfig : IEntityTypeConfiguration<PurchaseRequisition>
{
    public void Configure(EntityTypeBuilder<PurchaseRequisition> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.DocumentNumber).IsUnique();

        builder.Property(x => x.DepartmentName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.RequestorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.PurchaseRequisition)
            .HasForeignKey(x => x.PurchaseRequisitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
