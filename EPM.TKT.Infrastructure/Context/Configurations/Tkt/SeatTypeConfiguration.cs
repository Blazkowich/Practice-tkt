using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class SeatTypeConfiguration : IEntityTypeConfiguration<SeatType>
{
    public void Configure(EntityTypeBuilder<SeatType> builder)
    {
        builder.ToTable("SeatType", Constants.Tkt);

        builder.HasKey(st => st.SeatTypeId);

        builder.Property(st => st.Name).HasMaxLength(50).IsRequired();
    }
}
