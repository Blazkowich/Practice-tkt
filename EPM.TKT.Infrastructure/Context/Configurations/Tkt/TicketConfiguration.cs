using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Ticket", Constants.Tkt);

        builder.HasKey(t => t.TicketId);

        builder.Property(t => t.PaidAmount).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(t => t.IssuedAt).IsRequired();

        builder.HasOne(t => t.Order)
               .WithMany()
               .HasForeignKey(t => t.OrderId);

        builder.HasOne(t => t.Event)
               .WithMany()
               .HasForeignKey(t => t.EventId);

        builder.HasOne(t => t.EventSeat)
               .WithMany()
               .HasForeignKey(t => t.EventSeatId);

        builder.HasOne(t => t.User)
               .WithMany()
               .HasForeignKey(t => t.UserId);

        builder.HasOne(t => t.Price)
               .WithMany()
               .HasForeignKey(t => t.PriceId);
    }
}
