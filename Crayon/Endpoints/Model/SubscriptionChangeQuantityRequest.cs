using FluentValidation;

namespace Crayon.Endpoints.Model;

public class SubscriptionChangeQuantityRequest
{
    public int Quantity { get; set; }
}

public class SubscriptionChangeQuantityRequestValidator:AbstractValidator<SubscriptionChangeQuantityRequest>
{
    public SubscriptionChangeQuantityRequestValidator()
    {
        RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0);
    }
}
