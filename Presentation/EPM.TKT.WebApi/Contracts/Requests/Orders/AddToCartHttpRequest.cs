namespace EPM.TKT.WebApi.Contracts.Requests.Orders;

public sealed record AddToCartHttpRequest(int EventId, int SeatId, int PriceId);
