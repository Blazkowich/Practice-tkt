namespace EPM.TKT.WebApi.Contracts.Orders;

public sealed record AddToCartHttpRequest(int EventId, int SeatId, int PriceId);
