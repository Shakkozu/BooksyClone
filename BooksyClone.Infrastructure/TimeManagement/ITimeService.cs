namespace BooksyClone.Infrastructure.TimeManagement;
public interface ITimeService
{
    DateTime Now { get; }
    DateTime Today { get; }
    DateTime UtcNow { get; }
}

public class TimeService : ITimeService
{
    public DateTime Now => DateTime.Now;

    public DateTime Today => DateTime.Today;

    public DateTime UtcNow => DateTime.UtcNow;
}