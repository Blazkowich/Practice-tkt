using EPM.TKT.App.Models.Auth;
using EPM.TKT.App.Repositories;
using EPM.TKT.Infrastructure.Entities.Mappings.Sec;
using EPM.TKT.Infrastructure.Entities.Sec;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class UserRepository(ITKTDbContext context) : IUserRepository
{
    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<UserAuthInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .MapToAuthInfo(context.UserRoles, context.Roles)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<UserAuthInfo?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .MapToAuthInfo(context.UserRoles, context.Roles)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Guid> CreateUserAsync(string email, string passwordHash, string? firstName, string? lastName, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Email        = email,
            PasswordHash = passwordHash,
            FirstName    = firstName,
            LastName     = lastName,
            CreatedAt    = DateTime.UtcNow,
            IsActive     = true
        };
        context.Users.Add(user);
        context.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = 1, AssignedAt = DateTime.UtcNow });
        await context.SaveChangesAsync(cancellationToken);
        return user.UserId;
    }

    public async Task UpdatePasswordAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default)
        => await context.Users
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.PasswordHash, passwordHash), cancellationToken);
}
