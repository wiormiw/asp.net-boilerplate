using Application.Common.Contracts.File;

namespace Application.Common.Interfaces.Storage;

public interface IStorageService
{
    Task<string> UploadFileAsync(FileUploadRequest request);
}