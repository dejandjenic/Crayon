using System.Net;

namespace Crayon.Exceptions;

public class BaseException(string message) : Exception
{
    public string Message { get; } = message;
    public virtual HttpStatusCode Code => HttpStatusCode.InternalServerError;
}

public class InternalException(string message) : BaseException(message)
{
    public override HttpStatusCode Code => HttpStatusCode.InternalServerError;
    
}