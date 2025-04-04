using Crayon.Events.Base;

namespace Crayon.Helpers;

public static  class WebApplicationExtensions
{
    public static async Task StartSubscribers(this WebApplication? app)
    {
        
        var subscribers = app.Services.GetRequiredService<IEnumerable<ISubscriber>>();
        foreach (var subscriber in subscribers)
        {
            await subscriber.Start();
        }

    }
}