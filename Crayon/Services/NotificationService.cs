using Crayon.Endpoints;
using Crayon.Helpers;
using Crayon.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Crayon.Services;

public interface INotificationService
{
    Task NotifyAccount(Guid id,object message);
}

public class NotificationService(IHubContext<NotificationHub, INotificationHubClient> hub) : INotificationService
{
    public async Task NotifyAccount(Guid id, object message)
    {
        var dict = new Dictionary<string, string>()
        {
            {
                "account",id.ToString()
            }
        };
        await hub.Clients.Group(dict.ToGroupName()).OnEvent(message);
    }
}