using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Order", Schema = Constants.Tkt)]
internal class Order
{
    [Key]
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public int EventId { get; set; }

    public Guid CartId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(200)]
    public string? PaymentTransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    [ForeignKey(nameof(CartId))]
    public Cart? Cart { get; set; }
}
