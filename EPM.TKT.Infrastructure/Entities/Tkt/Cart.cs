using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Cart", Schema = Constants.Tkt)]
internal class Cart
{
    [Key]
    public Guid CartId { get; set; }

    public Guid UserId { get; set; }

    public int EventId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}
