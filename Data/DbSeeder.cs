using Microsoft.EntityFrameworkCore;
using QualiTrack.Data;
using QualiTrack.Models;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Checklists.Any()) return;

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
                    new() { Id = Guid.NewGuid(), Question = "Penerimaan barang sesuai prosedur", ClauseRef = "ISO9001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Pemeriksaan kualitas barang masuk", ClauseRef = "ISO9001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Labeling & identifikasi barang jelas", ClauseRef = "ISO9001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Penerapan FIFO/FEFO", ClauseRef = "ISO9001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Pencatatan stok akurat", ClauseRef = "ISO9001", OrderIndex = 5 },
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
                    new() { Id = Guid.NewGuid(), Question = "Pemisahan limbah sesuai kategori", ClauseRef = "ISO14001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Pengelolaan limbah (karton, plastik, dll)", ClauseRef = "ISO14001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Penyimpanan bahan berbahaya aman", ClauseRef = "ISO14001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Tidak ada kebocoran/pencemaran", ClauseRef = "ISO14001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Tersedia tempat sampah terpisah", ClauseRef = "ISO14001", OrderIndex = 5 },
                ]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ISO 9001 - Produksi",
                Standard = "ISO9001",
                Department = "Produksi",
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    new() { Id = Guid.NewGuid(), Question = "SOP produksi tersedia & dipatuhi", ClauseRef = "ISO9001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Mesin dalam kondisi layak", ClauseRef = "ISO9001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Quality control selama proses", ClauseRef = "ISO9001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Produk cacat dicatat & ditangani", ClauseRef = "ISO9001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Dokumentasi produksi lengkap", ClauseRef = "ISO9001", OrderIndex = 5 },
                ]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ISO 14001 - Produksi",
                Standard = "ISO14001",
                Department = "Produksi",
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    new() { Id = Guid.NewGuid(), Question = "Pengelolaan limbah produksi", ClauseRef = "ISO14001", OrderIndex = 1 },
                    new() { Id = Guid.NewGuid(), Question = "Emisi (asap/gas) terkontrol", ClauseRef = "ISO14001", OrderIndex = 2 },
                    new() { Id = Guid.NewGuid(), Question = "Penggunaan energi efisien", ClauseRef = "ISO14001", OrderIndex = 3 },
                    new() { Id = Guid.NewGuid(), Question = "Pengelolaan air limbah", ClauseRef = "ISO14001", OrderIndex = 4 },
                    new() { Id = Guid.NewGuid(), Question = "Penyimpanan bahan kimia aman", ClauseRef = "ISO14001", OrderIndex = 5 },
                ]
            },
        };

        db.Checklists.AddRange(checklists);
        await db.SaveChangesAsync();
    }
}
