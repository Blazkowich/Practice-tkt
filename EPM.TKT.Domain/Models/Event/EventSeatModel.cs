namespace EPM.TKT.Domain.Models.Event;

public sealed record EventSeatModel
{
    public long      EventSeatId  { get; init; }
    public int       EventId      { get; init; }
    public int       VenueSeatId  { get; init; }
    public int?      PriceId      { get; init; }
    public string    SeatState    { get; init; } = string.Empty;
    public Guid?     HeldByUserId { get; init; }
    public DateTime? HoldUntil    { get; init; }
    public DateTime  CreatedAt    { get; init; }
}
