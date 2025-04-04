using System.Text.Json;
using Crayon.ApiClients.CCPClient;
using Crayon.ApiClients.CCPClient.Model;
using Crayon.Endpoints.Model;
using WireMock.RequestBuilders;
using WireMock.Server;
using WireMock.ResponseBuilders;

namespace Crayon.Mock;


public class MockServer
{
    public static void Start()
    {
        var server = WireMockServer.Start(settings =>
        {
            settings.Port = 7777;
        });
        
        server
            .Given(Request.Create().WithPath("/inventory"))
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(
                new List<InventoryItem>()
                {
                    new()
                    {
                        Id = Guid.Parse( "69048a5a-fb5f-4a65-b209-557c5ce4cbf7"),
                        Name = "Microsoft Office"
                    },
                    new()
                    {
                        Id = Guid.Parse("a2bca648-d138-4072-b8c9-ccbcfd86df96"),
                        Name = "Microsoft SQL Server"
                    }
                }
                ));
        
        
        server
            .Given(Request.Create().WithPath("/subscriptions").UsingPost())
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(async (m) =>
                {
                    var quantity = JsonSerializer.Deserialize<PurchaseRequest>(m.Body,
                        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).Quantity;
                    return new
                    {
                        Expires = DateTime.UtcNow.AddYears(1),
                        Licences = Enumerable.Range(1, quantity).Select(x => Guid.NewGuid()).ToList()
                    };
                }
            ));
        
        server
            .Given(Request.Create().WithPath("/subscriptions/*").UsingDelete())
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200));
        
        server
            .Given(Request.Create().WithPath("/subscriptions/*/quantity").UsingPatch())
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(async (m) =>
                {
                    var quantity = JsonSerializer.Deserialize<SubscriptionChangeQuantityRequest>(m.Body,
                        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).Quantity;
                    return new
                    {
                        Licences = Enumerable.Range(1, quantity).Select(x => Guid.NewGuid()).ToList()
                    };
                }
            ));
        
        server
            .Given(Request.Create().WithPath("/subscriptions/*/expiration").UsingPatch())
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200));

    }
}