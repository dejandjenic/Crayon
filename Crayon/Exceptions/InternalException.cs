using System.Net;

namespace Crayon.Exceptions;

public class InternalException(string message) : BaseException(message)
{
    public override HttpStatusCode Code => HttpStatusCode.InternalServerError;
    
}