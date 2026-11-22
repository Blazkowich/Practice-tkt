using EPM.TKT.App.UseCases.Event;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPM.TKT.WebAPITests.Controllers;

[TestFixture]
public sealed class EventsControllerTests
{
    private Mock<ISender> _sender = null!;
    private EventsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _sender = new Mock<ISender>();
        _controller = new EventsController(_sender.Object);
    }

    // ── GetAll ───────────────────────────────────────────────────────────────

    [Test]
    public async Task GetAll_ReturnsOk_WithEventList()
    {
        IReadOnlyList<EventModel> events =
        [
            new() { EventId = 1, Name = "Concert", EventStatus = "OnSale", EventDate = DateTime.UtcNow.AddDays(10) }
        ];
        _sender.Setup(s => s.SendAsync(It.IsAny<GetEventsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(events);

        var result = await _controller.GetAllAsync(default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetAll_ReturnsOk_WithEmptyList()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<GetEventsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((IReadOnlyList<EventModel>)[]);

        var result = await _controller.GetAllAsync(default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // ── GetSeats ─────────────────────────────────────────────────────────────

    [Test]
    public async Task GetSeats_ReturnsOk_WhenSeatsFound()
    {
        IReadOnlyList<SeatAvailabilityModel> seats =
        [
            new()
            {
                EventSeatId = 1, VenueSeatId = 1, SectionId = 1,
                SeatState = new SeatStateModel { Id = 1, Name = "Available" },
                PriceOptions = []
            }
        ];
        _sender.Setup(s => s.SendAsync(It.IsAny<GetEventSectionSeatsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<IReadOnlyList<SeatAvailabilityModel>>.Ok(seats));

        var result = await _controller.GetSeatsAsync(eventId: 1, sectionId: 1, default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSeats_ReturnsNotFound_WhenEventOrSectionNotFound()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<GetEventSectionSeatsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<IReadOnlyList<SeatAvailabilityModel>>.Fail("Event not found.", ErrorType.NotFound));

        var result = await _controller.GetSeatsAsync(eventId: 99, sectionId: 1, default);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
}
