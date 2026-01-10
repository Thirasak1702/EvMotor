using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class BomProcessConfig : IEntityTypeConfiguration<BomProcess>
{
    public void Configure(EntityTypeBuilder<BomProcess> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkCode)
            .HasMaxLength(50);

        builder.Property(x => x.WorkName)
            .HasMaxLength(200);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.NumberOfPersons)
            .HasPrecision(18, 2);

        builder.Property(x => x.UnitOfMeasure)
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.BillOfMaterialId, x.Sequence })
            .IsUnique()
            .HasDatabaseName("IX_BomProcesses_BillOfMaterialId_Sequence");
    }
}

