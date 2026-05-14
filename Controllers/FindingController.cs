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
    [Authorize(Roles = "Admin,QualityManager,Auditor,auditee")]
    public async Task<IActionResult> GetAll([FromQuery] FindingStatus? status, [FromQuery] FindingCategory? category)
    {
        var query = db.Findings.Include(f => f.Capa).AsQueryable();
        if (status.HasValue) query = query.Where(f => f.Status == status);
        if (category.HasValue) query = query.Where(f => f.Category == category);
        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,auditee")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var finding = await db.Findings.Include(f => f.Capa).FirstOrDefaultAsync(f => f.Id == id);
        return finding is null ? NotFound() : Ok(finding);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> Create([FromBody] CreateFindingRequest req)
    {
        var finding = new Finding
        {
            Id = Guid.NewGuid(),
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
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateFindingRequest req)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();

        finding.Category = req.Category!.Value;
        finding.Description = req.Description;
        finding.ClauseRef = req.ClauseRef;
        
        await db.SaveChangesAsync();
        return Ok(finding);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] FindingStatus status)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();
        finding.Status = status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();
        db.Findings.Remove(finding);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
