namespace BooksyClone.Domain.Schedules;

public class DailySchedule
{
    public DateTime Day { get; }
    public Guid EmployeeId { get; }
    public Guid SupervisorId { get; }
    public HashSet<Reservation> Reservations { get; }
    public TimeSlot WorkingHours { get; }

    public static DailySchedule From(DateTime today,
        Guid employeeId,
        Guid supervisorId,
        HashSet<Reservation> reservations,
        TimeSlot workingHours)
    {
        return new DailySchedule(today, employeeId, supervisorId, reservations, workingHours);
    }

    private DailySchedule(DateTime day,
        Guid employeeId,
        Guid supervisorId,
        HashSet<Reservation> reservations,
        TimeSlot workingHours)
    {
        Day = day;
        EmployeeId = employeeId;
        SupervisorId = supervisorId;
        Reservations = reservations;
        WorkingHours = workingHours;
    }

    public TimeslotReservedEvent Reserve(Guid reservedBy, TimeSlot reservationTimeslot)
    {
        if (!reservationTimeslot.IsFullyWithin(WorkingHours))
        {
            throw new TimeSlotOutsideBoundariesException(reservationTimeslot, "Reservation can only be made within working hours");
        }
        if (Reservations.Any(reservation => reservation.OverlapsWith(reservationTimeslot)))
        {
            throw new InvalidOperationException("Cannot reserve already occupied term");
        }

        Reservations.Add(new Reservation(reservedBy, reservationTimeslot));
        return new TimeslotReservedEvent
        {
            ReservedBy = reservedBy,
            ReservedTimeslot = reservationTimeslot,
            Timestamp = DateTime.Now
        };

    }

    internal void Apply(IDailyScheduleEvent @event)
    {
        if (@event is TimeslotReservedEvent timeslotReservedEvent)
        {
            Reservations.Add(new Reservation(timeslotReservedEvent.ReservedBy, timeslotReservedEvent.ReservedTimeslot));
            return;
        }

        if (@event is BlockadeCreatedEvent blockadeCreatedEvent)
        {
            return;
        }
    }
}

internal interface IDailyScheduleEvent
{
    public DateTime Timestamp { get; set; }
}
public record TimeslotReservedEvent : IDailyScheduleEvent
{
    public Guid ReservedBy { get; set; }
    public TimeSlot ReservedTimeslot { get; set; }
    public DateTime Timestamp { get; set; }

}

public record BlockadeCreatedEvent : IDailyScheduleEvent
{
    public DateTime Timestamp { get; set; }
    public ISet<ConflictingReservation> ConflictingReservations { get; set; }
}

public record ConflictingReservation(Guid ReservationCreatorId, TimeSlot TimeSlot);





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

public class Reservation
{
    private Guid _reservedBy;
    private TimeSlot _reservationTimeslot;

    public Reservation(Guid reservedBy, TimeSlot reservationTimeslot)
    {
        _reservedBy = reservedBy;
        _reservationTimeslot = reservationTimeslot;
    }

    internal bool OverlapsWith(TimeSlot reservationTimeslot)
    {
        return _reservationTimeslot.OverlapsWith(reservationTimeslot);
    }
}

public class TimeSlotOutsideBoundariesException : Exception
{
    private TimeSlot _reservationTimeslot;
    private string _v;

    public TimeSlotOutsideBoundariesException(string message) : base(message)
    {

    }

    public TimeSlotOutsideBoundariesException(TimeSlot reservationTimeslot)
    {
        _reservationTimeslot = reservationTimeslot;
    }

    public TimeSlotOutsideBoundariesException(TimeSlot reservationTimeslot, string message) : base(message)
    {
        _reservationTimeslot = reservationTimeslot;
    }
}