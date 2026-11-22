using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Event", Constants.Tkt);

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.EventDate).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasOne(e => e.Venue)
               .WithMany()
               .HasForeignKey(e => e.VenueId);

        builder.HasOne(e => e.EventStatus)
               .WithMany()
               .HasForeignKey(e => e.EventStatusId);
    }
}
