namespace QualiTrack.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    public LocalStorageService(IConfiguration config, IWebHostEnvironment env)
    {
        _uploadPath = Path.Combine(env.ContentRootPath, "Uploads");
        _baseUrl = config["App:BaseUrl"] ?? "http://localhost:5144";
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(_uploadPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"{_baseUrl}/uploads/{fileName}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_uploadPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}