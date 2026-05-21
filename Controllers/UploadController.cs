using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using QualiTrack.Data;
using QualiTrack.Models;

[ApiController]
[Route("api/[controller]")]
public class UploadController(AppDbContext db, IWebHostEnvironment env) : ControllerBase
{
    private readonly string[] _allowedTypes = ["image/jpeg", "image/png", "image/jpg", "application/pdf"];
    private const int MaxImageWidth = 1280;
    private const int JpegQuality = 75;

    [HttpPost("finding/{findingId}")]
    public async Task<IActionResult> UploadForFinding(Guid findingId, IFormFile file)
    {
        var finding = await db.Findings.FindAsync(findingId);
        if (finding is null) return NotFound("Finding tidak ditemukan");

        var result = await SaveFile(file);
        if (result is null) return BadRequest("Format file tidak didukung. Gunakan JPG, PNG, atau PDF");

        var evidence = new EvidenceFile
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            StoragePath = result.Value.path,
            ContentType = file.ContentType,
            FileSizeBytes = result.Value.size,
            UploadedAt = DateTime.UtcNow
        };

        db.EvidenceFiles.Add(evidence);
        await db.SaveChangesAsync();

        return Ok(new {
            fileId = evidence.Id,
            fileName = evidence.FileName,
            url = $"/uploads/{Path.GetFileName(result.Value.path)}",
            originalSize = file.Length,
            compressedSize = result.Value.size,
            savedPercent = Math.Round((1 - (double)result.Value.size / file.Length) * 100, 1)
        });
    }

    [HttpPost("capa-action/{actionId}")]
    public async Task<IActionResult> UploadForCapaAction(Guid actionId, IFormFile file)
    {
        var action = await db.CAPAActions.FindAsync(actionId);
        if (action is null) return NotFound("CAPA Action tidak ditemukan");

        var result = await SaveFile(file);
        if (result is null) return BadRequest("Format file tidak didukung. Gunakan JPG, PNG, atau PDF");

        var evidence = new EvidenceFile
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            StoragePath = result.Value.path,
            ContentType = file.ContentType,
            FileSizeBytes = result.Value.size,
            UploadedAt = DateTime.UtcNow,
            CapaActionId = actionId
        };

        db.EvidenceFiles.Add(evidence);
        await db.SaveChangesAsync();

        return Ok(new {
            fileId = evidence.Id,
            fileName = evidence.FileName,
            url = $"/uploads/{Path.GetFileName(result.Value.path)}",
            originalSize = file.Length,
            compressedSize = result.Value.size,
            savedPercent = Math.Round((1 - (double)result.Value.size / file.Length) * 100, 1)
        });
    }

    [HttpGet("finding/{findingId}")]
    public async Task<IActionResult> GetFindingFiles(Guid findingId)
    {
        var files = await db.EvidenceFiles
            .Where(e => e.AuditResponseId == null && e.CapaActionId == null)
            .ToListAsync();
        return Ok(files);
    }

    private async Task<(string path, long size)?> SaveFile(IFormFile file)
    {
        if (!_allowedTypes.Contains(file.ContentType)) return null;

        var uploadPath = Path.Combine(env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadPath);

        // PDF langsung simpan tanpa kompresi
        if (file.ContentType == "application/pdf")
        {
            var pdfFileName = $"{Guid.NewGuid()}.pdf";
            var pdfPath = Path.Combine(uploadPath, pdfFileName);
            using var pdfStream = new FileStream(pdfPath, FileMode.Create);
            await file.CopyToAsync(pdfStream);
            return (pdfPath, file.Length);
        }

        // Gambar — kompres dengan SkiaSharp
        var fileName = $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine(uploadPath, fileName);

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        using var original = SKBitmap.Decode(ms.ToArray());
        if (original is null) return null;

        // Resize kalau lebih lebar dari MaxImageWidth
        SKBitmap bitmap = original;
        if (original.Width > MaxImageWidth)
        {
            var ratio = (float)MaxImageWidth / original.Width;
            var newHeight = (int)(original.Height * ratio);
            bitmap = original.Resize(new SKImageInfo(MaxImageWidth, newHeight), SKFilterQuality.High);
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);
        using var output = new FileStream(filePath, FileMode.Create);
        data.SaveTo(output);

        if (bitmap != original) bitmap.Dispose();

        var fileInfo = new FileInfo(filePath);
        return (filePath, fileInfo.Length);
    }
}
