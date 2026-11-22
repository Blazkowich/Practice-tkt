using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Enums;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Infrastructure.Entities.Mappings.Sec;
using EPM.TKT.Infrastructure.Entities.Sec;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class UserRepository(ITKTDbContext context) : IUserRepository
{
    public Task<bool> EmailExistsAsync(
        string email, CancellationToken ct = default
        ) => context.Users.AnyAsync(u => u.Email == email, ct);

    public async Task<UserAuthInfo?> GetByEmailAsync(
        string email, 
        CancellationToken ct = default
    ) => await context.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .MapToAuthInfo(context.UserRoles, context.Roles)
            .FirstOrDefaultAsync(ct);

    public async Task<UserAuthInfo?> GetByIdAsync(
        Guid userId, 
        CancellationToken ct = default
    ) => await context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .MapToAuthInfo(context.UserRoles, context.Roles)
            .FirstOrDefaultAsync(ct);

    public async Task<Guid> CreateUserAsync(
        string email,
        string passwordHash,
        string? firstName,
        string? lastName,
        CancellationToken ct = default
    )
    {
        return await context.ExecuteInTransactionAsync<Guid>(async token =>
        {
            var user = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            context.Users.Add(user);

            context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = (byte)RoleType.User,
                AssignedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync(token);
            return user.UserId;
        }, ct);
    }

    public async Task UpdatePasswordAsync(
        Guid userId, 
        string passwordHash, 
        CancellationToken ct = default
    ) => await context.Users
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.PasswordHash, passwordHash), ct);

    public async Task<IReadOnlyList<UserModel>> GetAllAsync(CancellationToken ct = default)
        => await context.Users
            .AsNoTracking()
            .OrderBy(u => u.Email)
            .MapToUserModel()
            .ToListAsync(ct);

    public async Task DeactivateAsync(
        Guid userId, 
        CancellationToken ct = default
    ) => await context.Users
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsActive, false), ct);
}
