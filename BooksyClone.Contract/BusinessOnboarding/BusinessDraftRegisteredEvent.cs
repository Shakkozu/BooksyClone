using MediatR;

namespace BooksyClone.Contract.BusinessOnboarding;
public record BusinessDraftRegisteredEvent(
    DateTime RegisteredAt,
    Guid BusinessUnitId,
    Guid OwnerId
    ) : INotification;
