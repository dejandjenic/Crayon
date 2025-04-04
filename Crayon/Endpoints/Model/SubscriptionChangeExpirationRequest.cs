using FluentValidation;

namespace Crayon.Endpoints.Model;

public class SubscriptionChangeExpirationRequest
{
    public DateTime Expires { get; set; }
}

public class SubscriptionChangeExpirationRequestValidator:AbstractValidator<SubscriptionChangeExpirationRequest>
{
    public SubscriptionChangeExpirationRequestValidator()
    {
        RuleFor(x => x.Expires).NotEmpty().GreaterThan(DateTime.UtcNow);
    }
}