using BooksyClone.Contract.Availability;
using BooksyClone.Domain.Availability;
using Npgsql;
using System.Globalization;

namespace BooksyClone.Tests.Availability;

[TestFixture]
public class LockingTermsAcceptanceTests
{
    private readonly BooksyCloneApp _app = BooksyCloneApp.CreateInstance();
    private AvailabilityFacade _facade;
    private AvailabilityFixture _fixture;
    private Guid _resourceAId;
    private Guid _resourceBId;

    [OneTimeTearDown]
    public void TearDown()
    {
        _app.Dispose();
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _facade = _app.AvailabilityFacade;
        _fixture = _app.AvailabilityFixture;
    }

    [SetUp]
    public void Setup()
    {
        _resourceAId = Guid.NewGuid();
        _resourceBId = Guid.NewGuid();
    }

    // Given when new resource is created with POST availability/resources
    // Then locks list fetched by GET availability/resources/{resourceId}/blockades is empty
    // When owner restricts range where locks can be created with /PUT availability/resources/{resourceId}/restrict from 8:00 to 16:00 from Monday to Friday
    // Then locking timeslots outside that range with /POST /availability/resources/{resourceId}/lock fails with error
    // And locking timeslots inside that range with /POST /availability/resources/{resourceId}/lock succeedes
    [Test]
    [Ignore("not-implemented-yet")]
    public async Task UserMightCreateNewResourceAndDefineItsRestrictions()
    {
    }

    [Test]
    public async Task ShouldCreateLocksOnResources_ButForbidLockingTheSameTimeslot()
    {
        await _fixture.GenerateNewResourceViaCorrelationId(_resourceAId);
        await _fixture.GenerateNewResourceViaCorrelationId(_resourceBId);

        var currentTime = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        _app.UpdateCurrentAppTime(currentTime);

        var blockadeRequestOnA = new GenerateNewLockRequest(
            _resourceAId,
            _fixture.FirstOwnerId,
            DateTime.ParseExact("15-12-2024T12:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
             );
        var blockadeRequestOnB = new GenerateNewLockRequest(
            _resourceBId,
            _fixture.FirstOwnerId,
            DateTime.ParseExact("15-12-2024T12:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
             );

        await _facade.GenerateLockAsync(blockadeRequestOnA);
        await _facade.GenerateLockAsync(blockadeRequestOnB);

        Assert.ThrowsAsync<PostgresException>(async () => await _facade.GenerateLockAsync(blockadeRequestOnA));
        Assert.ThrowsAsync<PostgresException>(async () => await _facade.GenerateLockAsync(blockadeRequestOnB));
    }

    [Test]
    public async Task SingleResourceCannotCreateLocksOnTheOverlappingTimeslot()
    {
        await _fixture.GenerateNewResourceViaCorrelationId(_resourceAId);
        var currentTime = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        _app.UpdateCurrentAppTime(currentTime);
        var exsitingTimeslotReservation = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T12:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };
        await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(exsitingTimeslotReservation));

        var timeslotOverlappingOnStart = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T11:45:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:00:01", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };
        var timeslotOverlappingOnEnd = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T12:29:59", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:30:01", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };
        var timeslotOverlappingWithin = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T12:15:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };
        Assert.ThrowsAsync<PostgresException>(async () => await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(timeslotOverlappingOnStart)));
        Assert.ThrowsAsync<PostgresException>(async () => await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(timeslotOverlappingOnEnd)));
        Assert.ThrowsAsync<PostgresException>(async () => await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(timeslotOverlappingWithin)));
    }

    [Test]
    public async Task ShouldBeAbleToCreateMultipleLocksOnTheSameResource()
    {
        await _fixture.GenerateNewResourceViaCorrelationId(_resourceAId);
        var currentTime = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        _app.UpdateCurrentAppTime(currentTime);
        var first = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T12:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };
        var second = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:45:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };
        var third = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T11:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };
        var onAnotherDay = new DateTimePair
        {
            Start = DateTime.ParseExact("16-12-2024T12:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("16-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };

        await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(first));
        await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(second));
        await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(third));
        await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(onAnotherDay));
    }

    [Test]
    public async Task CannotCreateTimeslotWithDdateFromAfterDateTo()
    {
        await _fixture.GenerateNewResourceViaCorrelationId(_resourceAId);
        var currentTime = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        _app.UpdateCurrentAppTime(currentTime);

        var incorrectTimerange = new DateTimePair
        {
            Start = DateTime.ParseExact("15-12-2024T13:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture),
            End = DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture)
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await _facade.GenerateLockAsync(GenerateRequestFromDateTimes(incorrectTimerange)));
    }

    [Test]
    [Ignore("not-implemented-yet")]
    public async Task MultipleResourceMightCreateLocksOnTheSameTimeslot()
    {
        Assert.Fail();
    }

    private GenerateNewLockRequest GenerateRequestFromDateTimes(DateTimePair pair)
    {
        return new GenerateNewLockRequest(
            _resourceAId,
            _fixture.FirstOwnerId,
            pair.Start, pair.End);
    }

    private class DateTimePair
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}