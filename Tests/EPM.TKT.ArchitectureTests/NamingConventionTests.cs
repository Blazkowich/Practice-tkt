using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class NamingConventionTests
{
    // ── Controllers ──────────────────────────────────────────────────────────

    [Test]
    public void Controllers_Should_End_With_Controller()
    {
        var result = Types.InAssembly(TestAssemblies.WebApi)
            .That().ResideInNamespace(ArchitectureConfig.WebApiControllersNs)
            .And().AreClasses()
            .Should().HaveNameEndingWith("Controller")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Every class in the Controllers namespace must end with 'Controller'.");
    }

    // ── App use-case types ───────────────────────────────────────────────────

    [Test]
    public void Handlers_Should_End_With_Handler()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().ResideInNamespace(ArchitectureConfig.AppUseCasesNs)
            .And().AreClasses()
            .And().HaveNameEndingWith("Handler")
            .Should().HaveNameEndingWith("Handler")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Handler classes must end with 'Handler'.");
    }

    [Test]
    public void Commands_Should_End_With_Command()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().ResideInNamespace(ArchitectureConfig.AppUseCasesNs)
            .And().AreClasses()
            .And().HaveNameEndingWith("Command")
            .Should().HaveNameEndingWith("Command")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Command types must end with 'Command'.");
    }

    [Test]
    public void Queries_Should_End_With_Query()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().ResideInNamespace(ArchitectureConfig.AppUseCasesNs)
            .And().AreClasses()
            .And().HaveNameEndingWith("Query")
            .Should().HaveNameEndingWith("Query")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Query types must end with 'Query'.");
    }

    // ── Repositories ─────────────────────────────────────────────────────────

    [Test]
    public void Repository_Implementations_Should_End_With_Repository()
    {
        var result = Types.InAssembly(TestAssemblies.Infrastructure)
            .That().ResideInNamespace(ArchitectureConfig.InfraRepositoriesNs)
            .And().AreClasses()
            .Should().HaveNameEndingWith("Repository")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "All classes in the Repositories namespace must end with 'Repository'.");
    }

    [Test]
    public void Repository_Interfaces_Should_Start_With_I()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .That().ResideInNamespace(ArchitectureConfig.AppRepositoriesNs)
            .And().AreInterfaces()
            .Should().HaveNameStartingWith("I")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Repository interfaces must start with 'I'.");
    }

    // ── Domain ───────────────────────────────────────────────────────────────

    [Test]
    public void Domain_Types_Should_Not_Have_Technical_Suffixes()
    {
        var noService = Types.InAssembly(TestAssemblies.Domain)
            .That().AreClasses().And().AreNotAbstract()
            .ShouldNot().HaveNameEndingWith("Service").GetResult();

        var noRepo = Types.InAssembly(TestAssemblies.Domain)
            .That().AreClasses().And().AreNotAbstract()
            .ShouldNot().HaveNameEndingWith("Repository").GetResult();

        var noController = Types.InAssembly(TestAssemblies.Domain)
            .That().AreClasses().And().AreNotAbstract()
            .ShouldNot().HaveNameEndingWith("Controller").GetResult();

        Assert.Multiple(() =>
        {
            Assert.That(noService.IsSuccessful, Is.True, "Domain types must not end with 'Service'.");
            Assert.That(noRepo.IsSuccessful, Is.True, "Domain types must not end with 'Repository'.");
            Assert.That(noController.IsSuccessful, Is.True, "Domain types must not end with 'Controller'.");
        });
    }
}
