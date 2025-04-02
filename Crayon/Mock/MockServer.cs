using Crayon.ApiClients.CCPClient;
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
        
        // Catch-all case
        server
            .Given(Request.Create().WithPath("/api/*"))
            .AtPriority(100)
            .RespondWith(Response.Create().WithStatusCode(401));

        // Specific case
        server
            .Given(Request.Create().WithPath("/api/specific-resource"))
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200));
        
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
    }
}