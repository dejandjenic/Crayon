using Crayon.Events.Base;
using RabbitMQ.Client;

namespace Crayon.Events.Handlers;

public class TempMessage
{
    public string Id { get; set; }
}

public class TempHandler(ConnectionFactory factory) : Subscriber<TempMessage>(factory)
{
    public override string Topic { get; set; } = Constants.PurchaseTopic;
    public override string BindingKey { get; set; } = "#";

    public async override Task<bool> HandleData(TempMessage data)
    {
        return true;
    }
}