using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class PriceMappingExtensions
{
    public static IQueryable<PriceModel> MapToDomainModel(this IQueryable<Price> query)
        => query.Select(MapToDomainExpression);

    public static PriceModel MapToDomainModel(this Price p) => new()
    {
        PriceId       = p.PriceId,
        OfferId       = p.OfferId,
        SectionId     = p.SectionId,
        RowId         = p.RowId,
        PriceCategory = p.PriceCategory != null ? p.PriceCategory.Name : string.Empty,
        Amount        = p.Amount,
        Currency      = p.Currency
    };

    private static readonly Expression<Func<Price, PriceModel>> MapToDomainExpression =
        p => new PriceModel
        {
            PriceId       = p.PriceId,
            OfferId       = p.OfferId,
            SectionId     = p.SectionId,
            RowId         = p.RowId,
            PriceCategory = p.PriceCategory != null ? p.PriceCategory.Name : string.Empty,
            Amount        = p.Amount,
            Currency      = p.Currency
        };
}
