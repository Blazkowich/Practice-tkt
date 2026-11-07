using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class CartItemMappingExtensions
{
    public static IQueryable<CartItemModel> MapToDomainModel(this IQueryable<CartItem> query)
        => query.Select(MapToDomainExpression);

    private static readonly Expression<Func<CartItem, CartItemModel>> MapToDomainExpression =
        ci => new CartItemModel
        {
            EventSeatId = ci.EventSeatId,
            VenueSeatId = ci.EventSeat != null ? ci.EventSeat.VenueSeatId : 0,
            SeatNumber  = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                ? ci.EventSeat.VenueSeat.SeatNumber : null,
            RowName     = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                          && ci.EventSeat.VenueSeat.Row != null
                ? ci.EventSeat.VenueSeat.Row.Name : null,
            SectionName = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                          && ci.EventSeat.VenueSeat.Section != null
                ? ci.EventSeat.VenueSeat.Section.Name : string.Empty,
            PriceId     = ci.EventSeat != null && ci.EventSeat.PriceId.HasValue
                ? ci.EventSeat.PriceId.Value : 0,
            PriceName   = ci.EventSeat != null && ci.EventSeat.Price != null
                          && ci.EventSeat.Price.PriceCategory != null
                ? ci.EventSeat.Price.PriceCategory.Name : string.Empty,
            Amount      = ci.EventSeat != null && ci.EventSeat.Price != null
                ? ci.EventSeat.Price.Amount : 0,
            Currency    = ci.EventSeat != null && ci.EventSeat.Price != null
                ? ci.EventSeat.Price.Currency : "USD"
        };
}
