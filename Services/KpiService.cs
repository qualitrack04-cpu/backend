using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.DTOs;
using QualiTrack.Models;

namespace QualiTrack.Services;

public class KpiService(AppDbContext db) : IKpiService
{
    public async Task<KpiDto> GetUserKpiAsync(Guid userId)
    {
        var userCapas = await db.CAPAs
            .Where(c => c.PicId == userId)
            .ToListAsync();

        var closedCapas = userCapas.Where(c => c.Status == CAPAStatus.Closed).ToList();

        var closedOnTime = closedCapas.Count(c =>
            c.ClosedAt.HasValue &&
            DateOnly.FromDateTime(c.ClosedAt.Value) <= c.Deadline);

        var totalFindingsReported = await db.Findings
            .CountAsync(f=> f.ReporterId == userId);

        return new KpiDto
        {
            TotalCapaAssigned = userCapas.Count,
            TotalCapaClosed = closedCapas.Count,
            TotalCapaOpenInProgress = userCapas.Count - closedCapas.Count,
            TotalCapaClosedOnTime = closedOnTime,
            TotalFindingsReported = totalFindingsReported,
            OnTimeCompletionRate = closedCapas.Count == 0
                ? 0
                : (double)closedOnTime / closedCapas.Count
        };
    }
}
