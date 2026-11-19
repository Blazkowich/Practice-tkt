namespace EPM.TKT.Domain.Models.Ticket;

public sealed record TicketModel
{
    public Guid      TicketId             { get; init; }
    public Guid      OrderId              { get; init; }
    public int       EventId              { get; init; }
    public long      EventSeatId          { get; init; }
    public Guid      UserId               { get; init; }
    public int       PriceId              { get; init; }
    public decimal   PaidAmount           { get; init; }
    public DateTime  IssuedAt             { get; init; }
    public DateTime? CancelledAt          { get; init; }
    public Guid?     CancelledByManagerId { get; init; }
}
