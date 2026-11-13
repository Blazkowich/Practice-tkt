using EPM.TKT.Infrastructure.Entities.Sec;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Sec;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role", Constants.Sec);

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();
    }
}
