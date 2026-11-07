using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("CartItem", Schema = Constants.Tkt)]
internal class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    public Guid CartId { get; set; }

    public long EventSeatId { get; set; }

    public DateTime AddedAt { get; set; }

    [ForeignKey(nameof(CartId))]
    public Cart? Cart { get; set; }

    [ForeignKey(nameof(EventSeatId))]
    public EventSeat? EventSeat { get; set; }
}
