using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.ToTable("Price", Constants.Tkt);

        builder.HasKey(p => p.PriceId);

        builder.Property(p => p.Amount).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(p => p.Currency).HasMaxLength(3).HasColumnType("char(3)").IsRequired();

        builder.HasOne(p => p.Offer)
               .WithMany()
               .HasForeignKey(p => p.OfferId);

        builder.HasOne(p => p.Section)
               .WithMany()
               .HasForeignKey(p => p.SectionId);

        builder.HasOne(p => p.Row)
               .WithMany()
               .HasForeignKey(p => p.RowId);

        builder.HasOne(p => p.PriceCategory)
               .WithMany()
               .HasForeignKey(p => p.PriceCategoryId);
    }
}
