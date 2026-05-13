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
public class AuditPlanController : ControllerBase
{
    private readonly AppDbContext _db;
    
    public AuditPlanController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,QualityManager,Auditor")]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _db.AuditPlans
            .Include(a => a.Schedules)
                .ThenInclude(s => s.Auditor)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        
        return Ok(new { message = "Data audit plan berhasil diambil", total = plans.Count, data = plans });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,QualityManager,Auditor")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plan = await _db.AuditPlans
            .Include(a => a.Schedules)
                .ThenInclude(s => s.Auditor)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        if (plan is null)
            return NotFound(new { message = $"Audit plan dengan ID {id} tidak ditemukan", id = id });
        
        return Ok(new { message = "Data audit plan ditemukan", data = plan });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,QualityManager")]
    public async Task<IActionResult> Create([FromBody] CreateAuditPlanDto planDto)
    {
        var plan = new AuditPlan
        {
            Id = Guid.NewGuid(),
            Title = planDto.Title,
            Year = planDto.Year,
            Standard = planDto.Standard,
            CreatedAt = DateTime.UtcNow,
            Description = planDto.Description,
            Priority = planDto.Priority
        };

        if (planDto.Schedules is not null)
        {
            plan.Schedules = planDto.Schedules.Select(scheduleDto => new AuditSchedule
            {
                Id = Guid.NewGuid(),
                ClauseRef = scheduleDto.ClauseRef,
                AuditorId = scheduleDto.AuditorId,
                ScheduledDate = scheduleDto.ScheduledDate!.Value,
                Department = scheduleDto.Department,
                AuditPlan = plan
            }).ToList();
        }

        _db.AuditPlans.Add(plan);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = plan.Id }, new { message = "Audit plan berhasil dibuat", data = plan });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,QualityManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuditPlanDto updatedPlanDto)
    {
        var plan = await _db.AuditPlans
            .Include(a => a.Schedules)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        if (plan is null)
            return NotFound(new { message = $"Audit plan dengan ID {id} tidak ditemukan", id = id });
        
        plan.Title = updatedPlanDto.Title;
        plan.Year = updatedPlanDto.Year;
        plan.Standard = updatedPlanDto.Standard;
        plan.Description = updatedPlanDto.Description;
        plan.Priority = updatedPlanDto.Priority;
        if (updatedPlanDto.Schedules is not null)
        {
            _db.AuditSchedules.RemoveRange(plan.Schedules);
            plan.Schedules = updatedPlanDto.Schedules.Select(scheduleDto => new AuditSchedule
            {
                Id = Guid.NewGuid(),
                ClauseRef = scheduleDto.ClauseRef,
                AuditorId = scheduleDto.AuditorId,
                ScheduledDate = scheduleDto.ScheduledDate!.Value,
                Department = scheduleDto.Department,
                AuditPlan = plan
            }).ToList();
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Audit plan berhasil diupdate", data = plan });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var plan = await _db.AuditPlans.FindAsync(id);
        
        if (plan is null)
            return NotFound(new { message = $"Audit plan dengan ID {id} tidak ditemukan", id = id });
        
        _db.AuditPlans.Remove(plan);
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Audit plan berhasil dihapus", id = id });
    }
}