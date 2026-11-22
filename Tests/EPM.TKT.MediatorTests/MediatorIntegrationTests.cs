using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.Mediator.DependencyInjection;
using EPM.TKT.Mediator.Exceptions;
using EPM.TKT.Mediator.Services;
using EPM.TKT.MediatorTests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace EPM.TKT.MediatorTests;

[TestFixture]
public sealed class MediatorIntegrationTests
{
    [SetUp]
    public void ResetVoidHandlerCounter() => LogCommandHandler.InvocationCount = 0;

    [Test]
    public async Task Send_runs_handler_for_request()
    {
        using var provider = CreateProviderWithTestAssemblyHandlers();

        var sender = provider.GetRequiredService<ISender>();
        var response = await sender.Send(new EchoRequest("hello"), CancellationToken.None);

        Assert.That(response, Is.EqualTo("hello"));
    }

    [Test]
    public async Task Send_returns_correct_data_from_handler()
    {
        using var provider = CreateProviderWithTestAssemblyHandlers();

        var sender = provider.GetRequiredService<ISender>();
        var sum = await sender.Send(new SumRequest(3, 4), CancellationToken.None);

        Assert.That(sum, Is.EqualTo(7));
    }

    [Test]
    public async Task Single_class_can_handle_multiple_request_types()
    {
        using var provider = CreateProviderWithTestAssemblyHandlers();

        var sender = provider.GetRequiredService<ISender>();
        var alpha = await sender.Send(new AlphaRequest(), CancellationToken.None);
        var beta = await sender.Send(new BetaRequest(), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(alpha, Is.EqualTo(10));
            Assert.That(beta, Is.EqualTo(20));
        }
    }

    [Test]
    public async Task Send_runs_handler_for_request_without_response()
    {
        using var provider = CreateProviderWithTestAssemblyHandlers();

        var sender = provider.GetRequiredService<ISender>();
        await sender.Send(new LogCommand("x"), CancellationToken.None);

        Assert.That(LogCommandHandler.InvocationCount, Is.EqualTo(1));
    }

    [Test]
    public void Send_propagates_exception_thrown_inside_handler()
    {
        using var provider = CreateProviderWithTestAssemblyHandlers();

        var sender = provider.GetRequiredService<ISender>();

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sender.Send(new ExplodingRequest(), CancellationToken.None));

        Assert.That(ex!.Message, Is.EqualTo("handler failure"));
    }

    [Test]
    public void AddMediator_registers_handlers_from_specified_assembly_only()
    {
        var servicesOnlyMediatorCore = new ServiceCollection();
        servicesOnlyMediatorCore.AddMediator(typeof(Sender).Assembly);
        using var providerCore = servicesOnlyMediatorCore.BuildServiceProvider();
        var senderCore = providerCore.GetRequiredService<ISender>();

        Assert.ThrowsAsync<HandlerNotRegisteredException>(async () =>
            await senderCore.Send(new EchoRequest("no"), CancellationToken.None));

        var servicesWithTests = new ServiceCollection();
        servicesWithTests.AddMediator(typeof(EchoHandler).Assembly);
        using var providerTests = servicesWithTests.BuildServiceProvider();
        var senderTests = providerTests.GetRequiredService<ISender>();

        Assert.DoesNotThrowAsync(async () =>
        {
            var r = await senderTests.Send(new EchoRequest("yes"), CancellationToken.None);
            Assert.That(r, Is.EqualTo("yes"));
        });
    }

    [Test]
    public void Send_unregistered_request_throws_HandlerNotRegisteredException()
    {
        using var provider = CreateProviderWithTestAssemblyHandlers();
        var sender = provider.GetRequiredService<ISender>();

        var ex = Assert.ThrowsAsync<HandlerNotRegisteredException>(async () =>
            await sender.Send(new UnhandledRequest(), CancellationToken.None));

        Assert.That(ex!.RequestType, Is.EqualTo(typeof(UnhandledRequest)));
    }

    [Test]
    public void AddMediator_TAnchor_uses_assembly_containing_anchor()
    {
        var services = new ServiceCollection();
        services.AddMediator<MediatorTestAssemblyAnchor>();
        using var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<ISender>();

        Assert.DoesNotThrowAsync(async () =>
        {
            var n = await sender.Send(new SumRequest(1, 2), CancellationToken.None);
            Assert.That(n, Is.EqualTo(3));
        });
    }

    private static ServiceProvider CreateProviderWithTestAssemblyHandlers()
    {
        var services = new ServiceCollection();
        services.AddMediator(typeof(EchoHandler).Assembly);
        return services.BuildServiceProvider();
    }
}
