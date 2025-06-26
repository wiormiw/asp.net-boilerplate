using Application.Common.Contracts.File;
using Application.Common.Interfaces.Storage;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Minio;

public class MinioService : IStorageService
{
    private readonly MinioSettings _settings;
    private readonly IMinioClient _minioClient;

    public MinioService(IOptions<MinioSettings> settings)
    {
        _settings = settings.Value;
        _minioClient = new MinioClient()
            .WithEndpoint(_settings.Endpoint.Replace("http://", "").Replace("https://", ""))
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.Endpoint.StartsWith("https"))
            .Build();
    }

    public async Task<string> UploadFileAsync(FileUploadRequest request)
    {
        // Ensure MinIO bucket exists
        bool bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_settings.BucketName));
        
        if (!bucketExists)
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_settings.BucketName));
        
        // Set bucket policy to public for default behaviour
        string policyJson = $@"
        {{
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {{
                    ""Effect"": ""Allow"",
                    ""Principal"": {{
                        ""AWS"": [""*""]
                    }},
                    ""Action"": [""s3:GetObject""],
                    ""Resource"": [""arn:aws:s3:::{_settings.BucketName}/*""]
                }}
            ]
        }}";
        
        await _minioClient.SetPolicyAsync(new  SetPolicyArgs()
            .WithBucket(_settings.BucketName)
            .WithPolicy(policyJson));

        var objectName = $"{Guid.NewGuid()}_{request.FileName}";
        
        // Upload the file to MinIO
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName)
            .WithStreamData(request.Content)
            .WithObjectSize(request.Content.Length)
            .WithContentType(request.ContentType));
        
        // Newly created object url
        return $"{_settings.Endpoint}/{_settings.BucketName}/{objectName}";
    }
}