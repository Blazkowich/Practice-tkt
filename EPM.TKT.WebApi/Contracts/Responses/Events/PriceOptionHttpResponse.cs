namespace EPM.TKT.WebApi.Contracts.Responses.Events;

public sealed record PriceOptionHttpResponse(int PriceId, string Name, decimal Amount, string Currency);
