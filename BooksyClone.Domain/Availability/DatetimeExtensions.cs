namespace BooksyClone.Domain.Availability;

public static class DatetimeExtensions
{
	public static DateTime GetStartOfDay(this DateTime date)
	{
		return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

	}

	public static DateTime GetEndOfDay(this DateTime date)
	{
		return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
	}
}
