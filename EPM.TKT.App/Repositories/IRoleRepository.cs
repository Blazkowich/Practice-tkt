using EPM.TKT.Domain.Enums;
using EPM.TKT.Domain.Models.Auth;

namespace EPM.TKT.App.Repositories;

public interface IRoleRepository
{
    Task<IReadOnlyList<RoleModel>> GetAllAsync(CancellationToken ct = default);
    Task<RoleModel?> GetByIdAsync(
        RoleType roleType, 
        CancellationToken ct = default
    );
    Task<IReadOnlyList<RoleModel>> GetUserRolesAsync(
        Guid userId, 
        CancellationToken ct = default
    );
    Task AssignRoleAsync(
        Guid userId,
        RoleType roleType,
        CancellationToken ct = default
    );
    Task RemoveRoleAsync(
        Guid userId,
        RoleType roleType,
        CancellationToken ct = default
    );
}
