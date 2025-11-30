using Dsw2025Tpi.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Dsw2025Tpi.Api.MiddleareCustoms;

public class ExceptionHandlerCustom
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerCustom> _logger;

    public ExceptionHandlerCustom(RequestDelegate next, ILogger<ExceptionHandlerCustom> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // pasa al siguiente middleware
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.GetType().Name}: {ex.Message}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            BadRequestException => HttpStatusCode.BadRequest,
            NoContentException=> HttpStatusCode.NoContent,
            EntityNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            DuplicatedEntityException=> HttpStatusCode.Conflict,
            InvalidStatusTransitionException=>HttpStatusCode.BadRequest,
            DataInsertException=>HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(exception.Message));
    }
}
