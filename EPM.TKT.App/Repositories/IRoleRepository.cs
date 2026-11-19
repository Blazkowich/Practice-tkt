using EPM.TKT.Domain.Models.Auth;

namespace EPM.TKT.App.Repositories;

public interface IRoleRepository
{
    Task<IReadOnlyList<RoleModel>> GetAllAsync(CancellationToken ct = default);
    Task<RoleModel?> GetByIdAsync(byte roleId, CancellationToken ct = default);
    Task<IReadOnlyList<RoleModel>> GetUserRolesAsync(Guid userId, CancellationToken ct = default);
    Task AssignRoleAsync(Guid userId, byte roleId, CancellationToken ct = default);
    Task RemoveRoleAsync(Guid userId, byte roleId, CancellationToken ct = default);
}
