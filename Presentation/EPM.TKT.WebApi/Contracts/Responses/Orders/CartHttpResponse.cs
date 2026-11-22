namespace EPM.TKT.WebApi.Contracts.Responses.Orders;

public sealed record CartHttpResponse(
    Guid CartId,
    int EventId,
    IReadOnlyList<CartItemHttpResponse> Items,
    decimal TotalAmount);
