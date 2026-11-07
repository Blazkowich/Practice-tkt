namespace EPM.TKT.WebApi.Contracts.Venues;

public sealed record VenueHttpResponse(int VenueId, string Name, string? City, string? Country);
