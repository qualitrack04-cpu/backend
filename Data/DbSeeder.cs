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
                    new() { Id = Guid.NewGuid(), Question = "Goods receipt follows established procedure", Description = "Verify that all incoming goods are received according to the documented receiving procedure, including checking purchase orders and delivery notes.", ClauseRef = "ISO9001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Incoming goods quality inspection conducted", Description = "Confirm that a quality inspection is performed on all incoming goods before being stored, including visual checks and quantity verification.", ClauseRef = "ISO9001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Goods labeling and identification are clear", Description = "Ensure all stored goods have clear and legible labels including product name, quantity, batch number, and expiry date where applicable.", ClauseRef = "ISO9001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "FIFO/FEFO principle is applied", Description = "Check that First In First Out (FIFO) or First Expired First Out (FEFO) principles are consistently applied during goods retrieval and dispatch.", ClauseRef = "ISO9001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Stock records are accurate and up to date", Description = "Verify that stock records in the system match physical inventory and are updated in real time or at regular intervals.", ClauseRef = "ISO9001", OrderIndex = 5 },
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
                    new() { Id = Guid.NewGuid(), Question = "Waste is separated according to category", Description = "Confirm that waste is sorted into designated categories such as organic, non-organic, hazardous, and recyclable materials in accordance with environmental policy.", ClauseRef = "ISO14001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Waste management (cardboard, plastic, etc.) is in place", Description = "Verify that packaging waste such as cardboard boxes and plastic wrapping are collected, sorted, and disposed of or recycled through approved channels.", ClauseRef = "ISO14001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Hazardous materials are stored safely", Description = "Check that hazardous materials are stored in designated areas with proper ventilation, secondary containment, and clearly marked warning signs.", ClauseRef = "ISO14001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "No leaks or environmental contamination found", Description = "Inspect the warehouse area for any signs of liquid leaks, spills, or contamination that could impact soil or drainage systems.", ClauseRef = "ISO14001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Separate waste bins are available", Description = "Ensure that clearly labeled waste bins for different waste types are available and accessible throughout the warehouse area.", ClauseRef = "ISO14001", OrderIndex = 5 },
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
                    new() { Id = Guid.NewGuid(), Question = "Production SOP is available and followed", Description = "Verify that Standard Operating Procedures for all production activities are documented, accessible to operators, and consistently followed on the production floor.", ClauseRef = "ISO9001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Machines are in proper working condition", Description = "Confirm that all production machines have undergone scheduled maintenance and are operating within acceptable parameters without defects or safety hazards.", ClauseRef = "ISO9001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Quality control is performed during production", Description = "Check that in-process quality control checks are conducted at defined intervals and results are recorded in the production log.", ClauseRef = "ISO9001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Defective products are recorded and handled", Description = "Ensure that any defective or non-conforming products are identified, tagged, segregated from good products, and reported through the non-conformance process.", ClauseRef = "ISO9001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Production documentation is complete", Description = "Verify that all required production records including batch records, work orders, and inspection reports are completed accurately and filed properly.", ClauseRef = "ISO9001", OrderIndex = 5 },
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
                    new() { Id = Guid.NewGuid(), Question = "Production waste is properly managed", Description = "Confirm that all waste generated during production is collected, categorized, and disposed of according to the environmental waste management plan.", ClauseRef = "ISO14001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Emissions (smoke/gas) are controlled", Description = "Check that air emissions from production processes are within permissible limits and that emission control equipment is functioning properly.", ClauseRef = "ISO14001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Energy usage is efficient", Description = "Verify that energy consumption is monitored and that energy-saving measures such as turning off idle machines and optimizing production schedules are implemented.", ClauseRef = "ISO14001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Wastewater is properly managed", Description = "Ensure that wastewater from production processes is treated before disposal and meets regulatory discharge standards.", ClauseRef = "ISO14001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Chemical storage is safe and compliant", Description = "Inspect that all chemicals used in production are stored in approved containers, properly labeled, and kept in designated storage areas away from ignition sources.", ClauseRef = "ISO14001", OrderIndex = 5 },
                ]
            },
        };

        db.Checklists.AddRange(checklists);
        await db.SaveChangesAsync();
    }
}