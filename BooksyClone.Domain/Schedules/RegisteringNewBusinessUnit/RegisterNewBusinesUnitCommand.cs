namespace BooksyClone.Domain.Schedules.RegisteringNewBusinessUnit;

public record RegisterNewBusinesUnitCommand(Guid BusinessUnitId, Guid OwnerId);
