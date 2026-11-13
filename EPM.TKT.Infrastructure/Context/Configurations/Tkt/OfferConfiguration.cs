using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offer", Constants.Tkt);

        builder.HasKey(o => o.OfferId);

        builder.Property(o => o.Name).HasMaxLength(200).IsRequired();
        builder.Property(o => o.SaleStartAt).IsRequired();
        builder.Property(o => o.IsActive).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();

        builder.HasOne(o => o.Event)
               .WithMany()
               .HasForeignKey(o => o.EventId);
    }
}
