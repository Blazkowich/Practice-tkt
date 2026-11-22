using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Infrastructure.Entities.Mappings.Sec;
using EPM.TKT.Infrastructure.Entities.Sec;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class RoleRepository(ITKTDbContext context) : IRoleRepository
{
    public async Task<IReadOnlyList<RoleModel>> GetAllAsync(CancellationToken ct = default)
        => await context.Roles
            .AsNoTracking()
            .OrderBy(r => r.RoleId)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<RoleModel?> GetByIdAsync(byte roleId, CancellationToken ct = default)
        => await context.Roles
            .AsNoTracking()
            .Where(r => r.RoleId == roleId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<RoleModel>> GetUserRolesAsync(Guid userId, CancellationToken ct = default)
        => await context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(context.Roles, ur => ur.RoleId, r => r.RoleId, (_, r) => r)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task AssignRoleAsync(Guid userId, byte roleId, CancellationToken ct = default)
    {
        var exists = await context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);

        if (!exists)
        {
            context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveRoleAsync(Guid userId, byte roleId, CancellationToken ct = default)
        => await context.UserRoles
            .Where(ur => ur.UserId == userId && ur.RoleId == roleId)
            .ExecuteDeleteAsync(ct);
}
