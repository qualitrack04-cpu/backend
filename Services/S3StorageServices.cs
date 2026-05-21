using Amazon.S3;
using Amazon.S3.Model;

namespace QualiTrack.Services;

public class S3StorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName;
    private readonly string _endpoint;

    public S3StorageService(IConfiguration config)
    {
        _bucketName = config["S3:BucketName"]!;
        _endpoint = config["S3:Endpoint"]!;

        var s3Config = new AmazonS3Config
        {
            ServiceURL = _endpoint,
            ForcePathStyle = true
        };

        _s3 = new AmazonS3Client(
            config["S3:AccessKeyId"] ?? Environment.GetEnvironmentVariable("RAILWAY_BUCKET_ACCESS_KEY_ID"),
            config["S3:SecretAccessKey"] ?? Environment.GetEnvironmentVariable("RAILWAY_BUCKET_SECRET_ACCESS_KEY"),
            s3Config
        );
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

        using var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = $"uploads/{fileName}",
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3.PutObjectAsync(request);

        return $"{_endpoint}/{_bucketName}/uploads/{fileName}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        var key = fileUrl.Replace($"{_endpoint}/{_bucketName}/", "");

        await _s3.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        });
    }
}