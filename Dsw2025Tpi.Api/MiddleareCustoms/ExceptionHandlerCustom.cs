using Dsw2025Tpi.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Dsw2025Tpi.Api.MiddleareCustoms;

public class ExceptionHandlerCustom
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlerCustom(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // pasa al siguiente middleware
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            BadRequestException => HttpStatusCode.BadRequest,
            EntityNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            DuplicatedEntityException=> HttpStatusCode.Conflict,
            InvalidStatusTransitionException=>HttpStatusCode.BadRequest,
            DataInsertException=>HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(exception.Message));
    }
}
