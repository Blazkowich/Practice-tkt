namespace EPM.TKT.Domain.Models.Auth;

public sealed record RoleModel
{
    public byte RoleId { get; init; }
    public string Name { get; init; } = string.Empty;
}
