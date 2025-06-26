namespace Infrastructure.Minio;

public record MinioSettings(
    string Endpoint,
    string AccessKey,
    string SecretKey,
    string BucketName
);
