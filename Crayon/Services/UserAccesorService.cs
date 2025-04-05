using Crayon.Exceptions;

namespace Crayon.Services;

public interface IUserAccessorService
{
    Task<Guid> CurrentCustomer();
}

public class UserAccessorService(IHttpContextAccessor httpContextAccessor):IUserAccessorService
{
    public async Task<Guid> CurrentCustomer()
    {
        try
        {
            return Guid.Parse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CustomerIdClaimName)
                .Value);
        }
        catch
        {
            return Guid.Empty;
        }
    }
}