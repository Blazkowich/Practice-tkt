using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class VenueSeatMappingExtensions
{
    public static IQueryable<VenueSeatModel> MapToDomainModel(this IQueryable<VenueSeat> query)
        => query.Select(MapToDomainExpression);

    public static VenueSeatModel MapToDomainModel(this VenueSeat vs) => new()
    {
        VenueSeatId = vs.VenueSeatId,
        SectionId   = vs.SectionId,
        RowId       = vs.RowId,
        SeatNumber  = vs.SeatNumber,
        SortOrder   = vs.SortOrder
    };

    private static readonly Expression<Func<VenueSeat, VenueSeatModel>> MapToDomainExpression =
        vs => new VenueSeatModel
        {
            VenueSeatId = vs.VenueSeatId,
            SectionId   = vs.SectionId,
            RowId       = vs.RowId,
            SeatNumber  = vs.SeatNumber,
            SortOrder   = vs.SortOrder
        };
}
