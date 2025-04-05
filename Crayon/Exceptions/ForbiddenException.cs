using System.Net;

namespace Crayon.Exceptions;

public class ForbiddenException(string message) : BaseException(message)
{
    public override HttpStatusCode Code => HttpStatusCode.Forbidden;
    
}