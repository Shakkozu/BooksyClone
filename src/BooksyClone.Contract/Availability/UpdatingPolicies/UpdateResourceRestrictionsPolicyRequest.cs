namespace BooksyClone.Contract.Availability.UpdatingPolicies;

public class UpdateResourceRestrictionsPolicyRequest
{
	public Guid ResourceId { get; set; }
	public Guid CreatedBy { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public BaseTimeRestrictionsPolicyDto[] Policies { get; set; }
}