namespace EPM.TKT.Infrastructure.Entities.Sec;

internal class User
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
