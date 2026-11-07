namespace EPM.TKT.WebApi.Contracts.Events;

public sealed record EventHttpResponse(
    int       EventId,
    string    Name,
    string?   Description,
    DateTime  EventDate,
    DateTime? DoorsOpenAt,
    string    Status);
