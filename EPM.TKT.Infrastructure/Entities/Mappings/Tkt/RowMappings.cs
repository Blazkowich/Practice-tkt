using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class RowMappingExtensions
{
    public static IQueryable<RowModel> MapToDomainModel(this IQueryable<Row> query)
        => query.Select(MapToDomainExpression);

    public static RowModel MapToDomainModel(this Row r) => new()
    {
        RowId     = r.RowId,
        SectionId = r.SectionId,
        Name      = r.Name,
        SortOrder = r.SortOrder
    };

    private static readonly Expression<Func<Row, RowModel>> MapToDomainExpression =
        r => new RowModel
        {
            RowId     = r.RowId,
            SectionId = r.SectionId,
            Name      = r.Name,
            SortOrder = r.SortOrder
        };
}
