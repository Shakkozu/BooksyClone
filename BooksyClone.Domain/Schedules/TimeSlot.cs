namespace BooksyClone.Domain.Schedules;

public class TimeSlot
{
    public DateTime From { get; }
    public DateTime To { get; }

    public static TimeSlot FromDates(DateTime from, DateTime to)
    {
        return new TimeSlot(from, to);
    }

    public bool IsFullyWithin(TimeSlot another)
    {
        return From >= another.From && To <= another.To;
    }

    public bool OverlapsWith(TimeSlot another)
    {
        return From < another.To && To > another.From;
    }

    private TimeSlot(DateTime from, DateTime to)
    {
        if (from >= to)
            throw new ArgumentException("from date cannot be larger than to date");
        From = from;
        To = to;
    }

}
