using System.Net.WebSockets;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;

namespace Crayon.Tests.Helpers;

public class SingalRUtility(TestServer server)
{
    private HubConnection? connection;
    private bool waiting = true;
    private int waitInterval = 0;
  
    public async Task Start()
    {
        connection = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl($"{server.BaseAddress}notification", options =>
            {
                options.SkipNegotiation = true;
                options.Transports = HttpTransportType.WebSockets;
                options.HttpMessageHandlerFactory = _ => server.CreateHandler();
                options.WebSocketFactory = (context, cancellationToken) =>
                {
                    var webSocketClient = server.CreateWebSocketClient();
                    var webSocketTask = webSocketClient.ConnectAsync(context.Uri, cancellationToken);
                    return new ValueTask<WebSocket>(webSocketTask);
                };
            })
            .Build();
     
        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 500);
            await connection.StartAsync();
        };


        await connection.StartAsync();
     
    }


    public void Handle(Action<object> handler)
    {
        connection!.On("OnEvent", handler);
    }


    public async Task Subscribe(object data)
    {
        await connection!.SendCoreAsync("subscribe", new[] { data });
    }


    public async Task Stop()
    {
        await connection!.StopAsync();
    }


    public async Task WaitFor(int waitPeriod = 30)
    {
        waitInterval = 0;
        while (waiting && waitInterval<waitPeriod)
        {
            await Task.Delay(1000);
            waitInterval++;
        }
    }


    public void WaitingDone()
    {
        waiting = false;
    }
}
