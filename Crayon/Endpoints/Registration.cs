using Crayon.Endpoints.Model;
using Crayon.Services;
using O9d.AspNet.FluentValidation;

namespace Crayon.Endpoints;

public static class Registration
{
    public static void RegisterEndpoints(this WebApplication? app)
    {
        var root = app.MapGroup("").WithValidationFilter(options =>
            options.InvalidResultFactory = validationResult => Results.BadRequest())
            .RequireAuthorization();

        root.MapGet("/accounts",
                async (IAccountService service, IUserAccessorService userAccessorService) => 
                    await service.GetAccounts(await userAccessorService.CurrentCustomer()))
            .WithName("Accounts");

        root.MapGet("/accounts/{id}/subscriptions",
                async (Guid id, ISubscriptionService service, IUserAccessorService userAccessorService,
                    IAuthorizationService authorizationService) =>
                {
                    var idCustomer = await userAccessorService.CurrentCustomer();
                    await authorizationService.CheckAccountAccess(id,idCustomer);
                    return Results.Ok( await service.GetSubscriptions(id));
                })
            .WithName("AccountSubscriptions");

        root.MapGet("/accounts/{id}/subscriptions/{subscriptionId}/licences",
                async (Guid id, Guid subscriptionId, ISubscriptionService service,
                    IUserAccessorService userAccessorService,
                    IAuthorizationService authorizationService) =>
                {
                    var idCustomer = await userAccessorService.CurrentCustomer();
                    await authorizationService.CheckSubscriptionAccess(id,idCustomer,subscriptionId);
                    return Results.Ok(await service.GetLicences(subscriptionId));
                })
            .WithName("SubscriptionLicences");

        root.MapGet("/inventory",
            async (ICCPService service) => await service.GetInventory()).WithName("AvailableInventory");

        root.MapPost("/accounts/{id}/subscriptions",
                async (ISubscriptionService service,
                    IUserAccessorService userAccessorService,
                    Guid id,
                    [Validate] SubscriptionOrderRequest request,
                    IAuthorizationService authorizationService) =>
                {
                    var idCustomer = await userAccessorService.CurrentCustomer();
                    await authorizationService.CheckAccountAccess(id,idCustomer);
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
                Guid subscriptionId,Guid id,
                IAuthorizationService authorizationService, IUserAccessorService userAccessorService) =>
            {
                var idCustomer = await userAccessorService.CurrentCustomer();
                await authorizationService.CheckSubscriptionAccess(id,idCustomer,subscriptionId);
                await service.ChangeQuantity(subscriptionId, request.Quantity,id);
                return Results.Accepted();
            }).WithName("UpdateSubscriptionQuantity");

        root.MapPatch("/accounts/{id}/subscriptions/{subscriptionId}/expiration",
            async (ISubscriptionService service, Guid subscriptionId,Guid id,
                [Validate] SubscriptionChangeExpirationRequest request,
                IAuthorizationService authorizationService, IUserAccessorService userAccessorService) =>
            {
                var idCustomer = await userAccessorService.CurrentCustomer();
                await authorizationService.CheckSubscriptionAccess(id,idCustomer,subscriptionId);
                await service.SetExpiration(subscriptionId, request.Expires,id);
                return Results.Accepted();
            }).WithName("UpdateSubscriptionExpiration");

        root.MapDelete("/accounts/{id}/subscriptions/{subscriptionId}",
            async (ISubscriptionService service, Guid subscriptionId,Guid id,
                IAuthorizationService authorizationService, IUserAccessorService userAccessorService) =>
            {
                var idCustomer = await userAccessorService.CurrentCustomer();
                await authorizationService.CheckSubscriptionAccess(id,idCustomer,subscriptionId);
                await service.CancelSubscription(subscriptionId,id);
                return Results.Accepted();
            }).WithName("CancelSubscription");
    }
}