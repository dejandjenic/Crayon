using Crayon.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace Crayon.Notifications;

public interface INotificationHubClient
{
    Task OnEvent(object data);
}


public class NotificationHub : Hub<INotificationHubClient>
{
    public async Task Subscribe(Dictionary<string,string> data)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId,data.ToGroupName());
    }
  
    public async Task UnSubscribe(Dictionary<string,string> data)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId,data.ToGroupName());
    }
}