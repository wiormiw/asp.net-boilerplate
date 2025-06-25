namespace Application.Common.Contracts.Api;

public class Result
{
    public bool IsSuccess { get; init; }
    public string[] Errors { get; init; }

    internal Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToArray();
    }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Error(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}