namespace BooksyClone.Domain.Availability.LockingTimeslotOnResource;

public static class AvailabilityErrorFactory
{
	public static Error LockingResourceFailedDueToDayOfWeekTimeRestricitonPolicy(Guid? resourceId = null, DateTime? from = null, DateTime? to = null)
	{
		if (resourceId is null || from is null || to is null)
			return new Error("Resource could not be locked due to daily time resrictions policy ", "LockingResourceFailedDueToDayOfWeekTimeRestricitonPolicy");

		return new Error(
						$"Resource @resourceId could not be locked from @from to @to due to daily time resrictions policy ",
						"LockingResourceFailedDueToDayOfWeekTimeRestricitonPolicy");
	}

}
