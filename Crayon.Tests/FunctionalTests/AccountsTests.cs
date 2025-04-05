using System.Net;
using System.Security.Claims;
using Crayon.Tests.Helpers;
using FluentAssertions;
using TestProject1;

namespace Crayon.Tests.FunctionalTests;

[Collection("Database collection")]
public class AccountsTests : IClassFixture<TestRunFixture>
{
    private TestJWTLibrary.Generator generator = new();
    private readonly ContainerFixture _testbase;
    private TestRunFixture _run;

    public AccountsTests(ContainerFixture containerFixture,TestRunFixture runFixture)
    {
        _testbase = containerFixture;
        DatabaseInitializer.Initialize(_testbase.GetConnectionString("db"),runFixture.Id);
        _run = runFixture;
        _run.SetConnectionStrings(containerFixture);
    }

    [Fact]
    async Task Should_Be_Authenticated(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);

        var (code,_) = await helper.Accounts();
        code.Should().Be(HttpStatusCode.Unauthorized);

    }
    
    [Fact]
    async Task Should_Return_Accounts(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);
        helper.Authorize(generator.GenerateJwt(additionalClaims:new Claim("customer_id","8debd754-286d-4944-8fb5-1a48440f3848")));
        var (code,accounts) = await helper.Accounts();
        code.Should().Be(HttpStatusCode.OK);
        accounts.Count.Should().Be(2);

    }
}