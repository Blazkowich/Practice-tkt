using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class EventSeatConfiguration : IEntityTypeConfiguration<EventSeat>
{
    public void Configure(EntityTypeBuilder<EventSeat> builder)
    {
        builder.ToTable("EventSeat", Constants.Tkt);

        builder.HasKey(es => es.EventSeatId);

        builder.Property(es => es.CreatedAt).IsRequired();

        builder.HasOne(es => es.Event)
               .WithMany()
               .HasForeignKey(es => es.EventId);

        builder.HasOne(es => es.VenueSeat)
               .WithMany()
               .HasForeignKey(es => es.VenueSeatId);

        builder.HasOne(es => es.Price)
               .WithMany()
               .HasForeignKey(es => es.PriceId);

        builder.HasOne(es => es.SeatState)
               .WithMany()
               .HasForeignKey(es => es.SeatStateId);
    }
}
