using Crayon.Exceptions;

namespace Crayon.Services;

public interface IUserAccessorService
{
    Task<Guid> CurrentCustomer();
}

public class UserAccessorService:IUserAccessorService
{
    public async Task<Guid> CurrentCustomer()
    {
        return Guid.Parse("8debd754-286d-4944-8fb5-1a48440f3848");
    }
}