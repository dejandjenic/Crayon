using System.Net;

namespace Crayon.Exceptions;

public class BaseException(string message) : Exception(message)
{
    public virtual HttpStatusCode Code => HttpStatusCode.InternalServerError;
}