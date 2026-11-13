using EPM.TKT.Infrastructure.Entities.Sec;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Sec;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken", Constants.Sec);

        builder.HasKey(rt => rt.TokenId);

        builder.Property(rt => rt.Token).HasMaxLength(500).IsRequired();
        builder.Property(rt => rt.ExpiresAt).IsRequired();
        builder.Property(rt => rt.IsRevoked).IsRequired();
        builder.Property(rt => rt.CreatedAt).IsRequired();

        builder.HasOne(rt => rt.User)
               .WithMany()
               .HasForeignKey(rt => rt.UserId);
    }
}
