namespace EPM.TKT.WebApi.Contracts.Responses.Orders;

public sealed record CartItemHttpResponse(
    long EventSeatId,
    int VenueSeatId,
    string? SeatNumber,
    string? RowName,
    string SectionName,
    int PriceId,
    string PriceName,
    decimal Amount,
    string Currency);
