using BooksyClone.Contract.Availability;
using BooksyClone.Contract.Availability.UpdatingPolicies;
using BooksyClone.Domain.Availability;
using FakeItEasy;
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

	[Test]
	public async Task ShouldSaveAndReadPolicies()
	{
		var currentTime = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
		_app.UpdateCurrentAppTime(currentTime);
		await _fixture.GenerateNewResourceViaCorrelationId(_resourceAId);
		var updateRequest = GetDayOfWeekTimeRestrictionsPolicyWhichAllows8To16MondayToFriday();

		var request = new UpdateResourceRestrictionsPolicyRequest
		{
			ResourceId = _resourceAId,
			CreatedBy = _fixture.FirstOwnerId,
			Start = currentTime,
			End = currentTime,
			Policies = [
				new DayOfWeekTimeRestrictionsPolicyDto { DaysDefinition = (updateRequest.Policies[0] as DayOfWeekTimeRestrictionsPolicyDto).DaysDefinition },
				new TestPolicyDto { TestProperty = "dupa"},
			]
		};

		await _facade.UpdateResourceRestrictionsPolicy(request);

		var restrictions = await _facade.GetResourceRestrictionsPolicies(_resourceAId, currentTime, currentTime.AddDays(3));
		Assert.That(restrictions.Count, Is.EqualTo(2));
		Assert.That(restrictions[0].PolicyType, Is.EqualTo("DayOfWeekTimeRestrictionsPolicy"));
		Assert.That(restrictions[1].PolicyType, Is.EqualTo("TestPolicy"));
	}

	// Given when new resource is created with POST availability/resources
	// Then locks list fetched by GET availability/resources/{resourceId}/blockades is empty
	// When owner restricts range where locks can be created with /PUT availability/resources/{resourceId}/restrict from 8:00 to 16:00 from Monday to Friday
	// Then locking timeslots outside that range with /POST /availability/resources/{resourceId}/lock fails with error
	// And locking timeslots inside that range with /POST /availability/resources/{resourceId}/lock succeedes
	[Test]
	public async Task UserMightCreateNewResourceAndDefineItsRestrictions()
	{
		await _fixture.GenerateNewResourceViaCorrelationId(_resourceAId);
		var restrictionPolicyRequest = GetDayOfWeekTimeRestrictionsPolicyWhichAllows8To16MondayToFriday();

		await _facade.UpdateResourceRestrictionsPolicy(restrictionPolicyRequest);

		var currentTime = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
		_app.UpdateCurrentAppTime(currentTime);

		var requestFailedDueToForbiddedDay = ALockRequestForDay(DayOfWeek.Saturday);
		var requestFailedDueToForbiddedTime = ALockRequestForDay(DayOfWeek.Monday, new TimeSpan(7, 59, 59), new TimeSpan(8, 0, 1));
		var requestSucceed = ALockRequestForDay(DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(8, 30, 0));

		await _facade.GenerateLockAsync(requestSucceed);
		Assert.ThrowsAsync<PostgresException>(async () => await _facade.GenerateLockAsync(requestFailedDueToForbiddedDay));
		Assert.ThrowsAsync<PostgresException>(async () => await _facade.GenerateLockAsync(requestFailedDueToForbiddedTime));
	}

	private UpdateResourceRestrictionsPolicyRequest GetDayOfWeekTimeRestrictionsPolicyWhichAllows8To16MondayToFriday()
	{
		var policy = new DayOfWeekTimeRestrictionsPolicyDto
		{
			DaysDefinition =
			[
				new DayOfWeekTimeRestriction
				{
					DayOfWeek = DayOfWeek.Monday,
					StartTime = new TimeSpan(8, 0, 0),
					EndTime = new TimeSpan(16, 0, 0)
				},
				new DayOfWeekTimeRestriction
				{
					DayOfWeek = DayOfWeek.Tuesday,
					StartTime = new TimeSpan(8, 0, 0),
					EndTime = new TimeSpan(16, 0, 0)
				},
				new DayOfWeekTimeRestriction
				{
					DayOfWeek = DayOfWeek.Wednesday,
					StartTime = new TimeSpan(8, 0, 0),
					EndTime = new TimeSpan(16, 0, 0)
				},
				new DayOfWeekTimeRestriction
				{
					DayOfWeek = DayOfWeek.Thursday,
					StartTime = new TimeSpan(8, 0, 0),
					EndTime = new TimeSpan(16, 0, 0)
				},
				new DayOfWeekTimeRestriction
				{
					DayOfWeek = DayOfWeek.Friday,
					StartTime = new TimeSpan(8, 0, 0),
					EndTime = new TimeSpan(16, 0, 0)
				}
			]
		};
		return new UpdateResourceRestrictionsPolicyRequest
		{
			ResourceId = _resourceAId,
			CreatedBy = _fixture.FirstOwnerId,
			Start = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
			End = DateTime.ParseExact("2024-12-22T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
			Policies = [policy]
		};
	}

	private GenerateNewLockRequest ALockRequestForDay(DayOfWeek dayOfWeek, TimeSpan? start = null, TimeSpan? end = null)
	{
		var todayDate = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
		var today = todayDate.DayOfWeek;
		var daysToMove = dayOfWeek - today;
		var date = todayDate.AddDays(daysToMove);
		start ??= new TimeSpan(12, 0, 0);
		end ??= new TimeSpan(12, 30, 0);
		return new GenerateNewLockRequest(
			_resourceAId,
			_fixture.FirstOwnerId,
			date.Date.Add(start.Value),
			date.Date.Add(end.Value)
		);
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
	public async Task MultipleResourceMightCreateLocksOnTheSameTimeslot()
	{
		await _fixture.GenerateNewResourceViaCorrelationId(_resourceAId);
		await _fixture.GenerateNewResourceViaCorrelationId(_resourceBId);

		var currentTime = DateTime.ParseExact("2024-12-15T10:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
		_app.UpdateCurrentAppTime(currentTime);

		var startDate = DateTime.ParseExact("15-12-2024T12:00:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
		var endDate = DateTime.ParseExact("15-12-2024T12:30:00", "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

		var blockadeRequestOnA = new GenerateNewLockRequest(
			_resourceAId,
			_fixture.FirstOwnerId,
			startDate, endDate
			 );
		var blockadeRequestOnB = new GenerateNewLockRequest(
			_resourceBId,
			_fixture.FirstOwnerId,
			startDate, endDate
			 );

		await _facade.GenerateLockAsync(blockadeRequestOnA);
		await _facade.GenerateLockAsync(blockadeRequestOnB);
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