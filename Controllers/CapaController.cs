using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;
using QualiTrack.DTOs;
using QualiTrack.Filters;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ValidateModelAttribute]
public class CapaController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
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
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
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
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var capa = await db.CAPAs
            .Include(c => c.Actions)
            .Include(c => c.CloseOut)
            .FirstOrDefaultAsync(c => c.Id == id);
        return capa is null ? NotFound() : Ok(capa);
    }

    [HttpPost("finding/{findingId}")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> Create(Guid findingId, [FromBody] CreateCapaRequest req)
    {
        var finding = await db.Findings.FindAsync(findingId);
        if (finding is null) return NotFound("Finding tidak ditemukan");

        if (!req.Deadline.HasValue)
            return BadRequest(new { message = "Deadline wajib diisi" });

        if (!req.PicId.HasValue || req.PicId == Guid.Empty)
        {
            if (string.IsNullOrEmpty(req.PicName))
                return BadRequest(new { message = "PIC harus diisi dengan PicId atau PicName" });

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.FullName == req.PicName);
            if (user is null)
                return BadRequest(new { message = "PIC tidak ditemukan berdasarkan nama" });

            req.PicId = user.Id;
        }

        var capa = new CAPA
        {
            Id = Guid.NewGuid(),
            FindingId = findingId,
            RootCause = req.RootCause,
            CorrectiveAction = req.CorrectiveAction,
            PreventiveAction = req.PreventiveAction,
            PicId = req.PicId.Value,
            Deadline = req.Deadline.Value,
            Status = CAPAStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        db.CAPAs.Add(capa);
        finding.Status = FindingStatus.InProgress;
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = capa.Id }, capa);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCapaRequestDto req)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        if (!req.Deadline.HasValue)
            return BadRequest(new { message = "Deadline wajib diisi" });
        if (req.PicId.HasValue && req.PicId == Guid.Empty)
            return BadRequest(new { message = "PIC tidak boleh Guid kosong" });

        capa.RootCause = req.RootCause;
        capa.CorrectiveAction = req.CorrectiveAction;
        capa.PreventiveAction = req.PreventiveAction;
        capa.Deadline = req.Deadline.Value;
        if (req.PicId.HasValue)
            capa.PicId = req.PicId.Value;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CAPAStatus status)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        capa.Status = status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/actions")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> AddAction(Guid id, [FromBody] AddCapaActionRequest req)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        if (!req.DoneById.HasValue || req.DoneById == Guid.Empty)
            return BadRequest(new { message = "DoneById harus diisi dan bukan Guid kosong" });

        var action = new CAPAAction
        {
            Id = Guid.NewGuid(),
            CapaId = id,
            Description = req.Description,
            DoneById = req.DoneById.Value,
            DoneAt = DateTime.UtcNow
        };

        db.CAPAActions.Add(action);
        capa.Status = CAPAStatus.InProgress;
        await db.SaveChangesAsync();
        return Ok(action);
    }

    [HttpPost("{id}/closeout")]
    [Authorize(Roles = "Admin,QualityManager,Auditor,Auditee")]
    public async Task<IActionResult> CloseOut(Guid id, [FromBody] CloseOutVerificationRequest req)
    {
        var capa = await db.CAPAs.Include(c => c.Finding).FirstOrDefaultAsync(c => c.Id == id);
        if (capa is null) return NotFound();
        if (!req.IsEffective.HasValue)
            return BadRequest(new { message = "IsEffective wajib diisi" });
        if (!req.VerifiedById.HasValue || req.VerifiedById == Guid.Empty)
            return BadRequest(new { message = "VerifiedById harus diisi dan bukan Guid kosong" });

        var verification = new CloseOutVerification
        {
            Id = Guid.NewGuid(),
            CapaId = id,
            IsEffective = req.IsEffective.Value,
            VerificationNotes = req.VerificationNotes,
            VerifiedById = req.VerifiedById.Value,
            VerifiedAt = DateTime.UtcNow
        };

        db.CloseOutVerifications.Add(verification);
        capa.Status = CAPAStatus.Closed;
        if (capa.Finding is not null)
            capa.Finding.Status = FindingStatus.Closed;

        await db.SaveChangesAsync();
        return Ok(verification);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,QualityManager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        db.CAPAs.Remove(capa);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
