using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Ticket;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class TicketMappingExtensions
{
    public static IQueryable<TicketModel> MapToDomainModel(this IQueryable<Ticket> query)
        => query.Select(MapToDomainExpression);

    public static TicketModel MapToDomainModel(this Ticket t) => new()
    {
        TicketId             = t.TicketId,
        OrderId              = t.OrderId,
        EventId              = t.EventId,
        EventSeatId          = t.EventSeatId,
        UserId               = t.UserId,
        PriceId              = t.PriceId,
        PaidAmount           = t.PaidAmount,
        IssuedAt             = t.IssuedAt,
        CancelledAt          = t.CancelledAt,
        CancelledByManagerId = t.CancelledByManagerId
    };

    private static readonly Expression<Func<Ticket, TicketModel>> MapToDomainExpression =
        t => new TicketModel
        {
            TicketId             = t.TicketId,
            OrderId              = t.OrderId,
            EventId              = t.EventId,
            EventSeatId          = t.EventSeatId,
            UserId               = t.UserId,
            PriceId              = t.PriceId,
            PaidAmount           = t.PaidAmount,
            IssuedAt             = t.IssuedAt,
            CancelledAt          = t.CancelledAt,
            CancelledByManagerId = t.CancelledByManagerId
        };
}
