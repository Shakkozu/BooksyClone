using BooksyClone.Contract.Availability;
using BooksyClone.Domain.Availability.CreatingNewResource;
using BooksyClone.Domain.Availability.LockingTimeslotOnResource;

namespace BooksyClone.Domain.Availability;
public class AvailabilityFacade
{
    private readonly CreateNewResource _createNewResource;
    private readonly GenerateLock _generateLock;

    internal AvailabilityFacade(CreateNewResource createNewResource,
        GenerateLock generateLock)
    {
        _createNewResource = createNewResource;
        _generateLock = generateLock;
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
}
