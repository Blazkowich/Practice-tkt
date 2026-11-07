using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Sec;

[Table("UserRole", Schema = Constants.Sec)]
internal class UserRole
{
    public Guid UserId { get; set; }

    public byte RoleId { get; set; }

    public DateTime AssignedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }
}
