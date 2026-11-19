using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class OfferMappingExtensions
{
    public static IQueryable<OfferModel> MapToDomainModel(this IQueryable<Offer> query)
        => query.Select(MapToDomainExpression);

    public static OfferModel MapToDomainModel(this Offer o) => new()
    {
        OfferId     = o.OfferId,
        EventId     = o.EventId,
        Name        = o.Name,
        SaleStartAt = o.SaleStartAt,
        SaleEndAt   = o.SaleEndAt,
        IsActive    = o.IsActive,
        CreatedAt   = o.CreatedAt
    };

    private static readonly Expression<Func<Offer, OfferModel>> MapToDomainExpression =
        o => new OfferModel
        {
            OfferId     = o.OfferId,
            EventId     = o.EventId,
            Name        = o.Name,
            SaleStartAt = o.SaleStartAt,
            SaleEndAt   = o.SaleEndAt,
            IsActive    = o.IsActive,
            CreatedAt   = o.CreatedAt
        };
}
