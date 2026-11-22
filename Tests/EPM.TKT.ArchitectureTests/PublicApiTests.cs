using System.Reflection;
using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class PublicApiTests
{
    // ── Commands and queries must be public ───────────────────────────────────

    [Test]
    public void Commands_Should_Be_Public()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().HaveNameEndingWith("Command")
            .Should().BePublic()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Commands are part of the application API surface and must be public.");
    }

    [Test]
    public void Queries_Should_Be_Public()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().HaveNameEndingWith("Query")
            .Should().BePublic()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Queries are part of the application API surface and must be public.");
    }

    // ── Handlers must be internal (implementation detail) ────────────────────

    [Test]
    public void Handlers_Should_Not_Be_Public()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().HaveNameEndingWith("Handler")
            .Should().NotBePublic()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Handlers are implementation details and must not be public.");
    }

    // ── Repository implementations must be internal ───────────────────────────

    [Test]
    public void Repository_Implementations_Should_Not_Be_Public()
    {
        var result = Types.InAssembly(TestAssemblies.Infrastructure)
            .That().ResideInNamespace(ArchitectureConfig.InfraRepositoriesNs)
            .And().AreClasses()
            .Should().NotBePublic()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Repository implementations are infrastructure details and must not be public.");
    }

    // ── Domain models should have accessible constructors ────────────────────

    [Test]
    public void Domain_Public_Classes_Should_Have_Public_Constructor()
    {
        // Only model types (records used as DTOs) are required to have public constructors.
        // Utility types such as Result<T> and Error intentionally use factory methods
        // with private constructors and live outside the Models sub-namespace.
        var publicClasses = TestAssemblies.Domain
            .GetTypes()
            .Where(t => t.IsClass && t.IsPublic && !t.IsAbstract
                     && t.Namespace?.StartsWith(ArchitectureConfig.DomainModelsNs) == true);

        foreach (var type in publicClasses)
        {
            var hasPublicCtor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length > 0;
            Assert.That(hasPublicCtor, Is.True,
                $"Public domain class '{type.FullName}' has no public constructor.");
        }
    }
}
