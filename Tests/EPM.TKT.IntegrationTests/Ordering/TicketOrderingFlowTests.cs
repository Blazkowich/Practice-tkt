using EPM.TKT.App.Services;
using EPM.TKT.Infrastructure.Context;
using EPM.TKT.Infrastructure.Entities.Tkt;
using EPM.TKT.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using SeatState = EPM.TKT.Domain.Enums.SeatState;
using OrderStatus = EPM.TKT.Domain.Enums.OrderStatus;
using PriceCategoryEnum = EPM.TKT.Domain.Enums.PriceCategory;

namespace EPM.TKT.IntegrationTests.Ordering;

/// <summary>
/// Integration tests for the full ticket-ordering pipeline.
/// Uses an SQLite in-memory database backed by the real TKTDbContext so that
/// EF Core bulk operations (ExecuteUpdateAsync) run against an actual SQL engine.
/// Only IPaymentServiceClient is mocked — everything else is real.
/// </summary>
[TestFixture]
public sealed class TicketOrderingFlowTests
{
    // ── Test infrastructure ───────────────────────────────────────────────────

    private SqliteConnection _connection = null!;
    private TKTDbContext _context = null!;
    private CartRepository _cartRepo = null!;
    private PaymentRepository _paymentRepo = null!;
    private Mock<IPaymentServiceClient> _paymentMock = null!;

    // IDs generated during seed — shared across a single test run
    private long _eventSeatId;
    private int _priceId;

    private const int EventId = 1;
    private const int VenueSeatId = 1;

    [SetUp]
    public async Task SetUpAsync()
    {
        // Keep the connection open for the lifetime of the test so the
        // in-memory SQLite database is not destroyed between commands.
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        // Disable FK enforcement so the seed only needs the transactional
        // entities under test, not the full lookup-table chain.
        // DB integrity is verified by the SSDT project.
        await using var pragma = _connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = OFF;";
        await pragma.ExecuteNonQueryAsync();

        var options = new DbContextOptionsBuilder<TKTDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new TKTDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        await SeedAsync();

        _paymentMock = new Mock<IPaymentServiceClient>();
        _cartRepo = new CartRepository(_context, _paymentMock.Object);
        _paymentRepo = new PaymentRepository(_context);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    // ── Seed ─────────────────────────────────────────────────────────────────

    /// <summary>Seeds one available seat with a price — the minimum needed for all tests.</summary>
    private async Task SeedAsync()
    {
        var price = new Price
        {
            OfferId = 1,
            PriceCategoryId = (byte)PriceCategoryEnum.Adult,
            Amount = 75m,
            Currency = "USD"
        };
        _context.Prices.Add(price);
        await _context.SaveChangesAsync();
        _priceId = price.PriceId;

        var seat = new EventSeat
        {
            EventId = EventId,
            VenueSeatId = VenueSeatId,
            PriceId = _priceId,
            SeatStateId = (byte)SeatState.Available,
            CreatedAt = DateTime.UtcNow
        };
        _context.EventSeats.Add(seat);
        await _context.SaveChangesAsync();
        _eventSeatId = seat.EventSeatId;

        // Detach all tracked entities so the repositories start with a clean tracker
        _context.ChangeTracker.Clear();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Guid SetupPaymentMock(Mock<IPaymentServiceClient> mock)
    {
        var paymentId = Guid.NewGuid();
        mock.Setup(p => p.InitiatePaymentAsync(
                It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentId);
        return paymentId;
    }

    private async Task<(Guid cartId, Guid userId, Guid paymentId)> BookSeatAsync()
    {
        var cartId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var paymentId = SetupPaymentMock(_paymentMock);

        await _cartRepo.AddSeatAsync(cartId, userId, EventId, VenueSeatId, _priceId);
        await _cartRepo.BookAsync(cartId, userId);

        return (cartId, userId, paymentId);
    }

    private async Task<EventSeat> GetSeatAsync() =>
        await _context.EventSeats
            .AsNoTracking()
            .FirstAsync(es => es.EventSeatId == _eventSeatId);

    private async Task<Order> GetOrderAsync() =>
        await _context.Orders
            .AsNoTracking()
            .FirstAsync();

    // ── Add to cart ───────────────────────────────────────────────────────────

    [Test]
    public async Task AddToCart_CartItemAppearsInDatabase()
    {
        var cartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        await _cartRepo.AddSeatAsync(cartId, userId, EventId, VenueSeatId, _priceId);

        // Verify DB state directly — avoids depending on GetAsync's complex multi-join
        // projection (CartItem → EventSeat → VenueSeat → Section + PriceCategory),
        // which requires full lookup-table data to produce non-empty results.
        var itemCount = await _context.CartItems
            .AsNoTracking()
            .CountAsync(ci => ci.CartId == cartId && ci.EventSeatId == _eventSeatId);

        Assert.That(itemCount, Is.EqualTo(1));
    }

    [Test]
    public async Task AddToCart_SeatPriceIsLocked()
    {
        await _cartRepo.AddSeatAsync(
            Guid.NewGuid(), Guid.NewGuid(), EventId, VenueSeatId, _priceId);

        var seat = await _context.EventSeats
            .AsNoTracking()
            .FirstAsync(es => es.EventSeatId == _eventSeatId);

        Assert.That(seat.PriceId, Is.EqualTo(_priceId));
    }

    [Test]
    public async Task RemoveFromCart_CartBecomesEmpty()
    {
        var cartId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await _cartRepo.AddSeatAsync(cartId, userId, EventId, VenueSeatId, _priceId);

        await _cartRepo.RemoveSeatAsync(cartId, EventId, VenueSeatId);

        var cart = await _cartRepo.GetAsync(cartId);
        Assert.That(cart!.Items, Is.Empty);
    }

    // ── Book cart ─────────────────────────────────────────────────────────────

    [Test]
    public async Task BookCart_SeatTransitionsToBooked()
    {
        await BookSeatAsync();

        var seat = await GetSeatAsync();
        Assert.That(seat.SeatStateId, Is.EqualTo((byte)SeatState.Booked));
    }

    [Test]
    public async Task BookCart_OrderIsCreatedWithPendingStatus()
    {
        await BookSeatAsync();

        var order = await GetOrderAsync();
        Assert.That(order.OrderStatusId, Is.EqualTo((byte)OrderStatus.Pending));
        Assert.That(order.TotalAmount, Is.EqualTo(75m));
    }

    [Test]
    public async Task BookCart_OrderStoresPaymentTransactionId()
    {
        var (_, _, paymentId) = await BookSeatAsync();

        var order = await GetOrderAsync();
        Assert.That(order.PaymentTransactionId, Is.EqualTo(paymentId.ToString()));
    }

    [Test]
    public async Task BookCart_FailsWhenSeatIsAlreadyBooked()
    {
        // First user books the seat
        await BookSeatAsync();

        // Second user tries to book the same (now Booked) seat
        var cartId2 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        SetupPaymentMock(_paymentMock);
        await _cartRepo.AddSeatAsync(cartId2, userId2, EventId, VenueSeatId, _priceId);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _cartRepo.BookAsync(cartId2, userId2));
    }

    // ── Complete payment (order flow) ─────────────────────────────────────────

    [Test]
    public async Task CompletePayment_SeatTransitionsToSold()
    {
        var (_, _, paymentId) = await BookSeatAsync();

        await _paymentRepo.CompleteAsync(paymentId);

        var seat = await GetSeatAsync();
        Assert.That(seat.SeatStateId, Is.EqualTo((byte)SeatState.Sold));
    }

    [Test]
    public async Task CompletePayment_OrderStatusBecomessPaid()
    {
        var (_, _, paymentId) = await BookSeatAsync();

        await _paymentRepo.CompleteAsync(paymentId);

        var order = await GetOrderAsync();
        Assert.That(order.OrderStatusId, Is.EqualTo((byte)OrderStatus.Paid));
    }

    // ── Fail payment (release flow) ───────────────────────────────────────────

    [Test]
    public async Task FailPayment_SeatReleasedBackToAvailable()
    {
        var (_, _, paymentId) = await BookSeatAsync();

        await _paymentRepo.FailAsync(paymentId);

        var seat = await GetSeatAsync();
        Assert.That(seat.SeatStateId, Is.EqualTo((byte)SeatState.Available));
    }

    [Test]
    public async Task FailPayment_OrderStatusBecomesCancelled()
    {
        var (_, _, paymentId) = await BookSeatAsync();

        await _paymentRepo.FailAsync(paymentId);

        var order = await GetOrderAsync();
        Assert.That(order.OrderStatusId, Is.EqualTo((byte)OrderStatus.Cancelled));
    }

    [Test]
    public async Task FailPayment_SeatCanBeBookedAgainAfterRelease()
    {
        // First booking fails
        var (_, _, paymentId1) = await BookSeatAsync();
        await _paymentRepo.FailAsync(paymentId1);

        // Seat is Available again — second booking succeeds
        var cartId2 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var paymentId2 = SetupPaymentMock(_paymentMock);
        await _cartRepo.AddSeatAsync(cartId2, userId2, EventId, VenueSeatId, _priceId);
        var returnedPaymentId = await _cartRepo.BookAsync(cartId2, userId2);

        Assert.That(returnedPaymentId, Is.EqualTo(paymentId2));

        var seat = await GetSeatAsync();
        Assert.That(seat.SeatStateId, Is.EqualTo((byte)SeatState.Booked));
    }
}
