using System.Globalization;

namespace QualiTrack.Helpers;

public static class DateHelper
{
    private static readonly TimeZoneInfo WibZone = ResolveWibZone();

    // Format tanggal yang dipakai seluruh response API agar konsisten & human readable.
    public const string ReadableDateFormat = "dd MMMM yyyy";
    public const string ReadableDateTimeFormat = "dd MMMM yyyy, HH:mm";

    private static readonly CultureInfo ReadableCulture = CultureInfo.GetCultureInfo("en-US");

    public static DateOnly TodayWib() =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, WibZone));

    /// <summary>
    /// Format tanggal (tanpa jam) menjadi contoh: "21 August 2026".
    /// Nilai dari database diperlakukan sebagai UTC lalu dikonversi ke WIB.
    /// </summary>
    public static string FormatDate(DateTime value) =>
        TimeZoneInfo.ConvertTimeFromUtc(EnsureUtc(value), WibZone)
            .ToString(ReadableDateFormat, ReadableCulture);

    public static string? FormatDate(DateTime? value) =>
        value.HasValue ? FormatDate(value.Value) : null;

    /// <summary>
    /// Format timestamp menjadi contoh: "21 August 2026, 14:30".
    /// </summary>
    public static string FormatDateTime(DateTime value) =>
        TimeZoneInfo.ConvertTimeFromUtc(EnsureUtc(value), WibZone)
            .ToString(ReadableDateTimeFormat, ReadableCulture);

    public static string? FormatDateTime(DateTime? value) =>
        value.HasValue ? FormatDateTime(value.Value) : null;

    /// <summary>
    /// Format tanggal yang memang tidak memiliki komponen waktu (mis. CAPA Deadline).
    /// </summary>
    public static string FormatDate(DateOnly value) =>
        value.ToString(ReadableDateFormat, ReadableCulture);

    /// <summary>
    /// Format tanggal dari komponen tahun/bulan/tanggal tanpa konversi timezone.
    /// </summary>
    public static string FormatDate(int year, int month, int day) =>
        new DateOnly(year, month, day).ToString(ReadableDateFormat, ReadableCulture);

    private static DateTime EnsureUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        // Npgsql legacy timestamp behavior mengembalikan Kind=Unspecified,
        // padahal nilai yang disimpan selalu DateTime.UtcNow.
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    private static TimeZoneInfo ResolveWibZone()
    {
        foreach (var id in new[] { "Asia/Jakarta", "SE Asia Standard Time " })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
        }

        // Fallback WIB tetap UTC+7 dan Indonesia tidak memakai DST
        return TimeZoneInfo.CreateCustomTimeZone("WIB", TimeSpan.FromHours(7), "WIB", "WIB");
    }
}