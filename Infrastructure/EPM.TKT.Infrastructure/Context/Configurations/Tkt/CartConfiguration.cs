using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Cart", Constants.Tkt);

        builder.HasKey(c => c.CartId);

        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasOne(c => c.User)
               .WithMany()
               .HasForeignKey(c => c.UserId);

        builder.HasOne(c => c.Event)
               .WithMany()
               .HasForeignKey(c => c.EventId);
    }
}
