using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order", Constants.Tkt);

        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.TotalAmount).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(o => o.PaymentTransactionId).HasMaxLength(200);
        builder.Property(o => o.CreatedAt).IsRequired();

        builder.HasOne(o => o.User)
               .WithMany()
               .HasForeignKey(o => o.UserId);

        builder.HasOne(o => o.Event)
               .WithMany()
               .HasForeignKey(o => o.EventId);

        builder.HasOne(o => o.Cart)
               .WithMany()
               .HasForeignKey(o => o.CartId);
    }
}
