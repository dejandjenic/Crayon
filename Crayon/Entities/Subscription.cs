namespace Crayon.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime Expires { get; set; }
}