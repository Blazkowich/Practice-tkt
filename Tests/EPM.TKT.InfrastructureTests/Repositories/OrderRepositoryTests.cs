using EPM.TKT.Infrastructure.Context.Interfaces;
using EPM.TKT.Infrastructure.Entities.Tkt;
using EPM.TKT.Infrastructure.Repositories;
using EPM.TKT.InfrastructureTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EPM.TKT.InfrastructureTests.Repositories;

[TestFixture]
public sealed class OrderRepositoryTests
{
    private Mock<ITKTDbContext> _context    = null!;
    private Mock<DbSet<Order>>  _mockOrders = null!;
    private OrderRepository     _repo       = null!;

    [SetUp]
    public void SetUp()
    {
        _context    = new Mock<ITKTDbContext>();
        _mockOrders = MockDbSet.Create<Order>();

        _context.Setup(c => c.Orders).Returns(_mockOrders.Object);
        _context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _repo = new OrderRepository(_context.Object);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task CreateAsync_AddsOrder_WithCorrectFields()
    {
        var userId  = Guid.NewGuid();
        var cartId  = Guid.NewGuid();
        const int eventId = 7;
        const decimal total = 149.99m;

        Order? captured = null;
        _mockOrders.Setup(d => d.Add(It.IsAny<Order>()))
                   .Callback<Order>(o => captured = o);

        await _repo.CreateAsync(userId, eventId, cartId, total);

        Assert.That(captured,              Is.Not.Null);
        Assert.That(captured!.UserId,      Is.EqualTo(userId));
        Assert.That(captured.EventId,      Is.EqualTo(eventId));
        Assert.That(captured.CartId,       Is.EqualTo(cartId));
        Assert.That(captured.TotalAmount,  Is.EqualTo(total));
    }

    [Test]
    public async Task CreateAsync_SetsCreatedAt_ToUtcNow()
    {
        Order? captured = null;
        _mockOrders.Setup(d => d.Add(It.IsAny<Order>()))
                   .Callback<Order>(o => captured = o);

        var before = DateTime.UtcNow;
        await _repo.CreateAsync(Guid.NewGuid(), 1, Guid.NewGuid(), 50m);
        var after = DateTime.UtcNow;

        Assert.That(captured!.CreatedAt, Is.InRange(before, after));
    }

    [Test]
    public async Task CreateAsync_CallsSaveChanges()
    {
        _mockOrders.Setup(d => d.Add(It.IsAny<Order>()));

        await _repo.CreateAsync(Guid.NewGuid(), 1, Guid.NewGuid(), 100m);

        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ReturnsNewOrderId()
    {
        Order? captured = null;
        _mockOrders.Setup(d => d.Add(It.IsAny<Order>()))
                   .Callback<Order>(o => captured = o);

        var id = await _repo.CreateAsync(Guid.NewGuid(), 1, Guid.NewGuid(), 75m);

        Assert.That(id, Is.EqualTo(captured!.OrderId));
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Test]
    public async Task GetByIdAsync_ReturnsOrder_WhenFound()
    {
        var orderId = Guid.NewGuid();
        var userId  = Guid.NewGuid();
        var cartId  = Guid.NewGuid();
        var orders = new List<Order>
        {
            new()
            {
                OrderId = orderId, UserId = userId, EventId = 3,
                CartId = cartId, TotalAmount = 200m, CreatedAt = DateTime.UtcNow
            }
        };
        _context.Setup(c => c.Orders).Returns(MockDbSet.Create(orders).Object);
        var repo = new OrderRepository(_context.Object);

        var result = await repo.GetByIdAsync(orderId);

        Assert.That(result,              Is.Not.Null);
        Assert.That(result!.OrderId,     Is.EqualTo(orderId));
        Assert.That(result.TotalAmount,  Is.EqualTo(200m));
        Assert.That(result.CartId,       Is.EqualTo(cartId));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _context.Setup(c => c.Orders).Returns(MockDbSet.Create<Order>().Object);
        var repo = new OrderRepository(_context.Object);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    // ── GetByUserAsync ────────────────────────────────────────────────────────

    [Test]
    public async Task GetByUserAsync_ReturnsOrders_ForUser()
    {
        var userId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new() { OrderId = Guid.NewGuid(), UserId = userId,        EventId = 1, CartId = Guid.NewGuid(), TotalAmount = 50m,  CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new() { OrderId = Guid.NewGuid(), UserId = userId,        EventId = 2, CartId = Guid.NewGuid(), TotalAmount = 80m,  CreatedAt = DateTime.UtcNow },
            new() { OrderId = Guid.NewGuid(), UserId = Guid.NewGuid(), EventId = 3, CartId = Guid.NewGuid(), TotalAmount = 120m, CreatedAt = DateTime.UtcNow } // different user
        };
        _context.Setup(c => c.Orders).Returns(MockDbSet.Create(orders).Object);
        var repo = new OrderRepository(_context.Object);

        var result = await repo.GetByUserAsync(userId);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(o => o.UserId == userId), Is.True);
    }

    [Test]
    public async Task GetByUserAsync_ReturnsOrdersDescendingByCreatedAt()
    {
        var userId = Guid.NewGuid();
        var older  = DateTime.UtcNow.AddDays(-1);
        var newer  = DateTime.UtcNow;
        var orders = new List<Order>
        {
            new() { OrderId = Guid.NewGuid(), UserId = userId, EventId = 1, CartId = Guid.NewGuid(), TotalAmount = 10m, CreatedAt = older },
            new() { OrderId = Guid.NewGuid(), UserId = userId, EventId = 2, CartId = Guid.NewGuid(), TotalAmount = 20m, CreatedAt = newer }
        };
        _context.Setup(c => c.Orders).Returns(MockDbSet.Create(orders).Object);
        var repo = new OrderRepository(_context.Object);

        var result = await repo.GetByUserAsync(userId);

        Assert.That(result[0].CreatedAt, Is.EqualTo(newer));
        Assert.That(result[1].CreatedAt, Is.EqualTo(older));
    }

    [Test]
    public async Task GetByUserAsync_ReturnsEmpty_WhenNoOrders()
    {
        _context.Setup(c => c.Orders).Returns(MockDbSet.Create<Order>().Object);
        var repo = new OrderRepository(_context.Object);

        var result = await repo.GetByUserAsync(Guid.NewGuid());

        Assert.That(result, Is.Empty);
    }
}
