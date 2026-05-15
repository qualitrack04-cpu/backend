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

        var result = (await query.ToListAsync()).Select(c => new
        {
            c.Id,
            c.Title,
            c.Standard,
            c.Department,
            c.CreatedAt,
            TotalItems = c.Items.Select(i => new
            {
                i.Id,
                i.Question,
                i.Description,
                i.ClauseRef,
                i.OrderIndex
            })
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var checklist = await db.Checklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checklist is null) return NotFound();

        return Ok(new
        {
            checklist.Id,
            checklist.Title,
            checklist.Standard,
            checklist.Department,
            checklist.CreatedAt,
            Items = checklist.Items.Select(i => new
            {
                i.Id,
                i.Question,
                i.Description,
                i.ClauseRef,
                i.OrderIndex
            })
        });
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

    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetItems(Guid id)
    {
        var checklist = await db.Checklists
            .Include(c => c.Items.OrderBy(i => i.OrderIndex))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checklist is null) return NotFound(new { message = "Checklist tidak ditemukan" });

        return Ok(new {
            checklistId = checklist.Id,
            title = checklist.Title,
            standard = checklist.Standard,
            department = checklist.Department,
            totalItems = checklist.Items.Count,
            items = checklist.Items.Select(i => new
            {
                i.Id, 
                i.Question,
                i.Description,
                i.ClauseRef,
                i.OrderIndex
            })
        });
    }
}
