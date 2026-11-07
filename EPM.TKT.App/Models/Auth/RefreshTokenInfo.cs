namespace EPM.TKT.App.Models.Auth;

public sealed record RefreshTokenInfo
{
    public Guid TokenId { get; init; }
    public Guid UserId { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsRevoked { get; init; }
}
