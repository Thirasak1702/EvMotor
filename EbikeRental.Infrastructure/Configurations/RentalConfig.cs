using EbikeRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EbikeRental.Infrastructure.Configurations;

public class RentalConfig : IEntityTypeConfiguration<RentalContract>
{
    public void Configure(EntityTypeBuilder<RentalContract> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ContractNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.ContractNumber).IsUnique();

        builder.HasOne(x => x.Asset)
            .WithMany(x => x.RentalContracts)
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
