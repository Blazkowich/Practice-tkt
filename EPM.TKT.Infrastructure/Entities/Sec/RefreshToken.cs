using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Sec;

[Table("RefreshToken", Schema = Constants.Sec)]
internal class RefreshToken
{
    [Key]
    public Guid TokenId { get; set; }

    public Guid UserId { get; set; }

    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
