namespace EPM.TKT.Domain.Models.Auth;

public sealed record UserModel
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsActive { get; init; }
}
