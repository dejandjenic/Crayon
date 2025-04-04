using System.Text.Json;
using Crayon.Exceptions;

namespace Crayon.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate requestDelegate,ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await requestDelegate(context);
        }
        catch (Exception ex)
        {
            await HandleException(ex,context);
        }
    }

    async Task HandleException(Exception ex,HttpContext context)
    {
        if (!(ex is BaseException crayonException))
            crayonException = new InternalException(ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)crayonException.Code;
        logger.LogError("An error occured",ex);
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { Errors = crayonException.Message }));
    }
}