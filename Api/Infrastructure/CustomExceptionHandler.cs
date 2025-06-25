using Application.Common.Contracts.Api;
using Microsoft.AspNetCore.Diagnostics;
using Application.Common.Exceptions;

namespace Api.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _handlers;

    public CustomExceptionHandler()
    {
        _handlers = new()
        {
            { typeof(NotFoundException), HandleNotFound },
            { typeof(ValidationException), HandleValidationException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_handlers.ContainsKey(exceptionType))
        {
            await _handlers[exceptionType].Invoke(context, exception);
            return true;
        }
        
        return false;
    }
    
    private async Task HandleNotFound(HttpContext context, Exception ex)
    {
        var exception = (NotFoundException) ex;

        var error = new ApiErrorResponse
        {
            StatusCode = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Messages = new() { exception.Message, }
        };
            
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        
        await context.Response.WriteAsJsonAsync(error);
    }

    private async Task HandleValidationException(HttpContext context, Exception ex)
    {
        var exception = (ValidationException) ex;
        
        var allMessages = exception.Errors
            .SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"))
            .ToList();

        var error = new ApiErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Messages = allMessages
        };
        
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        await context.Response.WriteAsJsonAsync(error);
    }

    private async Task HandleUnauthorizedException(HttpContext context, Exception ex)
    {
        var error = new ApiErrorResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Messages = new() { "Unauthorized, please log in to access resources." }
        };
        
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        
        await context.Response.WriteAsJsonAsync(error);
    }

    private async Task HandleForbiddenAccessException(HttpContext context, Exception ex)
    {
        var error = new ApiErrorResponse
        {
            StatusCode = StatusCodes.Status403Forbidden,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Messages = new() { "Forbidden, you are not allowed to access resources." }
        };
        
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        
        await context.Response.WriteAsJsonAsync(error);
    }
}