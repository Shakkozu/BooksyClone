using System.Text.Json.Serialization;

namespace BooksyClone.Contract.Availability.UpdatingPolicies;

[JsonConverter(typeof(PolicyDtoConverter))]
public abstract class BaseTimeRestrictionsPolicyDto
{
	public abstract string PolicyType { get; }
}

public class DayOfWeekTimeRestrictionsPolicyDto : BaseTimeRestrictionsPolicyDto
{
	public override string PolicyType { get => "DayOfWeekTimeRestrictionsPolicy"; }
	public List<DayOfWeekTimeRestriction> DaysDefinition { get; set; }
}

public class TestPolicyDto : BaseTimeRestrictionsPolicyDto
{
	public override string PolicyType { get => "TestPolicy"; }

	public string TestProperty { get; set; }
}

public class DayOfWeekTimeRestriction
{
	public DayOfWeek DayOfWeek { get; set; }
	public TimeSpan StartTime { get; set; }
	public TimeSpan EndTime { get; set; }
}
