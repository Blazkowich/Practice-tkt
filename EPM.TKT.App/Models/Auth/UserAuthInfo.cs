namespace EPM.TKT.App.Models.Auth;

public sealed record UserAuthInfo
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? PasswordHash { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
}
