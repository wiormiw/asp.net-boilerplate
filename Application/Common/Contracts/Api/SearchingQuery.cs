namespace Application.Common.Contracts.Api;

public record SearchingQuery
{
    public string? SearchBy { get; init; } = null;
    public string? SearchText { get; init; } = null;
    public string SortDirection { get; init; } = "desc";
}