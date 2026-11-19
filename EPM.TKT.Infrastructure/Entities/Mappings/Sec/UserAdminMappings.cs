using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Sec;

internal static class UserAdminMappingExtensions
{
    public static IQueryable<UserModel> MapToUserModel(this IQueryable<User> query)
        => query.Select(MapToUserModelExpression);

    public static UserModel MapToUserModel(this User u) => new()
    {
        UserId    = u.UserId,
        Email     = u.Email,
        FirstName = u.FirstName,
        LastName  = u.LastName,
        CreatedAt = u.CreatedAt,
        IsActive  = u.IsActive
    };

    private static readonly Expression<Func<User, UserModel>> MapToUserModelExpression =
        u => new UserModel
        {
            UserId    = u.UserId,
            Email     = u.Email,
            FirstName = u.FirstName,
            LastName  = u.LastName,
            CreatedAt = u.CreatedAt,
            IsActive  = u.IsActive
        };
}
