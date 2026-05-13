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

        var result = plans.Select(plan => new AuditPlanResponseDto
        {
            Id = plan.Id,
            Title = plan.Title,
            Year = plan.Year,
            Standard = plan.Standard,
            CreatedAt = plan.CreatedAt,
            TotalSchedules = plan.Schedules.Count,
            Schedules = plan.Schedules.Select(s => new ScheduleResponseDto
            {
                Id = s.Id,
                ClauseRef = s.ClauseRef,
                AuditorId = s.AuditorId ?? Guid.Empty,
                AuditorName = s.Auditor != null
                    ? s.Auditor.FullName
                    : s.AuditorName,  // Fallback ke AuditorName jika join gagal
                ScheduledDate = s.ScheduledDate,
                Department = s.Department
            }).ToList()
        });
        
        return Ok(new { message = "Data audit plan berhasil diambil", total = plans.Count, data = result });
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
        
        var result = new AuditPlanResponseDto
        {
            Id = plan.Id,
            Title = plan.Title,
            Year = plan.Year,
            Standard = plan.Standard,
            CreatedAt = plan.CreatedAt,
            TotalSchedules = plan.Schedules.Count,
            Schedules = plan.Schedules.Select(s => new ScheduleResponseDto
            {
                Id = s.Id,
                ClauseRef = s.ClauseRef,
                AuditorId = s.AuditorId ?? Guid.Empty,
                AuditorName = s.Auditor != null
                    ? s.Auditor.FullName
                    : s.AuditorName,  // Fallback ke AuditorName jika join gagal
                ScheduledDate = s.ScheduledDate,
                Department = s.Department
            }).ToList()
        };

        return Ok(new { message = "Data audit plan ditemukan", data = result });
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
            var schedules = new List<AuditSchedule>();
            foreach (var scheduleDto in planDto.Schedules)
            {
                var auditor = await _db.Users
                    .FirstOrDefaultAsync(u => u.FullName == scheduleDto.AuditorName);
                if (auditor is null)
                {
                    return BadRequest(new { message = $"Auditor dengan nama {scheduleDto.AuditorName} tidak ditemukan" });
                }

                schedules.Add(new AuditSchedule
                {
                    Id = Guid.NewGuid(),
                    ClauseRef = scheduleDto.ClauseRef,
                    AuditorId = auditor.Id,
                    AuditorName = auditor.FullName,
                    ScheduledDate = scheduleDto.ScheduledDate!.Value,
                    Department = scheduleDto.Department,
                    AuditPlan = plan
                });
            }
            plan.Schedules = schedules;
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
            var newSchedules = new List<AuditSchedule>();
            foreach (var scheduleDto in updatedPlanDto.Schedules)
            {
                var auditor = await _db.Users
                    .FirstOrDefaultAsync(u => u.FullName == scheduleDto.AuditorName);
                if (auditor is null)
                {
                    return BadRequest(new { message = $"Auditor dengan nama {scheduleDto.AuditorName} tidak ditemukan" });
                }

                newSchedules.Add(new AuditSchedule
                {
                    Id = Guid.NewGuid(),
                    ClauseRef = scheduleDto.ClauseRef,
                    AuditorId = auditor.Id,
                    AuditorName = auditor.FullName,
                    ScheduledDate = scheduleDto.ScheduledDate!.Value,
                    Department = scheduleDto.Department,
                    AuditPlanId = plan.Id
                });
            }
            _db.AuditSchedules.AddRange(newSchedules);
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