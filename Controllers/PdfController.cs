using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Services;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PdfController(AppDbContext db, PdfReportService pdfService) : ControllerBase
{
    [HttpGet("audit-report/{sessionId}")]
    public async Task<IActionResult> GenerateAuditReport(Guid sessionId)
    {
        var session = await db.AuditSessions
            .Include(s => s.Schedule)
                .ThenInclude(s => s.AuditPlan)
            .Include(s => s.Schedule)
                .ThenInclude(s => s.Auditor)
            .Include(s => s.Findings)
                .ThenInclude(f => f.Capa)
                    .ThenInclude(c => c!.Pic)
            .Include(s => s.Responses)
                .ThenInclude(r => r.ChecklistItem)
            .Include(s => s.Summary)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
            return NotFound(new { message = "Sesi audit tidak ditemukan" });

        var pdf = pdfService.GenerateAuditReport(session);
        return File(pdf, "application/pdf", $"audit-report-{sessionId}.pdf");
    }
}
