namespace EPM.TKT.App.Cache;

internal static class EventCacheKeys
{
    internal const string EventsList = "events:all";

    internal static string EventSeatGeneration(int eventId) =>
        $"events:{eventId}:seats:gen";

    internal static string EventSectionSeats(int eventId, int sectionId, int generation) =>
        $"events:{eventId}:sections:{sectionId}:seats:{generation}";
}
