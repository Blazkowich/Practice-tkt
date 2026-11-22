using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class ImmutabilityTests
{
    // ── Domain models ─────────────────────────────────────────────────────────

    [Test]
    public void Domain_Models_Should_Be_Immutable()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .That().ResideInNamespace("EPM.TKT.Domain.Models")
            .And().AreClasses()
            .Should().BeImmutable()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Domain models are records with init-only properties and must be immutable.");
    }

    // ── Commands and queries ──────────────────────────────────────────────────

    [Test]
    public void Commands_Should_Be_Immutable()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().HaveNameEndingWith("Command")
            .Should().BeImmutable()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Commands are sealed records; their properties must be read-only.");
    }

    [Test]
    public void Queries_Should_Be_Immutable()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().HaveNameEndingWith("Query")
            .Should().BeImmutable()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Queries are sealed records; their properties must be read-only.");
    }

    // ── Domain value objects and events (future-proofing) ────────────────────

    [Test]
    public void Value_Objects_Should_Be_Immutable()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .That().HaveNameEndingWith("ValueObject").Or().HaveNameEndingWith("VO")
            .Should().BeImmutable()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Value objects must be immutable.");
    }

    [Test]
    public void Domain_Events_Should_Be_Immutable()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .That().HaveNameEndingWith("Event").Or().HaveNameEndingWith("DomainEvent")
            .Should().BeImmutable()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain events must be immutable.");
    }
}
