using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class VenueMappingExtensions
{
    public static IQueryable<VenueModel> MapToDomainModel(this IQueryable<Venue> query)
        => query.Select(MapToDomainExpression);

    public static VenueModel MapToDomainModel(this Venue v)
        => MapToDomain(v);

    private static readonly Expression<Func<Venue, VenueModel>> MapToDomainExpression =
        v => new VenueModel
        {
            VenueId = v.VenueId,
            Name    = v.Name,
            City    = v.City,
            Country = v.Country
        };

    private static VenueModel MapToDomain(Venue v) => new()
    {
        VenueId = v.VenueId,
        Name    = v.Name,
        City    = v.City,
        Country = v.Country
    };
}
