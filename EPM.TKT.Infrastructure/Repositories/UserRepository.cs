using EPM.TKT.App.Models.Auth;
using EPM.TKT.App.Repositories;
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
            .Select(u => new UserAuthInfo
            {
                UserId       = u.UserId,
                Email        = u.Email,
                PasswordHash = u.PasswordHash,
                IsActive     = u.IsActive,
                Roles        = context.UserRoles
                    .Where(ur => ur.UserId == u.UserId)
                    .Select(ur => ur.Role!.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<UserAuthInfo?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .Select(u => new UserAuthInfo
            {
                UserId       = u.UserId,
                Email        = u.Email,
                PasswordHash = u.PasswordHash,
                IsActive     = u.IsActive,
                Roles        = context.UserRoles
                    .Where(ur => ur.UserId == u.UserId)
                    .Select(ur => ur.Role!.Name)
                    .ToList()
            })
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

    public async Task SaveRefreshTokenAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Add(new RefreshToken
        {
            UserId    = userId,
            Token     = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshTokenInfo?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        => await context.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.Token == token)
            .Select(rt => new RefreshTokenInfo
            {
                TokenId   = rt.TokenId,
                UserId    = rt.UserId,
                ExpiresAt = rt.ExpiresAt,
                IsRevoked = rt.IsRevoked
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        => await context.RefreshTokens
            .Where(rt => rt.Token == token)
            .ExecuteUpdateAsync(s => s.SetProperty(rt => rt.IsRevoked, true), cancellationToken);

    public async Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(rt => rt.IsRevoked, true), cancellationToken);
}
