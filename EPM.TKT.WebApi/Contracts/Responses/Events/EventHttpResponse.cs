namespace EPM.TKT.WebApi.Contracts.Responses.Events;

public sealed record EventHttpResponse(
    int       EventId,
    string    Name,
    string?   Description,
    DateTime  EventDate,
    DateTime? DoorsOpenAt,
    string    Status);
