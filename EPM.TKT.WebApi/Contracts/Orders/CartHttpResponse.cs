namespace EPM.TKT.WebApi.Contracts.Orders;

public sealed record CartHttpResponse(
    Guid                           CartId,
    int                            EventId,
    IReadOnlyList<CartItemHttpResponse> Items,
    decimal                        TotalAmount);
