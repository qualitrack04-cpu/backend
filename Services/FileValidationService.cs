namespace QualiTrack.Services;

public class FileValidationService
{
    private static readonly HashSet<string> AllowedExtensions = new(
        StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".pdf"
    };
    
    public (bool IsValid, string ErrorMessage) Validate(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, "File tidak boleh kosong.");
        
        const long maxSize = 10 * 1024 * 1024;
        if (file.Length > maxSize)
            return (false, $"Ukuran file maksimal 10 MB. File Anda {file.Length / (1024 * 1024)} MB.");
        
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            return (false, $"Ekstensi file tidak diizinkan. Gunakan: {string.Join(", ", AllowedExtensions)}.");
        
        return (true, string.Empty);
    }
    
    public static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        return $"{Guid.NewGuid():N}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
    }
}
