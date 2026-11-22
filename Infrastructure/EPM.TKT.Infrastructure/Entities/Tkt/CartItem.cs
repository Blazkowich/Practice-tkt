namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class CartItem
{
    public int CartItemId { get; set; }
    public Guid CartId { get; set; }
    public long EventSeatId { get; set; }
    public DateTime AddedAt { get; set; }

    public Cart? Cart { get; set; }
    public EventSeat? EventSeat { get; set; }
}
