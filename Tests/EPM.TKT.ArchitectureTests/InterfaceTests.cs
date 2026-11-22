using EPM.TKT.Mediator.Abstractions;
using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

/// <summary>
/// Verifies that use-case types correctly implement the mediator contracts
/// without referencing any external web framework types.
/// </summary>
[TestFixture]
public sealed class InterfaceTests
{
    // ── Mediator contracts ────────────────────────────────────────────────────

    [Test]
    public void Commands_Should_Implement_IRequest()
    {
        var commandTypes = TestAssemblies.App
            .GetTypes()
            .Where(t => t.IsClass
                     && t.Name.EndsWith("Command")
                     && t.Namespace?.StartsWith(ArchitectureConfig.AppUseCasesNs) == true);

        foreach (var type in commandTypes)
        {
            bool implementsIRequest = type.GetInterfaces()
                .Any(i => i == typeof(IRequest) ||
                         (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)));

            Assert.That(implementsIRequest, Is.True,
                $"'{type.FullName}' ends with 'Command' but does not implement IRequest or IRequest<T>.");
        }
    }

    [Test]
    public void Queries_Should_Implement_IRequest()
    {
        var queryTypes = TestAssemblies.App
            .GetTypes()
            .Where(t => t.IsClass
                     && t.Name.EndsWith("Query")
                     && t.Namespace?.StartsWith(ArchitectureConfig.AppUseCasesNs) == true);

        foreach (var type in queryTypes)
        {
            bool implementsIRequest = type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            Assert.That(implementsIRequest, Is.True,
                $"'{type.FullName}' ends with 'Query' but does not implement IRequest<T>.");
        }
    }

    [Test]
    public void Handlers_Should_Implement_IRequestHandler()
    {
        var handlerTypes = TestAssemblies.App
            .GetTypes()
            .Where(t => t.IsClass
                     && t.Name.EndsWith("Handler")
                     && t.Namespace?.StartsWith(ArchitectureConfig.AppUseCasesNs) == true);

        foreach (var type in handlerTypes)
        {
            // Accept both the value-returning IRequestHandler<TRequest, TResponse>
            // and the void IRequestHandler<TRequest> used by fire-and-forget handlers (e.g. LogoutHandler).
            bool implementsHandler = type.GetInterfaces()
                .Any(i => i.IsGenericType &&
                         (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                          i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)));

            Assert.That(implementsHandler, Is.True,
                $"'{type.FullName}' ends with 'Handler' but does not implement IRequestHandler<TRequest> or IRequestHandler<TRequest, TResponse>.");
        }
    }

    // ── Repository interfaces ─────────────────────────────────────────────────

    [Test]
    public void App_Repository_Contracts_Should_Be_Interfaces()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().ResideInNamespace(ArchitectureConfig.AppRepositoriesNs)
            .Should().BeInterfaces()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Everything in App.Repositories must be an interface.");
    }

    [Test]
    public void App_Service_Contracts_Should_Be_Interfaces()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().ResideInNamespace(ArchitectureConfig.AppServicesNs)
            .Should().BeInterfaces()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Everything in App.Services must be an interface.");
    }
}
