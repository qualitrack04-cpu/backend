using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;
using QualiTrack.DTOs;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FindingController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FindingStatus? status, [FromQuery] FindingCategory? category)
    {
        var query = db.Findings.Include(f => f.Capa).AsQueryable();
        if (status.HasValue) query = query.Where(f => f.Status == status);
        if (category.HasValue) query = query.Where(f => f.Category == category);
        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var finding = await db.Findings.Include(f => f.Capa).FirstOrDefaultAsync(f => f.Id == id);
        return finding is null ? NotFound() : Ok(finding);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Finding finding)
    {
        finding.Id = Guid.NewGuid();
        finding.FoundAt = DateTime.UtcNow;
        finding.Status = FindingStatus.Open;
        db.Findings.Add(finding);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = finding.Id }, finding);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] FindingStatus status)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();
        finding.Status = status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var finding = await db.Findings.FindAsync(id);
        if (finding is null) return NotFound();
        db.Findings.Remove(finding);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
