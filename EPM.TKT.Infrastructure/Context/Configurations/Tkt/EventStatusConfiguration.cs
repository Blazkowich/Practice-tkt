using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class EventStatusConfiguration : IEntityTypeConfiguration<EventStatus>
{
    public void Configure(EntityTypeBuilder<EventStatus> builder)
    {
        builder.ToTable("EventStatus", Constants.Tkt);

        builder.HasKey(es => es.EventStatusId);

        builder.Property(es => es.Name).HasMaxLength(50).IsRequired();
    }
}
