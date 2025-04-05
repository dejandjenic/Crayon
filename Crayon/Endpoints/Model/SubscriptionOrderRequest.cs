using FluentValidation;

namespace Crayon.Endpoints.Model;

public class SubscriptionOrderRequest
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}

public class SubscriptionOrderRequestValidator : AbstractValidator<SubscriptionOrderRequest>
{
    public SubscriptionOrderRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0);
    }
}