using System.Reflection;
using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class ImmutabilityTests
{
    // ── Domain models ─────────────────────────────────────────────────────────

    [Test]
    public void Domain_Models_Should_Be_Immutable()
    {
        var types = TestAssemblies.Domain
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.Namespace?.StartsWith(ArchitectureConfig.DomainModelsNs) == true);

        AssertNoMutableProperties(types, "Domain model");
    }

    // ── Commands and queries ──────────────────────────────────────────────────

    [Test]
    public void Commands_Should_Be_Immutable()
    {
        var types = TestAssemblies.App
            .GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Command")
                     && t.Namespace?.StartsWith(ArchitectureConfig.AppUseCasesNs) == true);

        AssertNoMutableProperties(types, "Command");
    }

    [Test]
    public void Queries_Should_Be_Immutable()
    {
        var types = TestAssemblies.App
            .GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Query")
                     && t.Namespace?.StartsWith(ArchitectureConfig.AppUseCasesNs) == true);

        AssertNoMutableProperties(types, "Query");
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

    // ── Helper ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Checks that every public instance property is either read-only or init-only.
    /// C# records expose <c>init</c> setters which compile to a real setter method in IL
    /// (marked with <c>IsExternalInit</c>). NetArchTest's BeImmutable() treats those as
    /// mutable; this helper correctly accepts them.
    /// </summary>
    private static void AssertNoMutableProperties(IEnumerable<Type> types, string context)
    {
        foreach (var type in types)
        {
            var mutable = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.SetMethod is { IsPublic: true } && !IsInitOnly(p.SetMethod))
                .Select(p => p.Name)
                .ToList();

            Assert.That(mutable, Is.Empty,
                $"{context} '{type.FullName}' has regular public setters: {string.Join(", ", mutable)}.");
        }
    }

    private static bool IsInitOnly(MethodInfo setter) =>
        setter.ReturnParameter
              .GetRequiredCustomModifiers()
              .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
}
