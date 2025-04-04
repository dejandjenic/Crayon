using Crayon.Endpoints.Model;
using Crayon.Services;
using O9d.AspNet.FluentValidation;

namespace Crayon.Endpoints;

public static class Registration
{
    public static void RegisterEndpoints(this WebApplication? app)
    {
        var root = app.MapGroup("").WithValidationFilter(options =>
            options.InvalidResultFactory = validationResult => Results.BadRequest());

        root.MapGet("/accounts",
                async (IAccountService service, IUserAccessorService userAccessorService) =>
                    await service.GetAccounts(await userAccessorService.CurrentCustomer()))
            .WithName("Accounts");

        root.MapGet("/accounts/{id}/subscriptions",
            async (Guid id, ISubscriptionService service, IUserAccessorService userAccessorService) =>
                await service.GetSubscriptions(id)).WithName("AccountSubscriptions");

        app.MapGet("/accounts/{id}/subscriptions/{subscriptionId}/licences",
                async (Guid id, Guid subscriptionId, ISubscriptionService service,
                    IUserAccessorService userAccessorService) => await service.GetLicences(subscriptionId))
            .WithName("SubscriptionLicences");

        root.MapGet("/inventory",
            async (ICCPService service) => await service.GetInventory()).WithName("AvailableInventory");

        root.MapPost("/accounts/{id}/subscriptions",
                async (ISubscriptionService service,
                    IUserAccessorService userAccessorService,
                    Guid id,
                    [Validate] SubscriptionOrderRequest request) =>
                {
                    var subscriptionId = await service.Order(await userAccessorService.CurrentCustomer(), id,
                        request.Id, request.Quantity);
                    return Results.Created($"/accounts/{id}/subscriptions/{subscriptionId}",
                        new SubscriptionOrderResponse
                        {
                            Id = subscriptionId
                        });
                })
            .WithName("OrderSubscription");

        root.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/quantity",
            async (ISubscriptionService service, [Validate] SubscriptionChangeQuantityRequest request,
                Guid subscriptionId) =>
            {
                await service.ChangeQuantity(subscriptionId, request.Quantity);
                return Results.Accepted();
            }).WithName("UpdateSubscriptionQuantity");

        root.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/expiration",
            async (ISubscriptionService service, Guid subscriptionId,
                [Validate] SubscriptionChangeExpirationRequest request) =>
            {
                await service.SetExpiration(subscriptionId, request.Expires);
                return Results.Accepted();
            }).WithName("UpdateSubscriptionExpiration");

        root.MapDelete("/accounts/{id}/subscriptions/{subscriptionId}",
            async (ISubscriptionService service, Guid subscriptionId) =>
            {
                await service.CancelSubscription(subscriptionId);
                return Results.Accepted();
            }).WithName("CancelSubscription");
    }
}