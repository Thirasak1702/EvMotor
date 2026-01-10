namespace EbikeRental.Shared.Helpers;

public static class DateHelper
{
    public static int CalculateDaysDifference(DateTime startDate, DateTime endDate)
    {
        return (endDate.Date - startDate.Date).Days;
    }

    public static bool IsOverdue(DateTime dueDate)
    {
        return DateTime.UtcNow.Date > dueDate.Date;
    }

    public static string ToThaiDateFormat(DateTime date)
    {
        return date.ToString("dd/MM/yyyy");
    }

    public static DateTime GetCurrentThaiTime()
    {
        var thaiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, thaiTimeZone);
    }
}
