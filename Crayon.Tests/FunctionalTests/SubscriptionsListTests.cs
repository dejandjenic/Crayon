using System.Net;
using System.Security.Claims;
using Crayon.Tests.Helpers;
using FluentAssertions;
using TestProject1;

namespace Crayon.Tests.FunctionalTests;

[Collection("Database collection")]
public class SubscriptionsListTests : IClassFixture<TestRunFixture>
{
    private TestJWTLibrary.Generator generator = new();
    private readonly ContainerFixture _testbase;
    private TestRunFixture _run;

    public SubscriptionsListTests(ContainerFixture containerFixture,TestRunFixture runFixture)
    {
        _testbase = containerFixture;
        DatabaseInitializer.Initialize(_testbase.GetConnectionString(ContainerType.Db),runFixture.Id);
        _run = runFixture;
        _run.SetConnectionStrings(containerFixture);
    }

    Guid accountId = Guid.Parse("de386f83-0f8e-11f0-95c6-34f39a52020b");
    
    [Fact]
    async Task Should_Be_Authenticated(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);

        var (code,_) = await helper.Subscriptions(accountId);
        code.Should().Be(HttpStatusCode.Unauthorized);

    }
    
    [Fact]
    async Task Should_Return_Subscriptions(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);
        helper.Authorize(generator.GenerateJwt(additionalClaims:new Claim(Constants.CustomerIdClaimName,Constants.FirstCustomerId)));
        var (code,subscriptions) = await helper.Subscriptions(accountId);
        code.Should().Be(HttpStatusCode.OK);
        subscriptions.Count.Should().Be(2);

    }
    
    [Fact]
    async Task Should_Be_Authorized(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);
        helper.Authorize(generator.GenerateJwt(additionalClaims:new Claim(Constants.CustomerIdClaimName,"7debd754-286d-4944-8fb5-1a48440f3848")));
        var (code,_) = await helper.Subscriptions(accountId);
        code.Should().Be(HttpStatusCode.Forbidden);

    }
}