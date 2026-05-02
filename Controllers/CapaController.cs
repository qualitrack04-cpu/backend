using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;
using QualiTrack.DTOs;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CapaController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CAPAStatus? status)
    {
        var query = db.CAPAs
            .Include(c => c.Actions)
            .Include(c => c.CloseOut)
            .AsQueryable();
        if (status.HasValue) query = query.Where(c => c.Status == status);
        return Ok(await query.ToListAsync());
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var overdue = await db.CAPAs
            .Where(c => c.Deadline < today && c.Status != CAPAStatus.Closed)
            .Include(c => c.Actions)
            .ToListAsync();
        return Ok(overdue);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var capa = await db.CAPAs
            .Include(c => c.Actions)
            .Include(c => c.CloseOut)
            .FirstOrDefaultAsync(c => c.Id == id);
        return capa is null ? NotFound() : Ok(capa);
    }

    [HttpPost("finding/{findingId}")]
    public async Task<IActionResult> Create(Guid findingId, CAPA capa)
    {
        var finding = await db.Findings.FindAsync(findingId);
        if (finding is null) return NotFound("Finding tidak ditemukan");
        capa.Id = Guid.NewGuid();
        capa.FindingId = findingId;
        capa.Status = CAPAStatus.Open;
        capa.CreatedAt = DateTime.UtcNow;
        db.CAPAs.Add(capa);
        finding.Status = FindingStatus.InProgress;
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = capa.Id }, capa);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCapaRequest req)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        capa.RootCause = req.RootCause;
        capa.CorrectiveAction = req.CorrectiveAction;
        capa.PreventiveAction = req.PreventiveAction;
        capa.Deadline = req.Deadline;
        if (req.PicId.HasValue && req.PicId != Guid.Empty)
            capa.PicId = req.PicId!.Value;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CAPAStatus status)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        capa.Status = status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/actions")]
    public async Task<IActionResult> AddAction(Guid id, CAPAAction action)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        action.Id = Guid.NewGuid();
        action.CapaId = id;
        action.DoneAt = DateTime.UtcNow;
        db.CAPAActions.Add(action);
        capa.Status = CAPAStatus.InProgress;
        await db.SaveChangesAsync();
        return Ok(action);
    }

    [HttpPost("{id}/closeout")]
    public async Task<IActionResult> CloseOut(Guid id, CloseOutVerification verification)
    {
        var capa = await db.CAPAs.Include(c => c.Finding).FirstOrDefaultAsync(c => c.Id == id);
        if (capa is null) return NotFound();
        verification.Id = Guid.NewGuid();
        verification.CapaId = id;
        verification.VerifiedAt = DateTime.UtcNow;
        db.CloseOutVerifications.Add(verification);
        capa.Status = CAPAStatus.Closed;
        if (capa.Finding is not null)
            capa.Finding.Status = FindingStatus.Closed;
        await db.SaveChangesAsync();
        return Ok(verification);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        db.CAPAs.Remove(capa);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
