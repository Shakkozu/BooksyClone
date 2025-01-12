using BooksyClone.Contract.Availability.UpdatingPolicies;
using BooksyClone.Domain.Availability.UpdatingResourceLockingPolicy;
using BooksyClone.Domain.Schedules;
using BooksyClone.Domain.Shared;

namespace BooksyClone.Domain.Availability.LockingTimeslotOnResource;

internal interface IResourceLockingLimitationPolicy
{
	internal Result CanLockResource(Guid resourceId, DateTime from, DateTime to);
}

internal class DayOfWeekTimeRestrictionPolicy : IResourceLockingLimitationPolicy
{
	public DayOfWeekTimeRestrictionPolicy(List<DayOfWeekTimeRestriction> dayOfWeekTimeRestrictions)
	{
		DayOfWeekTimeRestrictions = dayOfWeekTimeRestrictions;
	}
	public List<DayOfWeekTimeRestriction> DayOfWeekTimeRestrictions { get; }


	Result IResourceLockingLimitationPolicy.CanLockResource(Guid resourceId, DateTime from, DateTime to)
	{
		var timerange = TimeSlot.FromDates(from, to);
		var dayOfWeek = timerange.From.DayOfWeek;
		var time = timerange.From.TimeOfDay;
		var dayOfWeekTimeRestriction = DayOfWeekTimeRestrictions.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
		if (dayOfWeekTimeRestriction is null)
			return AvailabilityErrorFactory.LockingResourceFailedDueToDayOfWeekTimeRestricitonPolicy(resourceId, from, to).ToErrorResult();

		if (dayOfWeekTimeRestriction.StartTime <= time && dayOfWeekTimeRestriction.EndTime >= time)
			return Result.Correct();

		return AvailabilityErrorFactory.LockingResourceFailedDueToDayOfWeekTimeRestricitonPolicy(resourceId, from, to).ToErrorResult();
	}
}

internal class ResourceLockingLimitationPolicyFactory(GetResourcePolicies _getResourcePolicies)
{
	internal async Task<IEnumerable<IResourceLockingLimitationPolicy>> GetPoliciesForGivenResource(Guid resourceId, DateTime from, DateTime to)
	{
		var policies = await _getResourcePolicies.Handle(resourceId, from, to);
		var result = new List<IResourceLockingLimitationPolicy>();
		foreach (var policyDto in policies)
		{
			result.Add(CreatePolicy(policyDto));
		}
		return result;
	}

	private IResourceLockingLimitationPolicy CreatePolicy(BaseTimeRestrictionsPolicyDto policy)
	{
		return policy switch
		{
			DayOfWeekTimeRestrictionsPolicyDto timeRestrictionsPolicyDto => new DayOfWeekTimeRestrictionPolicy(timeRestrictionsPolicyDto.DaysDefinition),
			_ => throw new InvalidOperationException("unknown policy type")
		};
	}
}
