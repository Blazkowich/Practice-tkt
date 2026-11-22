using EPM.TKT.Infrastructure.Context.Interfaces;
using EPM.TKT.Infrastructure.Entities.Tkt;
using EPM.TKT.Infrastructure.Repositories;
using EPM.TKT.InfrastructureTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EPM.TKT.InfrastructureTests.Repositories;

[TestFixture]
public sealed class VenueRepositoryTests
{
    private Mock<ITKTDbContext> _context = null!;
    private Mock<DbSet<Venue>> _mockVenues = null!;
    private VenueRepository _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new Mock<ITKTDbContext>();
        _mockVenues = MockDbSet.Create<Venue>();

        _context.Setup(c => c.Venues).Returns(_mockVenues.Object);
        _context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _repo = new VenueRepository(_context.Object);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task CreateAsync_AddsVenue_WithCorrectFields()
    {
        Venue? captured = null;
        _mockVenues.Setup(d => d.Add(It.IsAny<Venue>()))
                   .Callback<Venue>(v => captured = v);

        await _repo.CreateAsync("Arena", "5 Stadium Rd", "Tbilisi", "Georgia");

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.Name, Is.EqualTo("Arena"));
        Assert.That(captured.Address, Is.EqualTo("5 Stadium Rd"));
        Assert.That(captured.City, Is.EqualTo("Tbilisi"));
        Assert.That(captured.Country, Is.EqualTo("Georgia"));
        Assert.That(captured.IsActive, Is.True);
    }

    [Test]
    public async Task CreateAsync_SetsCreatedAt_ToUtcNow()
    {
        Venue? captured = null;
        _mockVenues.Setup(d => d.Add(It.IsAny<Venue>()))
                   .Callback<Venue>(v => captured = v);

        var before = DateTime.UtcNow;
        await _repo.CreateAsync("Hall", null, null, null);
        var after = DateTime.UtcNow;

        Assert.That(captured!.CreatedAt, Is.InRange(before, after));
    }

    [Test]
    public async Task CreateAsync_CallsSaveChanges()
    {
        _mockVenues.Setup(d => d.Add(It.IsAny<Venue>()));

        await _repo.CreateAsync("Hall", null, null, null);

        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ReturnsVenueId()
    {
        Venue? captured = null;
        _mockVenues.Setup(d => d.Add(It.IsAny<Venue>()))
                   .Callback<Venue>(v => captured = v);

        var id = await _repo.CreateAsync("Hall", null, null, null);

        Assert.That(id, Is.EqualTo(captured!.VenueId));
    }

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task GetAllAsync_ReturnsOnlyActiveVenues()
    {
        var venues = new List<Venue>
        {
            new() { VenueId = 1, Name = "Active",   IsActive = true  },
            new() { VenueId = 2, Name = "Inactive", IsActive = false }
        };
        _context.Setup(c => c.Venues).Returns(MockDbSet.Create(venues).Object);
        var repo = new VenueRepository(_context.Object);

        var result = await repo.GetAllAsync();

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Active"));
    }

    [Test]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoActiveVenues()
    {
        var venues = new List<Venue>
        {
            new() { VenueId = 1, Name = "Closed", IsActive = false }
        };
        _context.Setup(c => c.Venues).Returns(MockDbSet.Create(venues).Object);
        var repo = new VenueRepository(_context.Object);

        var result = await repo.GetAllAsync();

        Assert.That(result, Is.Empty);
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Test]
    public async Task GetByIdAsync_ReturnsVenue_WhenFound()
    {
        var venues = new List<Venue>
        {
            new() { VenueId = 42, Name = "Stadium", City = "Kutaisi", Country = "Georgia", IsActive = true }
        };
        _context.Setup(c => c.Venues).Returns(MockDbSet.Create(venues).Object);
        var repo = new VenueRepository(_context.Object);

        var result = await repo.GetByIdAsync(42);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.VenueId, Is.EqualTo(42));
        Assert.That(result.Name, Is.EqualTo("Stadium"));
        Assert.That(result.City, Is.EqualTo("Kutaisi"));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _context.Setup(c => c.Venues).Returns(MockDbSet.Create<Venue>().Object);
        var repo = new VenueRepository(_context.Object);

        var result = await repo.GetByIdAsync(99);

        Assert.That(result, Is.Null);
    }
}
