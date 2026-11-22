using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Sec;

internal static class RoleMappingExtensions
{
    public static IQueryable<RoleModel> MapToDomainModel(this IQueryable<Role> query)
        => query.Select(MapToDomainExpression);

    public static RoleModel MapToDomainModel(this Role r) => new()
    {
        RoleId = r.RoleId,
        Name = r.Name
    };

    private static readonly Expression<Func<Role, RoleModel>> MapToDomainExpression =
        r => new RoleModel
        {
            RoleId = r.RoleId,
            Name = r.Name
        };
}
