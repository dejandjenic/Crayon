namespace Crayon.Events.Handlers;

public class OrderMessage
{
    public Guid SubscriptionId { get; set; }
    public Guid ItemId { get; set; }
    public Guid AccountId { get; set; }
    public int Quantity { get; set; }
}

public class OrderPurchasedMessage
{
    public DateTime Expires { get; set; }
    public Guid Id { get; set; }
    public List<Guid> Licences { get; set; }
}

public class ChangeSubscriptionQuantityMessage
{
    public Guid SubscriptionId { get; set; }
    public int Quantity { get; set; }
}
public class ChangeSubscriptionQuantityFinalizeMessage
{
    public Guid SubscriptionId { get; set; }
    public List<Guid> Licences { get; set; }
    public string Status { get; set; }
    public string? Error { get; set; }
}

public class ChangeSubscriptionExpirationMessage
{
    public Guid SubscriptionId { get; set; }
    public DateTime Expires { get; set; }
}
public class ChangeSubscriptionExpirationFinalizeMessage
{
    public Guid SubscriptionId { get; set; }
    public string Status { get; set; }
    public string? Error { get; set; }
    public DateTime Expires { get; set; }
}


public class CancelSubscriptionMessage
{
    public Guid SubscriptionId { get; set; }
}
public class CancelSubscriptionFinalizeMessage
{
    public Guid SubscriptionId { get; set; }
    public string Status { get; set; }
    public string? Error { get; set; }
}