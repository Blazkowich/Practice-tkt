namespace EPM.TKT.WebApi.Contracts.Events;

public sealed record PriceOptionHttpResponse(int PriceId, string Name, decimal Amount, string Currency);
