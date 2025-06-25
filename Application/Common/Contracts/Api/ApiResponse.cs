namespace Application.Common.Contracts.Api;

public class ApiResponse<T>
{
    public bool Success { get; init; } = true;
    public T Data { get; init; } = default!;
    public List<string>? Messages { get; init; }

    public static ApiResponse<T> SuccessResponse(T data, List<string>? messages = null)
        => new() { Data = data, Messages = messages };
}

public class ApiErrorResponse
{
    public bool Success { get; init; } = false;
    public int StatusCode { get; init; }
    public string? Type { get; init; }
    public List<string> Messages { get; init; } = [];
}