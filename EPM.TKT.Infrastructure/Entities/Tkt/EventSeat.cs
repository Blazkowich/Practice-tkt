namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class EventSeat
{
    public long EventSeatId { get; set; }
    public int EventId { get; set; }
    public int VenueSeatId { get; set; }
    public int? PriceId { get; set; }
    public byte SeatStateId { get; set; }
    public Guid? HeldByUserId { get; set; }
    public DateTime? HoldUntil { get; set; }
    public DateTime CreatedAt { get; set; }

    public Event? Event { get; set; }
    public VenueSeat? VenueSeat { get; set; }
    public Price? Price { get; set; }
    public SeatState? SeatState { get; set; }
}
