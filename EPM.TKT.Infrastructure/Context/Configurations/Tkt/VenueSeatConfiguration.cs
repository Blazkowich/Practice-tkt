using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Tkt;

internal sealed class VenueSeatConfiguration : IEntityTypeConfiguration<VenueSeat>
{
    public void Configure(EntityTypeBuilder<VenueSeat> builder)
    {
        builder.ToTable("VenueSeat", Constants.Tkt);

        builder.HasKey(vs => vs.VenueSeatId);

        builder.Property(vs => vs.SeatNumber).HasMaxLength(20);
        builder.Property(vs => vs.SortOrder).IsRequired();

        builder.HasOne(vs => vs.Section)
               .WithMany()
               .HasForeignKey(vs => vs.SectionId);

        builder.HasOne(vs => vs.Row)
               .WithMany()
               .HasForeignKey(vs => vs.RowId);
    }
}
