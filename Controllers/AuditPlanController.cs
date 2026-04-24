using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuditPlanController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await db.AuditPlans.Include(a => a.Schedules).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plan = await db.AuditPlans.Include(a => a.Schedules).FirstOrDefaultAsync(a => a.Id == id);
        return plan is null ? NotFound() : Ok(plan);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AuditPlan plan)
    {
        plan.Id = Guid.NewGuid();
        plan.CreatedAt = DateTime.UtcNow;
        db.AuditPlans.Add(plan);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, AuditPlan updated)
    {
        var plan = await db.AuditPlans.FindAsync(id);
        if (plan is null) return NotFound();
        plan.Title = updated.Title;
        plan.Year = updated.Year;
        plan.Standard = updated.Standard;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var plan = await db.AuditPlans.FindAsync(id);
        if (plan is null) return NotFound();
        db.AuditPlans.Remove(plan);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
