namespace Application.Common.Contracts.File;

public record FileUploadRequest(
    string FileName,
    string ContentType,
    Stream Content);