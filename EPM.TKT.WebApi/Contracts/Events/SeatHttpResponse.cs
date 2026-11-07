namespace EPM.TKT.WebApi.Contracts.Events;

public sealed record SeatHttpResponse(
    long                           EventSeatId,
    int                            VenueSeatId,
    int                            SectionId,
    int?                           RowId,
    string?                        SeatNumber,
    SeatStateHttpResponse          State,
    IReadOnlyList<PriceOptionHttpResponse> PriceOptions);
