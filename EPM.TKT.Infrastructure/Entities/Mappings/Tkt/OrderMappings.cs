using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Order;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class OrderMappingExtensions
{
    public static IQueryable<OrderModel> MapToDomainModel(this IQueryable<Order> query)
        => query.Select(MapToDomainExpression);

    public static OrderModel MapToDomainModel(this Order o) => new()
    {
        OrderId              = o.OrderId,
        UserId               = o.UserId,
        EventId              = o.EventId,
        CartId               = o.CartId,
        TotalAmount          = o.TotalAmount,
        PaymentTransactionId = o.PaymentTransactionId,
        CreatedAt            = o.CreatedAt
    };

    private static readonly Expression<Func<Order, OrderModel>> MapToDomainExpression =
        o => new OrderModel
        {
            OrderId              = o.OrderId,
            UserId               = o.UserId,
            EventId              = o.EventId,
            CartId               = o.CartId,
            TotalAmount          = o.TotalAmount,
            PaymentTransactionId = o.PaymentTransactionId,
            CreatedAt            = o.CreatedAt
        };
}
