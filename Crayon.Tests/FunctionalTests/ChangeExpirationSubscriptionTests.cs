using System.Net;
using System.Security.Claims;
using Crayon.Tests.Helpers;
using FluentAssertions;
using TestProject1;

namespace Crayon.Tests.FunctionalTests;

[Collection("Database collection")]
public class ChangeExpirationSubscriptionTests : IClassFixture<TestRunFixture>
{
    private TestJWTLibrary.Generator generator = new();
    private readonly ContainerFixture _testbase;
    private TestRunFixture _run;

    public ChangeExpirationSubscriptionTests(ContainerFixture containerFixture,TestRunFixture runFixture)
    {
        _testbase = containerFixture;
        DatabaseInitializer.Initialize(_testbase.GetConnectionString("db"),runFixture.Id);
        _run = runFixture;
        _run.SetConnectionStrings(containerFixture);
    }

    Guid accountId = Guid.Parse("de386f83-0f8e-11f0-95c6-34f39a52020b");
    Guid subscriptionId = Guid.Parse("d3f91708-0f92-11f0-95c6-34f39a52020b");
    
    [Fact]
    async Task Should_Be_Authenticated(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);

        var code = await helper.ChangeSubscriptionExpiration(accountId,subscriptionId,DateTime.UtcNow);
        code.Should().Be(HttpStatusCode.Unauthorized);

    }
    
    [Fact]
    async Task Should_Handle_Invalid_Input(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);
        helper.Authorize(generator.GenerateJwt(additionalClaims:new Claim("customer_id","8debd754-286d-4944-8fb5-1a48440f3848")));
        var code = await helper.ChangeSubscriptionExpiration(accountId,subscriptionId,DateTime.UtcNow);
        code.Should().Be(HttpStatusCode.BadRequest);

    }
    
    [Fact]
    async Task Should_Be_Authorized(){
        using var helper = new RestHelper(_run.AdjustApplicationSettings,generator);
        helper.Authorize(generator.GenerateJwt(additionalClaims:new Claim("customer_id","7debd754-286d-4944-8fb5-1a48440f3848")));
        var code = await helper.ChangeSubscriptionExpiration(accountId,subscriptionId,DateTime.UtcNow.AddDays(5));
        code.Should().Be(HttpStatusCode.Forbidden);

    }
}