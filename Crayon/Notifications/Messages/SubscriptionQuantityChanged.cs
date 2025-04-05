namespace Crayon.Notifications.Messages;

public class SubscriptionNotificationMessage
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public SubscriptionNotificationMessageType Type { get; set; }
}

public enum SubscriptionNotificationMessageType
{
    Created,
    ChangeQuantity,
    ChangeExpiration,
    Error,
    Cancel
}