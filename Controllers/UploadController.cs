using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;
using QualiTrack.DTOs;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController(AppDbContext db, IWebHostEnvironment env) : ControllerBase
{
    private readonly string[] _allowedTypes = ["image/jpeg", "image/png", "image/jpg", "application/pdf"];

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
            StoragePath = result,
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
            url = $"/uploads/{Path.GetFileName(result)}"
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
            StoragePath = result,
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
            url = $"/uploads/{Path.GetFileName(result)}"
        });
    }

    [HttpGet("finding/{findingId}")]
    public async Task<IActionResult> GetFindingFiles(Guid findingId)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var files = await db.EvidenceFiles
            .Where(e => e.FindingId == findingId)
            .Select( e => new
            {
                id = e.Id,
                fileName = e.FileName,
                url = $"{baseUrl}/uploads/{Path.GetFileName(e.StoragePath)}"
            })
            .ToListAsync();

        return Ok(files);
    }

    private async Task<string?> SaveFile(IFormFile file)
    {
        if (!_allowedTypes.Contains(file.ContentType)) return null;

        var uploadPath = Path.Combine(env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadPath);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }
}
