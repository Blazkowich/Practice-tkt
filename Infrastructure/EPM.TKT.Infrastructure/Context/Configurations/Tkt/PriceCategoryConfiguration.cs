using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class PriceCategoryConfiguration : IEntityTypeConfiguration<PriceCategory>
{
    public void Configure(EntityTypeBuilder<PriceCategory> builder)
    {
        builder.ToTable("PriceCategory", Constants.Tkt);

        builder.HasKey(pc => pc.PriceCategoryId);

        builder.Property(pc => pc.Name).HasMaxLength(50).IsRequired();
    }
}
