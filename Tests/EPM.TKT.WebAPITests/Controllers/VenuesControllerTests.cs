using EPM.TKT.App.UseCases.Venue;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPM.TKT.WebAPITests.Controllers;

[TestFixture]
public sealed class VenuesControllerTests
{
    private Mock<ISender> _sender = null!;
    private VenuesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _sender = new Mock<ISender>();
        _controller = new VenuesController(_sender.Object);
    }

    // ── GetAll ───────────────────────────────────────────────────────────────

    [Test]
    public async Task GetAll_ReturnsOk_WithVenueList()
    {
        IReadOnlyList<VenueModel> venues =
        [
            new() { VenueId = 1, Name = "National Arena", City = "Tbilisi", Country = "Georgia" }
        ];
        _sender.Setup(s => s.Send(It.IsAny<GetVenuesQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(venues);

        var result = await _controller.GetAll(default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetAll_ReturnsOk_WithEmptyList()
    {
        _sender.Setup(s => s.Send(It.IsAny<GetVenuesQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((IReadOnlyList<VenueModel>)[]);

        var result = await _controller.GetAll(default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // ── GetSections ──────────────────────────────────────────────────────────

    [Test]
    public async Task GetSections_ReturnsOk_WhenSectionsFound()
    {
        IReadOnlyList<SectionModel> sections =
        [
            new() { SectionId = 1, Name = "Floor", SeatType = "Reserved" }
        ];
        _sender.Setup(s => s.Send(It.IsAny<GetVenueSectionsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<IReadOnlyList<SectionModel>>.Ok(sections));

        var result = await _controller.GetSections(venueId: 1, default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetSections_ReturnsNotFound_WhenVenueDoesNotExist()
    {
        _sender.Setup(s => s.Send(It.IsAny<GetVenueSectionsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<IReadOnlyList<SectionModel>>.Fail("Venue not found.", ErrorType.NotFound));

        var result = await _controller.GetSections(venueId: 99, default);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
}
