using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class RowConfiguration : IEntityTypeConfiguration<Row>
{
    public void Configure(EntityTypeBuilder<Row> builder)
    {
        builder.ToTable("Row", Constants.Tkt);

        builder.HasKey(r => r.RowId);

        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();
        builder.Property(r => r.SortOrder).IsRequired();

        builder.HasOne(r => r.Section)
               .WithMany()
               .HasForeignKey(r => r.SectionId);
    }
}
