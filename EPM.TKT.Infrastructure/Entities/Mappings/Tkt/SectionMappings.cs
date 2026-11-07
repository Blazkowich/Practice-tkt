using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class SectionMappingExtensions
{
    public static IQueryable<SectionModel> MapToDomainModel(this IQueryable<Section> query)
        => query.Select(MapToDomainExpression);

    private static readonly Expression<Func<Section, SectionModel>> MapToDomainExpression =
        s => new SectionModel
        {
            SectionId = s.SectionId,
            VenueId   = s.VenueId,
            Name      = s.Name,
            SeatType  = s.SeatType != null ? s.SeatType.Name : string.Empty
        };
}
