using System.Globalization;

namespace BooksyClone.Domain.Schedules.Shared;

public class YearMonth
{
    public YearMonth(string str)
    {
        var date = DateTime.ParseExact(str, "yyyy-MM", CultureInfo.InvariantCulture);
        Year = date.Year;
        Month = date.Month;
    }

    public int Year { get; }
    public int Month { get; }



    public override string ToString()
    {
        return $"{Year}-{Month}";
    }
}
