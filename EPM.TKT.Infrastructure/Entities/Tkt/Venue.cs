namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Venue
{
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
