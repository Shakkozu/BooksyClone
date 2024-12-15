using BooksyClone.Contract.Availability;
using BooksyClone.Domain.Availability;

namespace BooksyClone.Tests.Availability;

internal class AvailabilityFixture
{
    private readonly AvailabilityFacade _availabilityFacade;
    public Guid FirstOwnerId { get; set; }

    public AvailabilityFixture(AvailabilityFacade availabilityFacade)
    {
        _availabilityFacade = availabilityFacade;
        FirstOwnerId = Guid.NewGuid();
    }

    internal async Task GenerateNewResourceViaCorrelationId(Guid correlationId, Guid? ownerId = null)
    {
        if (ownerId == null)
            ownerId = FirstOwnerId;

        var request = new CreateNewResourceRequest(correlationId, ownerId.Value);
        await _availabilityFacade.CreateNewResource(request);
    }
}
