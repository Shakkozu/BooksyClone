using BooksyClone.Domain;

namespace BooksyClone.Tests;

public class TimeslotTests
{
    [TestCase(1,5)]
    [TestCase(13,14)]
    [TestCase(1,8)]
    [TestCase(10,13)]
    public void ShouldReturnFalse_WhenTimeSlotsDoesNotMatch(int fromHours, int toHours)
    {
        var source = TimeSlot.FromDates(DateTime.Today.AddHours(6), DateTime.Today.AddHours(12));
        var another = TimeSlot.FromDates(DateTime.Today.AddHours(fromHours), DateTime.Today.AddHours(toHours));

        Assert.IsFalse(another.IsFullyWithin(source));
    }

    [TestCase(6,12)]
    [TestCase(7,8)]
    public void ShouldReturnTrue_WhenTimeSlotIsWithinOutsideTimeslot(int fromHours, int toHours)
    {
        var source = TimeSlot.FromDates(DateTime.Today.AddHours(6), DateTime.Today.AddHours(12));
        var another = TimeSlot.FromDates(DateTime.Today.AddHours(fromHours), DateTime.Today.AddHours(toHours));

        Assert.IsTrue(another.IsFullyWithin(source));
    }
}