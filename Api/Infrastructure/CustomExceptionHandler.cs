using Application.Common.Contracts.Api;
using Microsoft.AspNetCore.Diagnostics;
using Application.Common.Exceptions;

namespace Api.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private static readonly Dictionary<Type, Func<HttpContext, Exception, Task>> Handlers = new()
    {
        [typeof(NotFoundException)] = HandleNotFoundAsync,
        [typeof(ValidationException)] = HandleValidationExceptionAsync,
        [typeof(ArgumentNullException)] = HandleArgumentNotNullExceptionAsync,
        [typeof(UnauthorizedAccessException)] = HandleUnauthorizedAccessExceptionAsync,
        [typeof(ForbiddenAccessException)] = HandleForbiddenAccessExceptionAsync,
    };

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (Handlers.TryGetValue(exceptionType, out var handler))
        {
            await handler(context, exception);
            return true;
        }
        
        return false;
    }
    
    private static async Task HandleNotFoundAsync(HttpContext context, Exception ex)
    {
        var exception = (NotFoundException) ex;
        
        var error = ApiErrorResponse.FromStatus(
            StatusCodes.Status404NotFound,
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            exception.Message
        );
            
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        
        await context.Response.WriteAsJsonAsync(error);
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, Exception ex)
    {
        var exception = (ValidationException) ex;
        
        var error = ApiErrorResponse.FromValidation(
            StatusCodes.Status400BadRequest,
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            exception.Errors
        );
        
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        await context.Response.WriteAsJsonAsync(error);
    }

    private static async Task HandleArgumentNotNullExceptionAsync(HttpContext context, Exception ex)
    {
        var exception = (ArgumentException) ex;

        var error = ApiErrorResponse.FromStatus
        (
            StatusCodes.Status400BadRequest,
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "There's empty required fields."
        );
        
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        await context.Response.WriteAsJsonAsync(error);
    }

    private static async Task HandleUnauthorizedAccessExceptionAsync(HttpContext context, Exception ex)
    {
        var error = ApiErrorResponse.FromStatus
        (
            StatusCodes.Status401Unauthorized,
            "https://tools.ietf.org/html/rfc7235#section-3.1",
            "Unauthorized, please log in to access resources."
        );
        
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        
        await context.Response.WriteAsJsonAsync(error);
    }

    private static async Task HandleForbiddenAccessExceptionAsync(HttpContext context, Exception ex)
    {
        var error = ApiErrorResponse.FromStatus
        (
            StatusCodes.Status403Forbidden,
            "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            "Forbidden, you are not allowed to access resources."
        );
        
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        
        await context.Response.WriteAsJsonAsync(error);
    }
}