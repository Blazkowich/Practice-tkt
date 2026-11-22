using EPM.TKT.Infrastructure.Context.Interfaces;
using EPM.TKT.Infrastructure.Entities.Tkt;
using EPM.TKT.Infrastructure.Repositories;
using EPM.TKT.InfrastructureTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EPM.TKT.InfrastructureTests.Repositories;

[TestFixture]
public sealed class TicketRepositoryTests
{
    private Mock<ITKTDbContext>  _context     = null!;
    private Mock<DbSet<Ticket>> _mockTickets  = null!;
    private TicketRepository    _repo         = null!;

    [SetUp]
    public void SetUp()
    {
        _context     = new Mock<ITKTDbContext>();
        _mockTickets = MockDbSet.Create<Ticket>();

        _context.Setup(c => c.Tickets).Returns(_mockTickets.Object);
        _context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _repo = new TicketRepository(_context.Object);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task CreateAsync_AddsTicket_WithCorrectFields()
    {
        var orderId     = Guid.NewGuid();
        var userId      = Guid.NewGuid();
        const int eventId     = 5;
        const long eventSeatId = 42L;
        const int priceId     = 3;
        const decimal paid    = 75.00m;

        Ticket? captured = null;
        _mockTickets.Setup(d => d.Add(It.IsAny<Ticket>()))
                    .Callback<Ticket>(t => captured = t);

        await _repo.CreateAsync(orderId, eventId, eventSeatId, userId, priceId, paid);

        Assert.That(captured,               Is.Not.Null);
        Assert.That(captured!.OrderId,      Is.EqualTo(orderId));
        Assert.That(captured.EventId,       Is.EqualTo(eventId));
        Assert.That(captured.EventSeatId,   Is.EqualTo(eventSeatId));
        Assert.That(captured.UserId,        Is.EqualTo(userId));
        Assert.That(captured.PriceId,       Is.EqualTo(priceId));
        Assert.That(captured.PaidAmount,    Is.EqualTo(paid));
        Assert.That(captured.CancelledAt,   Is.Null);
    }

    [Test]
    public async Task CreateAsync_SetsIssuedAt_ToUtcNow()
    {
        Ticket? captured = null;
        _mockTickets.Setup(d => d.Add(It.IsAny<Ticket>()))
                    .Callback<Ticket>(t => captured = t);

        var before = DateTime.UtcNow;
        await _repo.CreateAsync(Guid.NewGuid(), 1, 1L, Guid.NewGuid(), 1, 10m);
        var after = DateTime.UtcNow;

        Assert.That(captured!.IssuedAt, Is.InRange(before, after));
    }

    [Test]
    public async Task CreateAsync_CallsSaveChanges()
    {
        _mockTickets.Setup(d => d.Add(It.IsAny<Ticket>()));

        await _repo.CreateAsync(Guid.NewGuid(), 1, 1L, Guid.NewGuid(), 1, 50m);

        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ReturnsNewTicketId()
    {
        Ticket? captured = null;
        _mockTickets.Setup(d => d.Add(It.IsAny<Ticket>()))
                    .Callback<Ticket>(t => captured = t);

        var id = await _repo.CreateAsync(Guid.NewGuid(), 1, 1L, Guid.NewGuid(), 1, 50m);

        Assert.That(id, Is.EqualTo(captured!.TicketId));
    }

    // ── GetByOrderAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task GetByOrderAsync_ReturnsTickets_ForOrder()
    {
        var orderId = Guid.NewGuid();
        var tickets = new List<Ticket>
        {
            new() { TicketId = Guid.NewGuid(), OrderId = orderId,        EventId = 1, UserId = Guid.NewGuid(), IssuedAt = DateTime.UtcNow },
            new() { TicketId = Guid.NewGuid(), OrderId = orderId,        EventId = 1, UserId = Guid.NewGuid(), IssuedAt = DateTime.UtcNow },
            new() { TicketId = Guid.NewGuid(), OrderId = Guid.NewGuid(), EventId = 2, UserId = Guid.NewGuid(), IssuedAt = DateTime.UtcNow } // different order
        };
        _context.Setup(c => c.Tickets).Returns(MockDbSet.Create(tickets).Object);
        var repo = new TicketRepository(_context.Object);

        var result = await repo.GetByOrderAsync(orderId);

        Assert.That(result.Count,                    Is.EqualTo(2));
        Assert.That(result.All(t => t.OrderId == orderId), Is.True);
    }

    [Test]
    public async Task GetByOrderAsync_ReturnsEmpty_WhenNoTicketsForOrder()
    {
        _context.Setup(c => c.Tickets).Returns(MockDbSet.Create<Ticket>().Object);
        var repo = new TicketRepository(_context.Object);

        var result = await repo.GetByOrderAsync(Guid.NewGuid());

        Assert.That(result, Is.Empty);
    }

    // ── GetByUserAsync ────────────────────────────────────────────────────────

    [Test]
    public async Task GetByUserAsync_ReturnsTickets_ForUser()
    {
        var userId = Guid.NewGuid();
        var tickets = new List<Ticket>
        {
            new() { TicketId = Guid.NewGuid(), OrderId = Guid.NewGuid(), UserId = userId,        EventId = 1, IssuedAt = DateTime.UtcNow.AddDays(-1) },
            new() { TicketId = Guid.NewGuid(), OrderId = Guid.NewGuid(), UserId = userId,        EventId = 2, IssuedAt = DateTime.UtcNow             },
            new() { TicketId = Guid.NewGuid(), OrderId = Guid.NewGuid(), UserId = Guid.NewGuid(), EventId = 3, IssuedAt = DateTime.UtcNow             } // different user
        };
        _context.Setup(c => c.Tickets).Returns(MockDbSet.Create(tickets).Object);
        var repo = new TicketRepository(_context.Object);

        var result = await repo.GetByUserAsync(userId);

        Assert.That(result.Count,                    Is.EqualTo(2));
        Assert.That(result.All(t => t.UserId == userId), Is.True);
    }

    [Test]
    public async Task GetByUserAsync_ReturnsTicketsDescendingByIssuedAt()
    {
        var userId = Guid.NewGuid();
        var older  = DateTime.UtcNow.AddDays(-2);
        var newer  = DateTime.UtcNow;
        var tickets = new List<Ticket>
        {
            new() { TicketId = Guid.NewGuid(), OrderId = Guid.NewGuid(), UserId = userId, EventId = 1, IssuedAt = older },
            new() { TicketId = Guid.NewGuid(), OrderId = Guid.NewGuid(), UserId = userId, EventId = 2, IssuedAt = newer }
        };
        _context.Setup(c => c.Tickets).Returns(MockDbSet.Create(tickets).Object);
        var repo = new TicketRepository(_context.Object);

        var result = await repo.GetByUserAsync(userId);

        Assert.That(result[0].IssuedAt, Is.EqualTo(newer));
        Assert.That(result[1].IssuedAt, Is.EqualTo(older));
    }

    [Test]
    public async Task GetByUserAsync_ReturnsEmpty_WhenNoTickets()
    {
        _context.Setup(c => c.Tickets).Returns(MockDbSet.Create<Ticket>().Object);
        var repo = new TicketRepository(_context.Object);

        var result = await repo.GetByUserAsync(Guid.NewGuid());

        Assert.That(result, Is.Empty);
    }
}
