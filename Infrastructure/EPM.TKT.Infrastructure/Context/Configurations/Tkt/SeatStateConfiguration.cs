using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class SeatStateConfiguration : IEntityTypeConfiguration<SeatState>
{
    public void Configure(EntityTypeBuilder<SeatState> builder)
    {
        builder.ToTable("SeatState", Constants.Tkt);

        builder.HasKey(ss => ss.SeatStateId);

        builder.Property(ss => ss.Name).HasMaxLength(50).IsRequired();
    }
}
