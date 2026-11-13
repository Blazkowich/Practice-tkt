using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venue", Constants.Tkt);

        builder.HasKey(v => v.VenueId);

        builder.Property(v => v.Name).HasMaxLength(200).IsRequired();
        builder.Property(v => v.Address).HasMaxLength(500);
        builder.Property(v => v.City).HasMaxLength(100);
        builder.Property(v => v.Country).HasMaxLength(100);
        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.IsActive).IsRequired();
    }
}
