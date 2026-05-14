using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.DTOs;
using QualiTrack.Filters;
using QualiTrack.Models;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ValidateModelAttribute]
public class FindingController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> GetAll(
        [FromQuery] FindingStatus? status,
        [FromQuery] FindingCategory? category,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var query = db.Findings.AsQueryable();
        if (status.HasValue) query = query.Where(f => f.Status == status);
        if (category.HasValue) query = query.Where(f => f.Category == category);
        if (from.HasValue) query = query.Where(f => f.FoundAt >= from.Value);
        if (to.HasValue) query = query.Where(f => f.FoundAt <= to.Value);
        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var finding = await db.Findings.FirstOrDefaultAsync(f => f.Id == id);
        return finding is null ? NotFound() : Ok(finding);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,QualityManager,Auditor")]
    public async Task<IActionResult> Create([FromBody] CreateFindingRequest req)
    {
        var finding = new Finding
        {
            Id = Guid.NewGuid(),
            Title = req.Title,
            Department = req.Department,
            SessionId = req.SessionId,
            Category = req.Category!.Value,
            Description = req.Description,
            ClauseRef = req.ClauseRef,
            FoundAt = DateTime.UtcNow,
            Status = FindingStatus.Open
        };
        db.Findings.Add(finding);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = finding.Id }, finding);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,QualityManager,Auditor")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFindingRequest req)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();
        finding.Title = req.Title;
        finding.Department = req.Department;
        if (req.Category.HasValue) finding.Category = req.Category.Value;
        finding.Description = req.Description;
        finding.ClauseRef = req.ClauseRef;
        await db.SaveChangesAsync();
        return Ok(finding);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,QualityManager")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] FindingStatus status)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();
        finding.Status = status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();
        db.Findings.Remove(finding);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
