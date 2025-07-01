namespace Application.Common.Contracts.Api;

public class ApiResponse<T>
{
    public bool Success { get; init; } = true;
    public int StatusCode { get; init; } = 200;
    public T Data { get; init; } = default!;
    public List<string>? Messages { get; init; }

    public static ApiResponse<T> SuccessResponse(T data, int statusCode = 200, List<string>? messages = null)
        => new() { StatusCode = statusCode, Data = data, Messages = messages };
}

public class ApiErrorResponse
{
    public bool Success { get; init; } = false;
    public int StatusCode { get; init; }
    public string? Type { get; init; }
    public List<string> Messages { get; init; } = [];
    
    public static ApiErrorResponse FromStatus(int statusCode, string?  type = null, params string[] messages)
        => new() { StatusCode = statusCode, Type = type, Messages = messages.ToList() };

    public static ApiErrorResponse FromValidation(int statusCode, string? type, IDictionary<string, string[]> errors)
    {
        var allMessages = errors
            .SelectMany(kvp => 
                kvp.Value.Select(msg => $"{kvp.Key}: {msg}")).ToList();

        return new ApiErrorResponse
        {
            StatusCode = statusCode,
            Type = type,
            Messages = allMessages
        };
    }
}