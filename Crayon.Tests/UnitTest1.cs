using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace TestProject1;

public class UnitTest1 : TestBase
{
    [Fact]
    async Task Test()
    {
        using var helper = new RestHelper(AdjustSettings);

        var (_,inventory) = await helper.Inventory();
        
        inventory.Count.Should().Be(2);
        
        inventory.FirstOrDefault().Id.Should().Be(Guid.Parse("69048a5a-fb5f-4a65-b209-557c5ce4cbf7"));
        inventory.FirstOrDefault().Name.Should().Be("Microsoft Office");
        
        inventory.Skip(1).FirstOrDefault().Id.Should().Be(Guid.Parse("a2bca648-d138-4072-b8c9-ccbcfd86df96"));
        inventory.Skip(1).FirstOrDefault().Name.Should().Be("Microsoft SQL Server");

        
        var (_,accounts) = await helper.Accounts();

        accounts.Count.Should().Be(2);
        
        accounts.FirstOrDefault().Id.Should().Be(Guid.Parse("be386f83-0f8e-11f0-95c6-34f39a52020b"));
        accounts.FirstOrDefault().Name.Should().Be("Department1");
        
        accounts.Skip(1).FirstOrDefault().Id.Should().Be(Guid.Parse("de386f83-0f8e-11f0-95c6-34f39a52020b"));
        accounts.Skip(1).FirstOrDefault().Name.Should().Be("Department2");


        var (_,subscriptions) = await helper.Subscriptions("de386f83-0f8e-11f0-95c6-34f39a52020b");
        
        subscriptions.Count.Should().Be(2);
        
        subscriptions.FirstOrDefault().Id.Should().Be(Guid.Parse("d3f91708-0f92-11f0-95c6-34f39a52020b"));
        subscriptions.FirstOrDefault().Name.Should().Be("Microsoft Office");
        
        subscriptions.Skip(1).FirstOrDefault().Id.Should().Be(Guid.Parse("d5d68be0-0f92-11f0-95c6-34f39a52020b"));
        subscriptions.Skip(1).FirstOrDefault().Name.Should().Be("Microsoft SQL Server");

        var (_,licences) = await helper.Licences("de386f83-0f8e-11f0-95c6-34f39a52020b", "d3f91708-0f92-11f0-95c6-34f39a52020b");
        
        licences.Count.Should().Be(2);
        
        licences.FirstOrDefault().Id.Should().Be(Guid.Parse("17efb216-0f93-11f0-95c6-34f39a52020b"));
        licences.FirstOrDefault().Value.Should().Be("a3f91708-0f92-11f0-95c6-34f39a52020b");
        
        licences.Skip(1).FirstOrDefault().Id.Should().Be(Guid.Parse("1802829d-0f93-11f0-95c6-34f39a52020b"));
        licences.Skip(1).FirstOrDefault().Value.Should().Be("b3f91708-0f92-11f0-95c6-34f39a52020b");

        await StopAllContainers();
    }
   
}