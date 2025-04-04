using System.Net;
using Crayon.ApiClients.CCPClient;
using Crayon.ApiClients.CCPClient.Model;
using Crayon.Repositories;
using FluentAssertions;
using TestProject1.Helpers;

namespace TestProject1;

public class FlowTest : TestBase
{
    [Fact]
    async Task Test()
    {
        Guid officeId = Guid.Parse("69048a5a-fb5f-4a65-b209-557c5ce4cbf7");
        Guid sqlId = Guid.Parse("a2bca648-d138-4072-b8c9-ccbcfd86df96");
        string officeName = "Microsoft Office";
        string sqlName = "Microsoft SQL Server";
        var firstAccountId = Guid.Parse("be386f83-0f8e-11f0-95c6-34f39a52020b");
        var secondAccountId = Guid.Parse("de386f83-0f8e-11f0-95c6-34f39a52020b");

        string ACTIVE = "Active";
        
        using var helper = new RestHelper(AdjustSettings);

        var (_,inventory) = await helper.Inventory();
        
        ValidateInventory(inventory, officeId, officeName, sqlId, sqlName);

        var (_,accounts) = await helper.Accounts();

        ValidateAccounts(accounts, firstAccountId, secondAccountId);

        var (_,subscriptions) = await helper.Subscriptions(secondAccountId);
        
        subscriptions.Count.Should().Be(2);

        ValidateSubscriptions(subscriptions, officeName, ACTIVE, sqlName);

        var (_,licences) = await helper.Licences(accounts.Second().Id, Guid.Parse("d3f91708-0f92-11f0-95c6-34f39a52020b"));
        
        ValidateLicences(licences);

        var subscriptionExpirationDate = DateTime.UtcNow.AddYears(1);
        
        var (code, response) = await helper.CreateSubscription(firstAccountId, 1, officeId);
        code.Should().Be(HttpStatusCode.Created);
        
        var (_,subscriptionsNew) = await helper.Subscriptions(firstAccountId);
        
        subscriptionsNew.Count.Should().Be(1);
        
        ValidateSubscription(subscriptionsNew.First(), response.Id, officeName, "Created", DateTime.MinValue);


        await DefaultDelay();
        
        var (_,subscriptionsAfterCreation) = await helper.Subscriptions(firstAccountId);
        
        subscriptionsAfterCreation.Count.Should().Be(1);
        ValidateSubscription(subscriptionsAfterCreation.First(), response.Id, officeName, ACTIVE, subscriptionExpirationDate);
        
        var (licencesBeforeCode,licencesBeforeQuantity) = await helper.Licences(accounts.First().Id, response.Id);
        licencesBeforeCode.Should().Be(HttpStatusCode.OK);
        licencesBeforeQuantity.Count.Should().Be(1);
        

        var quantityResponse = await helper.ChangeSubscriptionQuantity(firstAccountId, response.Id, 2);
        quantityResponse.Should().Be(HttpStatusCode.Accepted);
        
        await DefaultDelay();
        
        var (_,subscriptionsQuantity) = await helper.Subscriptions(firstAccountId);
        
        subscriptionsQuantity.Count.Should().Be(1);
        
        ValidateSubscription(subscriptionsQuantity.First(),response.Id,officeName,ACTIVE,subscriptionExpirationDate);
        
        var (licencesCode,licencesQuantity) = await helper.Licences(accounts.First().Id, response.Id);
        licencesCode.Should().Be(HttpStatusCode.OK);

        ValidateLicenceChange(licencesBeforeQuantity, licencesQuantity, 2);

        var expDate = DateTime.UtcNow.AddYears(2);
        var expirationResponse = await helper.ChangeSubscriptionExpiration(firstAccountId, response.Id, expDate);
        expirationResponse.Should().Be(HttpStatusCode.Accepted);
        await DefaultDelay();
        
        var (_,subscriptionsAfterSetExpiration) = await helper.Subscriptions(firstAccountId);
        ValidateSubscription(subscriptionsAfterSetExpiration.First(),response.Id,officeName,ACTIVE,expDate);
        
        var cancelResponse = await helper.CancelSubscription(firstAccountId, response.Id);
        cancelResponse.Should().Be(HttpStatusCode.Accepted);
        await DefaultDelay();
        
        var (_,subscriptionsCancel) = await helper.Subscriptions(firstAccountId);
        ValidateSubscription(subscriptionsCancel.First(),response.Id,officeName,"Cancelled",expDate);

        await StopAllContainers();
    }

    private void ValidateLicences(List<Licence> licences)
    {
        licences.Count.Should().Be(2);

        ValidateLicence(licences.First(), Guid.Parse("17efb216-0f93-11f0-95c6-34f39a52020b"),
            "a3f91708-0f92-11f0-95c6-34f39a52020b");
        ValidateLicence(licences.Second(), Guid.Parse("1802829d-0f93-11f0-95c6-34f39a52020b"),
            "b3f91708-0f92-11f0-95c6-34f39a52020b");
    }

    private void ValidateSubscriptions(List<Subscription> subscriptions, string officeName, string ACTIVE, string sqlName)
    {
        ValidateSubscription(subscriptions.First(), Guid.Parse("d3f91708-0f92-11f0-95c6-34f39a52020b"),
            officeName, ACTIVE, DateTime.Parse("2025-10-10"));

        ValidateSubscription(subscriptions.Second(), Guid.Parse("d5d68be0-0f92-11f0-95c6-34f39a52020b"),
            sqlName, ACTIVE, DateTime.Parse("2025-11-11"));
    }

    private void ValidateAccounts(List<Account> accounts, Guid firstAccountId, Guid secondAccountId)
    {
        accounts.Count.Should().Be(2);
        
        ValidateAccount(accounts.First(),firstAccountId,"Department1");
        ValidateAccount(accounts.Second(),secondAccountId,"Department2");
    }

    private void ValidateInventory(List<InventoryItem> inventory, Guid officeId, string officeName, Guid sqlId, string sqlName)
    {
        inventory.Count.Should().Be(2);
        
        ValidateInventory(inventory.First(),officeId,officeName);
        ValidateInventory(inventory.Second(),sqlId,sqlName);
    }

    async Task DefaultDelay()
    {
        await Task.Delay(5000);
    }
    void ValidateSubscription(Subscription subscription,Guid id,string name,string status,DateTime expiration)
    {
        subscription.Id.Should().Be(id);
        subscription.Name.Should().Be(name);
        subscription.Status.Should().Be(status);
        subscription.Expires.ToString("yyyy-MM-dd").Should().Be(expiration.ToString("yyyy-MM-dd"));
    }
    
    void ValidateAccount(Account account,Guid id,string name)
    {
        account.Id.Should().Be(id);
        account.Name.Should().Be(name);
    }
    
    void ValidateInventory(InventoryItem inventory,Guid id,string name)
    {
        inventory.Id.Should().Be(id);
        inventory.Name.Should().Be(name);
    }
    
    void ValidateLicence(Licence licence,Guid id,string name)
    {
        licence.Id.Should().Be(id);
        licence.Value.Should().Be(name);
    }
    
    void ValidateLicenceChange(List<Licence> licence1,List<Licence> licence2,int licence2Count)
    {
        licence2.Count.Should().Be(licence2Count);

        foreach (var licence in licence1)
        {
            licence2.Any(x => x.Id == licence.Id).Should().BeFalse();
        }
    }
}