using Azure.Core;
using BooksyClone.Contract.Availability;
using BooksyClone.Contract.Availability.UpdatingPolicies;
using BooksyClone.Domain.Availability.CreatingNewResource;
using BooksyClone.Domain.Availability.LockingTimeslotOnResource;
using BooksyClone.Domain.Availability.UpdatingResourceLockingPolicy;
using BooksyClone.Domain.Schedules;
using Newtonsoft.Json;

namespace BooksyClone.Domain.Availability;
public class AvailabilityFacade
{
    private readonly CreateNewResource _createNewResource;
    private readonly GenerateLock _generateLock;
	private readonly UpdateResourcePolicy _updateResourcePolicy;
	private readonly GetResourcePolicies _getResourcePolicies;

	internal AvailabilityFacade(CreateNewResource createNewResource,
        GenerateLock generateLock,
		UpdateResourcePolicy updateResourcePolicy,
		GetResourcePolicies getResourcePolicies)
    {
        _createNewResource = createNewResource;
        _generateLock = generateLock;
		_updateResourcePolicy = updateResourcePolicy;
		_getResourcePolicies = getResourcePolicies;
	}

    public async Task<Result> CreateNewResource(CreateNewResourceRequest request)
    {
        await _createNewResource.Handle(request);
        return Result.Correct();
    }

    public async Task<Result> GenerateLockAsync(GenerateNewLockRequest request)
    {
        return await _generateLock.Handle(request);
    }

	public async Task<List<BaseTimeRestrictionsPolicyDto>> GetResourceRestrictionsPolicies(Guid resourceId, DateTime from, DateTime to)
	{
		return await _getResourcePolicies.Handle(resourceId, from, to);
	}

	public async Task UpdateResourceRestrictionsPolicy(UpdateResourceRestrictionsPolicyRequest request)
    {
		await _updateResourcePolicy.Handle(request);
    }
}
