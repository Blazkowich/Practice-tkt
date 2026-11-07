using System.Linq.Expressions;
using EPM.TKT.App.Models.Auth;
using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Sec;

internal static class UserMappingExtensions
{
    public static IQueryable<UserAuthInfo> MapToAuthInfo(
        this IQueryable<User> query, IQueryable<UserRole> userRoles, IQueryable<Role> roles)
        => query.Select(u => new UserAuthInfo
        {
            UserId       = u.UserId,
            Email        = u.Email,
            PasswordHash = u.PasswordHash,
            IsActive     = u.IsActive,
            Roles        = userRoles
                .Where(ur => ur.UserId == u.UserId)
                .Join(roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
                .ToList()
        });

    public static UserAuthInfo MapToAuthInfo(this User u, IEnumerable<string> roleNames) => new()
    {
        UserId       = u.UserId,
        Email        = u.Email,
        PasswordHash = u.PasswordHash,
        IsActive     = u.IsActive,
        Roles        = roleNames.ToList()
    };
}
