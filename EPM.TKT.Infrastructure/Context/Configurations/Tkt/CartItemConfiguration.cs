using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItem", Constants.Tkt);

        builder.HasKey(ci => ci.CartItemId);

        builder.Property(ci => ci.AddedAt).IsRequired();

        builder.HasOne(ci => ci.Cart)
               .WithMany()
               .HasForeignKey(ci => ci.CartId);

        builder.HasOne(ci => ci.EventSeat)
               .WithMany()
               .HasForeignKey(ci => ci.EventSeatId);
    }
}
