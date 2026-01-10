using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class BomQcConfig : IEntityTypeConfiguration<BomQc>
{
    public void Configure(EntityTypeBuilder<BomQc> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.QcCode)
            .HasMaxLength(50);

        builder.Property(x => x.QcName)
            .HasMaxLength(200);

        builder.Property(x => x.QcValues)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.BillOfMaterialId, x.Sequence })
            .IsUnique()
            .HasDatabaseName("IX_BomQcs_BillOfMaterialId_Sequence");
    }
}

