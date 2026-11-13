using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Section", Constants.Tkt);

        builder.HasKey(s => s.SectionId);

        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.SortOrder).IsRequired();

        builder.HasOne(s => s.Venue)
               .WithMany()
               .HasForeignKey(s => s.VenueId);

        builder.HasOne(s => s.SeatType)
               .WithMany()
               .HasForeignKey(s => s.SeatTypeId);
    }
}
