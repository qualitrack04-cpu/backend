using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Checklists.AnyAsync()) return;

        var checklists = new List<Checklist>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ISO 9001 - Warehouse",
                Standard = "ISO9001",
                Department = "Warehouse",
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    new() { Id = Guid.NewGuid(), Question = "Goods receipt follows established procedure", ClauseRef = "ISO9001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Incoming goods quality inspection conducted", ClauseRef = "ISO9001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Goods labeling and identification are clear", ClauseRef = "ISO9001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "FIFO/FEFO principle is applied", ClauseRef = "ISO9001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Stock records are accurate and up to date", ClauseRef = "ISO9001", OrderIndex = 5 },
                ]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ISO 14001 - Warehouse",
                Standard = "ISO14001",
                Department = "Warehouse",
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    new() { Id = Guid.NewGuid(), Question = "Waste is separated according to category", ClauseRef = "ISO14001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Waste management (cardboard, plastic, etc.) is in place", ClauseRef = "ISO14001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Hazardous materials are stored safely", ClauseRef = "ISO14001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "No leaks or environmental contamination found", ClauseRef = "ISO14001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Separate waste bins are available", ClauseRef = "ISO14001", OrderIndex = 5 },
                ]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ISO 9001 - Production",
                Standard = "ISO9001",
                Department = "Produksi",
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    new() { Id = Guid.NewGuid(), Question = "Production SOP is available and followed", ClauseRef = "ISO9001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Machines are in proper working condition", ClauseRef = "ISO9001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Quality control is performed during production", ClauseRef = "ISO9001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Defective products are recorded and handled", ClauseRef = "ISO9001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Production documentation is complete", ClauseRef = "ISO9001", OrderIndex = 5 },
                ]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ISO 14001 - Production",
                Standard = "ISO14001",
                Department = "Produksi",
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    new() { Id = Guid.NewGuid(), Question = "Production waste is properly managed", ClauseRef = "ISO14001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Emissions (smoke/gas) are controlled", ClauseRef = "ISO14001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Energy usage is efficient", ClauseRef = "ISO14001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Wastewater is properly managed", ClauseRef = "ISO14001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Chemical storage is safe and compliant", ClauseRef = "ISO14001", OrderIndex = 5 },
                ]
            },
        };

        db.Checklists.AddRange(checklists);
        await db.SaveChangesAsync();
    }
}