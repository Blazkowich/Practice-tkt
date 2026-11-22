namespace EPM.TKT.WebApi.Contracts.Responses.Venues;

public sealed record VenueHttpResponse(int VenueId, string Name, string? City, string? Country);
