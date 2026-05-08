using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;

[ApiController]
[Route("api/[controller]")]
public class ChecklistController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? standard,
        [FromQuery] string? department)
    {
        var query = db.Checklists.Include(c => c.Items).AsQueryable();
        if (!string.IsNullOrEmpty(standard)) query = query.Where(c => c.Standard == standard);
        if (!string.IsNullOrEmpty(department)) query = query.Where(c => c.Department == department);
        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var checklist = await db.Checklists.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
        return checklist is null ? NotFound() : Ok(checklist);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Checklist checklist)
    {
        checklist.Id = Guid.NewGuid();
        checklist.CreatedAt = DateTime.UtcNow;
        foreach (var item in checklist.Items)
            item.Id = Guid.NewGuid();
        db.Checklists.Add(checklist);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = checklist.Id }, checklist);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var checklist = await db.Checklists.FindAsync(id);
        if (checklist is null) return NotFound();
        db.Checklists.Remove(checklist);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
