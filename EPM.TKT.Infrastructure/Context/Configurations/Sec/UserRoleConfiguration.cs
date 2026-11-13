using EPM.TKT.Infrastructure.Entities.Sec;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPM.TKT.Infrastructure.Context.Configurations.Sec;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole", Constants.Sec);

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.AssignedAt).IsRequired();

        builder.HasOne(ur => ur.User)
               .WithMany()
               .HasForeignKey(ur => ur.UserId);

        builder.HasOne(ur => ur.Role)
               .WithMany()
               .HasForeignKey(ur => ur.RoleId);
    }
}
