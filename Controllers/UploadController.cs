using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;
using QualiTrack.Services;

[ApiController]
[Route("api/[controller]")]
public class UploadController(AppDbContext db, IStorageService storage ) : ControllerBase
{
    private readonly string[] _allowedTypes = ["image/jpeg", "image/png", "image/jpg", "application/pdf"];
    private const int MaxImageWidth = 1280;
    private const int JpegQuality = 75;

    [HttpPost("finding/{findingId}")]
    public async Task<IActionResult> UploadForFinding(Guid findingId, IFormFile file)
    {
        var finding = await db.Findings.FindAsync(findingId);
        if (finding is null) return NotFound("Finding tidak ditemukan");

        var url = await storage.UploadFileAsync(file);
        if (url is null) return BadRequest("Format file tidak didukung. Gunakan JPG, PNG, atau PDF");

        var evidence = new EvidenceFile
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            StoragePath = url,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            UploadedAt = DateTime.UtcNow,
            FindingId = findingId
        };

        db.EvidenceFiles.Add(evidence);
        await db.SaveChangesAsync();

        return Ok(new {
            fileId = evidence.Id,
            fileName = evidence.FileName,
            url = url
        });
    }

    [HttpPost("capa-action/{actionId}")]
    public async Task<IActionResult> UploadForCapaAction(Guid actionId, IFormFile file)
    {
        var action = await db.CAPAActions.FindAsync(actionId);
        if (action is null) return NotFound("CAPA Action tidak ditemukan");

        var url = await storage.UploadFileAsync(file);
        if (url is null) return BadRequest("Format file tidak didukung. Gunakan JPG, PNG, atau PDF");

        var evidence = new EvidenceFile
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            StoragePath = url,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            UploadedAt = DateTime.UtcNow,
            CapaActionId = actionId
        };

        db.EvidenceFiles.Add(evidence);
        await db.SaveChangesAsync();

        return Ok(new {
            fileId = evidence.Id,
            fileName = evidence.FileName,
            url = url,
            originalSize = file.Length,
            compressedSize = file.Length,
            savedPercent = 0
        });
    }

    [HttpGet("finding/{findingId}")]
    public async Task<IActionResult> GetFindingFiles(Guid findingId)
    {
        var files = await db.EvidenceFiles
            .Where(e => e.FindingId == findingId)
            .Select(e => new
            {
                id = e.Id,
                fileName = e.FileName,
                url = e.StoragePath,
            })
            .ToListAsync();

        return Ok(files);
    }

    [HttpDelete("{fileId}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid fileId)
    {
        var evidence = await db.EvidenceFiles.FindAsync(fileId);
        if (evidence is null) return NotFound(new { message = "File tidak ditemukan" });

        if (!string.IsNullOrEmpty(evidence.StoragePath))
            await storage.DeleteFileAsync(evidence.StoragePath);

        db.EvidenceFiles.Remove(evidence);
        await db.SaveChangesAsync();

        return Ok(new { message = "File berhasil dihapus", fileId = fileId });
    }

    [HttpGet("file/{fileId}")]
    [Authorize]
    public async Task<IActionResult> GetFileById(Guid fileId)
    {
        var evidence = await db.EvidenceFiles.FindAsync(fileId);
        if(evidence is null) return NotFound(new { message = "File tidak ditemukan"});
        return Ok(new
        {
            fileId = evidence.Id,
            fileName = evidence.FileName,
            url = evidence.StoragePath,
            ContentType = evidence.ContentType,
            fileSizeBytes = evidence.FileSizeBytes,
            uploadedAt = evidence.UploadedAt
        });
    }
}
