using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class ProductionOrderProcessesConfig : IEntityTypeConfiguration<ProductionOrderProcesses>
{
    public void Configure(EntityTypeBuilder<ProductionOrderProcesses> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.WorkName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.UnitOfMeasure)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.NumberOfPersons)
            .HasPrecision(18, 2);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.Processes)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ProductionOrderId, x.Sequence });
    }
}
