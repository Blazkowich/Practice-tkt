using EPM.TKT.App.Models.Auth;

namespace EPM.TKT.App.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserAuthInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserAuthInfo?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Guid> CreateUserAsync(string email, string passwordHash, string? firstName, string? lastName, CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default);
    Task SaveRefreshTokenAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken = default);
    Task<RefreshTokenInfo?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
