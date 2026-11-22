namespace EPM.TKT.Infrastructure.Entities.Sec;

internal class UserRole
{
    public Guid UserId { get; set; }
    public byte RoleId { get; set; }
    public DateTime AssignedAt { get; set; }

    public User? User { get; set; }
    public Role? Role { get; set; }
}
